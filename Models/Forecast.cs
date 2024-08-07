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
        internal List<Weather> WeatherList = new List<Weather>();
        private int currWeatherIndex;
        internal Weather CurrentWeather => WeatherList[currWeatherIndex];
        internal Weather NextWeather => WeatherList[currWeatherIndex + 1] != null ? WeatherList[currWeatherIndex + 1] : WeatherList[0];
        private List<Text> TextList;
        private List<Texture> TexturesList;
        private int timeInterval;

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
            for (var index = 0; index < WeatherList.Count; index++)
            {
                var weather = WeatherList[index];
                Text weatherText = new Text(weather.Temperature.ToString(), 20, Color.White);
                TextList.Add(weatherText);
                TexturesList.Add(weather.Texture);
            }
        }

        internal void Process()
        {
        }

        internal void TransitionWeather()
        {
            float percentChanged = 0.00f;
            while (true)
            {
                GameFiber.Yield();
                percentChanged += 0.001f;
                NativeFunction.Natives.x578C752848ECFA0C(Game.GetHashKey(CurrentWeather.WeatherName), 
                    Game.GetHashKey(NextWeather.WeatherName), percentChanged);
                if (percentChanged >= 0.99)
                {
                    NativeFunction.Natives.SET_WEATHER_TYPE_NOW_PERSIST(NextWeather.WeatherName);
                    currWeatherIndex++;
                    break;
                }
            }
        }

        internal static List<Weather> CreateForecast()
        {
            List<Weather> weatherList = new List<Weather>();
            Random random = new Random(DateTime.Today.Millisecond);
            int index = random.Next(stages.Length);
            weatherList.Add(Weathers.WeatherData[stages[index]]);
            for (int i = 1; i < 5; i++)
            {
                if (random.Next(101) > 50)
                {
                    index++;
                    weatherList.Add(Weathers.WeatherData[stages.GetNext(index)]);
                }
                else
                {
                    weatherList.Add(Weathers.WeatherData[stages.GetNext(index)]);
                }
            }
            return weatherList;
        }

        internal void DrawForecast(Rage.Graphics g)
        {
            TextureHelper.DrawTexture(g, TexturesList);
            TextureHelper.DrawText(g, TextList);
        }
    }
}
