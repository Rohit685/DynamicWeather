using System.Collections.Generic;
using DynamicWeather.Models;

namespace DynamicWeather.API;

public static class Functions
{
    public static void PauseForecast()
    {
        EntryPoint.currentForecast.PauseForecast();
    }
    
    public static void ResumeForecast()
    {
        EntryPoint.currentForecast.ResumeForecast();
    }

    public static List<Weather> GetCurrentForecast(out int currentWeatherIndex)
    {
        currentWeatherIndex = EntryPoint.currentForecast.currWeatherIndex;
        return EntryPoint.currentForecast.WeatherList;
    }
}