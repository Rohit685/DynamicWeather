using DynamicWeather.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicWeather.API;
using DynamicWeather.Enums;
using DynamicWeather.Helpers;
using Rage;
using Rage.Attributes;
using Rage.Native;

namespace DynamicWeather
{
    internal class Forecast
    {
        internal List<Weather> WeatherList = new List<Weather>();
        internal int currWeatherIndex { get; private set; }
        private List<Text> TextList;
        private List<Texture> TexturesList;
        private double timeInterval;
        internal static Random random = new Random(DateTime.Today.Millisecond);
        private bool ForecastRunning;

        private static WeatherTypesEnum[] stages =
        {
            WeatherTypesEnum.ExtraSunny,
            WeatherTypesEnum.Clear,
            WeatherTypesEnum.Foggy,
            WeatherTypesEnum.Neutral,
            WeatherTypesEnum.Smog,
            WeatherTypesEnum.Clouds,
            WeatherTypesEnum.Overcast,
            WeatherTypesEnum.Clearing,
            WeatherTypesEnum.Rain,
            WeatherTypesEnum.Thunder,
            WeatherTypesEnum.Clearing,
            WeatherTypesEnum.Overcast,
        };


        internal Forecast(int interval)
        {
            timeInterval = interval;
            TextList = new List<Text>();
            TexturesList = new List<Texture>();
            currWeatherIndex = 0;
            CreateForecast(); 
            ForecastRunning = true;
        }

        internal void Process()
        {
            GameFiber.StartNew(delegate
            {
                NativeFunction.Natives.SET_WEATHER_TYPE_NOW_PERSIST(WeatherList[currWeatherIndex].WeatherName);
                while (true && ForecastRunning)
                {
                    GameFiber.Yield();
                    DateTime currTime = GameTimeImproved.GetTime();
                    var e = (WeatherTypesEnum) NativeFunction.Natives.GET_PREV_WEATHER_TYPE_HASH_NAME<int>();
                    if (Enum.IsDefined(typeof(WeatherTypesEnum), e) && (e != WeatherList[currWeatherIndex].WeatherType))
                    {
                        NativeFunction.Natives.SET_WEATHER_TYPE_NOW_PERSIST(WeatherList[currWeatherIndex].WeatherName);
                    }
                    if (currTime > WeatherList[currWeatherIndex + 1].WeatherTime)
                    {
                        TransitionWeather(WeatherList[currWeatherIndex].WeatherName,
                            WeatherList[currWeatherIndex + 1].WeatherName);
                        Game.LogTrivial(
                            $"Starting the transition from {WeatherList[currWeatherIndex].WeatherName} --> {WeatherList[currWeatherIndex + 1].WeatherName}");
                        currWeatherIndex++;
                        
                        if (currWeatherIndex >= WeatherList.Count - 1)
                        {
                            CreateForecast(WeatherList[currWeatherIndex].WeatherName);
                            currWeatherIndex = 0;
                        }
                    }
                }
            });
        }

        internal void TransitionWeather(string CurrentWeather, string NextWeather)
        {
            float percentChanged = 0.00f;
            while (true)
            {
                GameFiber.Yield();
                percentChanged += 0.001f;
                NativeFunction.Natives.x578C752848ECFA0C(Game.GetHashKey(CurrentWeather),
                    Game.GetHashKey(NextWeather), percentChanged);
                if (percentChanged >= 0.99)
                {
                    NativeFunction.Natives.SET_WEATHER_TYPE_NOW_PERSIST(NextWeather);
                    Game.LogTrivial($"Transitioned --> {NextWeather}");
                    Events.InvokeOnWeatherChanged(WeatherList[currWeatherIndex], WeatherList[currWeatherIndex + 1]);
                    break;
                }
            }
        }

        internal void CreateForecast(string startingWeatherName = "")
        {
            List<Weather> weatherList = new List<Weather>();
            DateTime time = GameTimeImproved.GetTime();
            int index = random.Next(0, 5);
            Game.LogTrivial(startingWeatherName);
            if (startingWeatherName.Length == 0)
            {
                Weather weather = Weathers.WeatherData[stages[index]].Clone();
                weather.WeatherTime = time;
                weatherList.Add(weather);
                index++;
            }
            else
            {
                Enum.TryParse(startingWeatherName, true, out WeatherTypesEnum type);
                Weather weather = Weathers.WeatherData[type].Clone();
                weather.WeatherTime = time;
                weatherList.Add(weather);
            }

            for (int i = 1; i < 5; i++)
            {
                if (index >= stages.Length)
                {
                    index = 0;
                }

                Weather weather = Weathers.WeatherData[stages[index]].Clone();
                time = UpdateTime(weather, time);
                weatherList.Add(weather);
                index++;
            }
            WeatherList = weatherList;
            GenerateForecastTextures();
            Game.LogTrivial($"New forecast generated! --> {String.Join(", ", weatherList.Select(w => w.WeatherName))}");
        }

        internal static DateTime UpdateTime(Weather weather, DateTime time)
        {
            DateTime predictedTime = time += TimeSpan.FromHours(Settings.TimeInterval);
            weather.WeatherTime = predictedTime;
            return predictedTime;
        }

        internal void GenerateForecastTextures()
        {
            TextList.Clear();
            TexturesList.Clear();
            for (var index = 0; index < WeatherList.Count; index++)
            {
                var weather = WeatherList[index];
                string f =
                    $"{weather.Temperature.ToString()}° {(Weathers.usingMuricaUnits ? "F" : "C")}\n{weather.WeatherTime.ToString("t")}";
                Text weatherText = new Text(f, 40, Color.White);
                TextList.Add(weatherText);
                TexturesList.Add(weather.GetTexture());
            }

            Game.DisplayNotification("char_ls_tourist_board", "char_ls_tourist_board", "~b~ Weather Notification",
                "Dynamic Weather", "Los Santos Transit has updated its forecast.");
        }

        internal void DrawForecast(Rage.Graphics g)
        {
            TextureHelper.DrawTexture(g, TexturesList);
            TextureHelper.DrawText(g, TextList);

            SizeF size = Game.Resolution;
            TextureHelper.DrawText(g, new Text(GameTimeImproved.GetTimeString(), 37, Color.White), size.Width / 2, 10);
        }


        internal void DrawCurrentWeather(Rage.Graphics g)
        {
            SizeF size = Game.Resolution;
            String f =
                $"{WeatherList[currWeatherIndex].Temperature.ToString()}° {(Weathers.usingMuricaUnits ? "F" : "C")}\n{GameTimeImproved.GetTimeString()}";
            TextureHelper.DrawText(g, new Text(f, 37, Color.White), size.Width - 200, size.Height / 5);
            TextureHelper.DrawTexture(g, WeatherList[currWeatherIndex].GetTexture(), size.Width - 200, size.Height / 10, 96, 96);
        }

        internal void PauseForecast()
        {
            ForecastRunning = false;
            Game.DisplayNotification("char_ls_tourist_board", "char_ls_tourist_board", "~b~ Weather Notification",
                "Dynamic Weather", "All weather systems have went down. The forecast and current weather display are now inactive.");
            EntryPoint.Stop();
        }
        
        internal void ResumeForecast()
        {
            ForecastRunning = true;
            Game.DisplayNotification("char_ls_tourist_board", "char_ls_tourist_board", "~b~ Weather Notification",
                "Dynamic Weather", "All weather systems are back up. The forecast and current weather display are now active.");
            RegenerateForecast();
            Process();
            EntryPoint.Start();
        }

        internal void RegenerateForecast()
        {
            var e = (WeatherTypesEnum) NativeFunction.Natives.GET_PREV_WEATHER_TYPE_HASH_NAME<int>();
            if (Enum.IsDefined(typeof(WeatherTypesEnum), e))
            {
                CreateForecast(e.ToString().ToUpper());
            }
            else
            {
                Game.LogTrivial("Something special was going on.");
                CreateForecast();
            }
        }
    }
}
