using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using Rage;
using DynamicWeather.Enums;
using DynamicWeather.Models;
using Rage.Native;

namespace DynamicWeather.Helpers;

internal static class RealLifeWeatherSync
{
    private static readonly string userAgent = Assembly.GetExecutingAssembly().GetName().Name + "/" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
    internal static Thread networkThread = null;
    private static bool responseReceived = false;
    internal static bool isRealLifeWeatherSyncRunning = false;
    internal static Weather RealLifeWeather = null;

    private static Dictionary<int, WeatherTypesEnum> codeToEnum = new Dictionary<int, WeatherTypesEnum>()
    {
        { 1000, WeatherTypesEnum.Clear },
        { 1003, WeatherTypesEnum.Clouds },
        { 1006, WeatherTypesEnum.Neutral },
        { 1009, WeatherTypesEnum.Overcast },
        { 1030, WeatherTypesEnum.Foggy },
        { 1063, WeatherTypesEnum.Clearing },
        { 1066, WeatherTypesEnum.Snowlight },
        { 1069, WeatherTypesEnum.Snowlight },
        { 1072, WeatherTypesEnum.Snowlight },
        { 1087, WeatherTypesEnum.Thunder },
        { 1114, WeatherTypesEnum.Snow },
        { 1117, WeatherTypesEnum.Blizzard },
        { 1135, WeatherTypesEnum.Foggy },
        { 1147, WeatherTypesEnum.Foggy },
        { 1150, WeatherTypesEnum.Clearing },
        { 1153, WeatherTypesEnum.Clearing },
        { 1168, WeatherTypesEnum.Clearing },
        { 1171, WeatherTypesEnum.Clearing },
        { 1180, WeatherTypesEnum.Clearing },
        { 1183, WeatherTypesEnum.Clearing },
        { 1186, WeatherTypesEnum.Rain },
        { 1189, WeatherTypesEnum.Rain },
        { 1192, WeatherTypesEnum.Rain },
        { 1195, WeatherTypesEnum.Rain },
        { 1198, WeatherTypesEnum.Rain },
        { 1201, WeatherTypesEnum.Rain },
        { 1204, WeatherTypesEnum.Snowlight },
        { 1207, WeatherTypesEnum.Snowlight },
        { 1213, WeatherTypesEnum.Snowlight },
        { 1216, WeatherTypesEnum.Snow },
        { 1219, WeatherTypesEnum.Snow },
        { 1222, WeatherTypesEnum.Snow },
        { 1225, WeatherTypesEnum.Snow },
        { 1237, WeatherTypesEnum.Snow },
        { 1240, WeatherTypesEnum.Clearing },
        { 1243, WeatherTypesEnum.Rain },
        { 1246, WeatherTypesEnum.Rain },
        { 1249, WeatherTypesEnum.Snowlight },
        { 1252, WeatherTypesEnum.Snowlight },
        { 1255, WeatherTypesEnum.Snowlight },
        { 1258, WeatherTypesEnum.Snow },
        { 1261, WeatherTypesEnum.Snow },
        { 1264, WeatherTypesEnum.Snow },
        { 1273, WeatherTypesEnum.Thunder },
        { 1276, WeatherTypesEnum.Thunder },
        { 1279, WeatherTypesEnum.Thunder },
        { 1282, WeatherTypesEnum.Thunder },
    };   

    internal static void UpdateWeather()
    {
        networkThread = new Thread(GetUpdatedWeather);
        networkThread.Start();
        
        while(!responseReceived && networkThread.IsAlive)
        {
            GameFiber.Sleep(1000);
        }

        if (!responseReceived)
        {
            Game.LogTrivial("Unable to retrieve real life weather. Switching to forecast.");
            Game.DisplayNotification("new_editor", "warningtriangle", "ERROR", "~y~Dynamic Weather",
                $"Real life weather was not able to be fetched. Switching to normal forecast.");
            if (EntryPoint.currentForecast == null)
            {
                EntryPoint.currentForecast = new Forecast(Settings.TimeInterval);
                EntryPoint.currentForecast.Process();
            }
            isRealLifeWeatherSyncRunning = false;
            return;
        }
        isRealLifeWeatherSyncRunning = true;
    }

    private static void GetUpdatedWeather()
    {
        WebClient weatherChecker = new();
        weatherChecker.Headers.Add("User-Agent", userAgent);
        string response = "";
        string endpoint =
            $"https://api.weatherapi.com/v1/current.json?key={Settings.APIKey}&q={Settings.Location}&aqi=no";
        try
        {
            response = weatherChecker.DownloadString(endpoint);
        }
        catch (WebException we)
        {
            Game.LogTrivial($"Unable to get recent weather from {Settings.Location}");
        }
        catch (ThreadAbortException)
        {
        }
        catch (Exception e)
        {
            Game.LogTrivial($"{e.ToString()}");
        }
        Game.LogTrivial(response);
        int tempC = Int32.Parse(Regex.Match(response, @"temp_c..(\d{2})").Groups[1].Value);
        int tempF = Int32.Parse(Regex.Match(response, @"temp_f..(\d{2})").Groups[1].Value);
        int conditionCode = int.Parse(Regex.Match(response, @"code..(\d{4})").Groups[1].Value);
        RealLifeWeather = Weathers.WeatherData[codeToEnum[conditionCode]].Clone();
        RealLifeWeather.Temperature = Weathers.usingMuricaUnits ? tempF : tempC;
        RealLifeWeather.WeatherTime = GameTimeImproved.GetTime();
        Game.LogTrivial($"Current Temperature in {Settings.Location}: {RealLifeWeather.Temperature}");
        Game.LogTrivial($"Current condition in {Settings.Location}: {Weathers.WeatherData[codeToEnum[conditionCode]].WeatherName}");
        NativeFunction.Natives.SET_WEATHER_TYPE_NOW_PERSIST(Weathers.WeatherData[codeToEnum[conditionCode]].WeatherName);
        responseReceived = true;
    }
    
    
}

