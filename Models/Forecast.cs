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
            (int, int) lastTransitionTime = GetTime();

            while (true)
            {
                GameFiber.Yield();
                (int, int) currTime = GetTime();
                (int, int) elapsedTime = GetTimeElapsed(lastTransitionTime.Item1, lastTransitionTime.Item2,
                    currTime.Item1, currTime.Item2);

                if (elapsedTime.Item1 >= (timeInterval - 0.5))
                {
                    TransitionWeather(WeatherList[currWeatherIndex].WeatherName, WeatherList[currWeatherIndex + 1].WeatherName); 
                    lastTransitionTime = NativeFunction.Natives.GET_GAME_TIME<DateTime>(); 
                }

                // GameFiber.Sleep(5000);
            }

        }

        internal void TransitionWeather(string CurrentWeather, string NextWeather)
        {
            float percentChanged = 0.00f;
            while (true)
            {
                GameFiber.Yield();
                percentChanged += 1f;
                // NativeFunction.Natives.x578C752848ECFA0C(Game.GetHashKey(CurrentWeather), 
                //     Game.GetHashKey(NextWeather), percentChanged);
                if (percentChanged >= 0.99)
                {
                    NativeFunction.Natives.SET_WEATHER_TYPE_NOW_PERSIST(NextWeather);
                    currWeatherIndex++;
                    if (currWeatherIndex >= WeatherList.Count)
                    {
                        currWeatherIndex = 0;
                        CreateForecast();
                    }
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
            TextureHelper.DrawText(g, new Text(GetTimeString(), 40, Color.White), size.Width / 2, 1);
        }

        internal (int, int) GetTimeElapsed(int startHour, int startMinute, int endHour, int endMinute)
        {
            // Convert start and end times to minutes
            int startTotalMinutes = startHour * 60 + startMinute;
            int endTotalMinutes = endHour * 60 + endMinute;

            // Calculate the difference in minutes
            int elapsedMinutes = endTotalMinutes - startTotalMinutes;

            // If the elapsed time is negative, adjust for the next day
            if (elapsedMinutes < 0)
            {
                elapsedMinutes += 24 * 60; // Add 24 hours worth of minutes
            }

            // Convert elapsed time back to hours and minutes
            int hoursElapsed = elapsedMinutes / 60;
            int minutesElapsed = elapsedMinutes % 60;

            return (hoursElapsed, minutesElapsed);
        }

        internal (int, int) GetTime()
        {
            return (NativeFunction.Natives.GET_CLOCK_HOURS<int>(), NativeFunction.Natives.GET_CLOCK_MINUTES<int>());
        }
        
        internal string GetTimeString()
        {
            int lastTransitionHour = NativeFunction.Natives.GET_CLOCK_HOURS<int>();
            int lastTransitionMinute = NativeFunction.Natives.GET_CLOCK_MINUTES<int>();
            return new DateTime(1, 1, 1, lastTransitionHour, lastTransitionMinute, 0).ToString("t");
        }
    }
}
