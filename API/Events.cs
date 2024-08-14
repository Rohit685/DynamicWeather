using DynamicWeather.Models;

namespace DynamicWeather.API;

public delegate void WeatherChangeDelegate(Weather oldWeather, Weather newWeather);


public static class Events
{
    public static event WeatherChangeDelegate OnWeatherChanged;

    internal static void InvokeOnWeatherChanged(Weather oldWeather, Weather newWeather) =>
        OnWeatherChanged?.Invoke(oldWeather, newWeather);

}