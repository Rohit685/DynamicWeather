using DynamicWeather.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        internal static void CreateForecast(int interval)
        {
            
        }

        internal void DrawForecast(Rage.Graphics g)
        {
            var textList = new List<Text>();
            var texturesList = new List<Texture>();
            for (var index = 0; index < WeatherList.Count; index++)
            {
                var weather = WeatherList[index];
                Text weatherText = new Text(weather.Temperature.ToString(), 20, Color.White);
                textList.Add(weatherText);
                texturesList.Add(TextureHelper.loadedTextures[weather.WeatherName]);
            }
        }
    }
}
