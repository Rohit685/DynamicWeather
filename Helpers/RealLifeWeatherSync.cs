using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading;
using Rage;
using System.Text.Json;
using DynamicWeather.Enums;
using DynamicWeather.Models;

namespace DynamicWeather.Helpers;

internal static class RealLifeWeatherSync
{
    private static readonly string userAgent = Assembly.GetExecutingAssembly().GetName().Name + "/" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
    private static Thread networkThread = null;
    private static bool responseReceived = false;
    private static string endpoint =
        $"https://api.weatherapi.com/v1/current.json?key={Settings.APIKey}&q={Settings.Location}&aqi=no";

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
            EntryPoint.currentForecast = new Forecast(Settings.TimeInterval);
            EntryPoint.currentForecast.Process();
        }
    }

    private static void GetUpdatedWeather()
    {
        WebClient weatherChecker = new();
        weatherChecker.Headers.Add("User-Agent", userAgent);
        string response = "";
        try
        {
            weatherChecker.DownloadString(endpoint);
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
        
        using (JsonDocument doc = JsonDocument.Parse(response))
        {
            JsonElement root = doc.RootElement;
            JsonElement current = root.GetProperty("current");
            int temp = Weathers.usingMuricaUnits ? current.GetProperty("temp_f").GetInt32() : current.GetProperty("temp_c").GetInt32();
            int conditionCode = current.GetProperty("condition").GetProperty("code").GetInt32();
            EntryPoint.RealLifeWeather = Weathers.WeatherData[codeToEnum[conditionCode]].Clone();
            EntryPoint.RealLifeWeather.Temperature = temp;
        }
        responseReceived = true;
    }
    
    
}

