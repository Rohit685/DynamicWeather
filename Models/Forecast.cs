using DynamicWeather.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicWeather.Enums;
using DynamicWeather.Helpers;
using Rage;
using Rage.Native;

namespace DynamicWeather
{
    internal class Forecast
    {
        internal List<Weather> WeatherList = new List<Weather>();
        private int currWeatherIndex;
       private List<Text> TextList;
        private List<Texture> TexturesList;
        private double timeInterval;
        internal static Random random = new Random(DateTime.Today.Millisecond);

        private static WeatherTypesEnum[] stages = {
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
            WeatherList = CreateForecast();
            currWeatherIndex = 0;
            timeInterval = interval;
            TextList = new List<Text>();
            TexturesList = new List<Texture>();
            GenerateForecastTextures();
        }

        internal void Process()
        {
            DateTime lastTransitionTime = GameTimeImproved.GetTime();
            NativeFunction.Natives.SET_WEATHER_TYPE_NOW_PERSIST(WeatherList[currWeatherIndex].WeatherName);
            while (true)
            {
                GameFiber.Yield();
                DateTime currTime = GameTimeImproved.GetTime();
                TimeSpan elapsedTime = currTime - lastTransitionTime;
                
                if (elapsedTime.TotalMinutes >= ((timeInterval - 0.5) * 60))
                {
                    TransitionWeather(WeatherList[currWeatherIndex].WeatherName, WeatherList[currWeatherIndex + 1].WeatherName);
                    lastTransitionTime = GameTimeImproved.GetTime();
                    currWeatherIndex++;
                    if (currWeatherIndex >= WeatherList.Count - 1)
                    {
                        currWeatherIndex = 0;
                        WeatherList = CreateForecast(WeatherList[currWeatherIndex + 1].WeatherName);
                        GenerateForecastTextures();
                        Game.LogTrivial($"New forecast generated! --> {String.Join(", ", WeatherList.Select(w => w.WeatherName))}");
                    }
                }
                GameFiber.Sleep(5000);
            }

        }

        internal void TransitionWeather(string CurrentWeather, string NextWeather)
        {
            float percentChanged = 0.00f;
            while (true)
            {
                GameFiber.Yield();
                percentChanged += 0.05f;
                NativeFunction.Natives.x578C752848ECFA0C(Game.GetHashKey(CurrentWeather), 
                    Game.GetHashKey(NextWeather), percentChanged);
                if (percentChanged >= 0.99)
                {
                    NativeFunction.Natives.SET_WEATHER_TYPE_NOW_PERSIST(NextWeather);
                    Game.LogTrivial($"Transitioned --> {NextWeather}");
                    break;
                }
            }
        }

        internal static List<Weather> CreateForecast(string startingWeatherName = "")
        {
            List<Weather> weatherList = new List<Weather>();
            int index = random.Next(0,5);
            if (startingWeatherName.Length == 0)
            {
                weatherList.Add(Weathers.WeatherData[stages[index]]);
            }
            else
            {
                Enum.TryParse(startingWeatherName, true, out WeatherTypesEnum type);
                weatherList.Add(Weathers.WeatherData[type]);
            }
            for (int i = 1; i < 5; i++)
            {
                if(index >= stages.Length)
                {
                    index = 0;
                }
                weatherList.Add(Weathers.WeatherData[stages[index]]);
                index++;
            }
            return weatherList;
        }

        internal void GenerateForecastTextures()
        {
            TextList.Clear();
            TexturesList.Clear();
            for (var index = 0; index < WeatherList.Count; index++)
            {
                var weather = WeatherList[index];
                Text weatherText = new Text(weather.Temperature.ToString() + "°", 40, Color.White);
                TextList.Add(weatherText);
                TexturesList.Add(weather.Texture);
            }
        }

        internal void DrawForecast(Rage.Graphics g)
        {
            //Forecast
            TextureHelper.DrawTexture(g, TexturesList);
            TextureHelper.DrawText(g, TextList);
            
            SizeF size = Game.Resolution;
            TextureHelper.DrawText(g, new Text(GameTimeImproved.GetTimeString(), 40, Color.White), size.Width / 2, 1);
        }
    }
}
