using System.Collections.Generic;
using DynamicWeather.Helpers;
using DynamicWeather.Models;

namespace DynamicWeather.API;

public static class Functions
{
    public static void PauseWeatherSystem()
    {
        if (RealLifeWeatherSync.isRealLifeWeatherSyncRunning) return;
        EntryPoint.currentForecast.PauseForecast();
    }
    
    public static void ResumeWeatherSystem()
    {
        if (RealLifeWeatherSync.isRealLifeWeatherSyncRunning)
        {
            RealLifeWeatherSync.UpdateWeather();   
            return;
        }
        EntryPoint.currentForecast.ResumeForecast();
    }
    

    public static List<Weather> GetCurrentForecast(out int currentWeatherIndex)
    {
        currentWeatherIndex = 0;
        if (RealLifeWeatherSync.isRealLifeWeatherSyncRunning) return new List<Weather>();
        currentWeatherIndex = EntryPoint.currentForecast.currWeatherIndex;
        return EntryPoint.currentForecast.WeatherList;
    }
    
    public static bool IsRealLifeWeatherSyncRunning => RealLifeWeatherSync.isRealLifeWeatherSyncRunning;
}