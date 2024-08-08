using DynamicWeather.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicWeather.Enums;
using DynamicWeather.Extensions;
using DynamicWeather.Helpers;
using Rage;
using Rage.Native;

namespace DynamicWeather
{
    internal class Forecast
    {
        internal List<Weather> WeatherList;
        private int currWeatherIndex;
        internal Weather CurrentWeather => WeatherList[currWeatherIndex];
        internal Weather NextWeather => WeatherList.GetNext(currWeatherIndex);
        private List<Text> TextList;
        private List<Texture> TexturesList;
        private double timeInterval;

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
            WeatherList = new List<Weather>();
            TextList = new List<Text>();
            TexturesList = new List<Texture>();
            CreateForecast();
            currWeatherIndex = 0;
            timeInterval = interval;
        }

        internal void Process()
        {
            DateTime lastTransitionTime = GetTime();
            NativeFunction.Natives.SET_WEATHER_TYPE_NOW_PERSIST(CurrentWeather.WeatherName);
            while (true)
            {
                GameFiber.Yield();

                TimeSpan elapsedTime = GetTime() - lastTransitionTime;

                if (elapsedTime.TotalHours >= (timeInterval - 0.5))
                {
                    TransitionWeather();
                    lastTransitionTime = GetTime();
                }

            }

        }

        internal void TransitionWeather()
        {
            float percentChanged = 0.00f;
            while (true)
            {
                GameFiber.Yield();
                percentChanged += 1f;
                // NativeFunction.Natives.x578C752848ECFA0C(Game.GetHashKey(CurrentWeather.WeatherName), 
                //     Game.GetHashKey(NextWeather.WeatherName), percentChanged);
                // for testing, leave it commented out cuz its hard to test with the transitions.
                if (percentChanged >= 0.99)
                {
                    NativeFunction.Natives.SET_WEATHER_TYPE_NOW_PERSIST(NextWeather.WeatherName);
                    currWeatherIndex++;
                    if (currWeatherIndex == WeatherList.Count - 2)
                    {
                        CreateForecast();
                    }
                    if(currWeatherIndex == WeatherList.Count - 1)
                    {
                        currWeatherIndex = 0;
                    }
                    break;
                }
            }
        }

        internal void CreateForecast()
        {
            List<Weather> weatherList = new List<Weather>();
            Random random = new Random(DateTime.Today.Millisecond);
            int index = random.Next(0,8);
            if (WeatherList.Count != 0)
            {
                Enum.TryParse(CurrentWeather.WeatherName, true, out WeatherTypesEnum type);
                weatherList.Add(Weathers.WeatherData[type]);
            }
            else
            {
                weatherList.Add(Weathers.WeatherData[stages[index]]);
                index++;
            }
            for (int i = 1; i < 5; i++)
            {
                weatherList.Add(Weathers.WeatherData[stages.Get(index)]);
                index++;
                if (index >= stages.Length)
                {
                    index = 0;
                }
            }
            WeatherList = weatherList;
            TextList.Clear();
            TexturesList.Clear();
            for (index = 0; index < WeatherList.Count; index++)
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
            //DateTime
            SizeF size = Game.Resolution;
            TextureHelper.DrawText(g, new Text(GetTimeString(), 40, Color.White), size.Width / 2, 1);
            //change pos!!
        }

        internal static DateTime GetTime()
        {
            int lastTransitionHour = NativeFunction.Natives.GET_CLOCK_HOURS<int>();
            int lastTransitionMinute = NativeFunction.Natives.GET_CLOCK_MINUTES<int>();
            return new DateTime(1, 1, 1, lastTransitionHour, lastTransitionMinute, 0);
        }
        
        internal static string GetTimeString()
        {
            int lastTransitionHour = NativeFunction.Natives.GET_CLOCK_HOURS<int>();
            int lastTransitionMinute = NativeFunction.Natives.GET_CLOCK_MINUTES<int>();
            return new DateTime(1, 1, 1, lastTransitionHour, lastTransitionMinute, 0).ToString("t");
        }
    }
}
