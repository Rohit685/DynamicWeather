using System.Windows.Forms;
using Rage;

namespace DynamicWeather;

internal static class Settings
{
    internal static InitializationFile iniFile;
    internal static Keys GlobalModifierKey = Keys.None;
    internal static Keys ShowForecastKey = Keys.Oemtilde;
    internal static int TimeInterval = 3;
    internal static bool EnableAlwaysOnUI = true;
    internal static bool EnableMilitaryClock = false;
    internal static string APIKey = "";
    internal static string Location = "";
    internal static bool RealLifeWeatherSyncEnabled = false;
    internal static float AlwaysOnUIX = 1720;
    internal static float AlwaysOnUIY = 216;
    internal static float AlwaysOnUITextureSize = 96;
    internal static float AlwaysOnUIFontSize = 37;
    
    
    
    internal static void ReadSettings()
    {
        iniFile = new InitializationFile(@"Plugins/DynamicWeather/DynamicWeather.ini");
        iniFile.Create();
        
        
        GlobalModifierKey = iniFile.ReadEnum("General", "GlobalModifierKey", GlobalModifierKey);    
        ShowForecastKey = iniFile.ReadEnum("General", "ShowForecastKey", ShowForecastKey);
        TimeInterval = iniFile.ReadInt32("General", "TimeInterval", TimeInterval);
        EnableMilitaryClock = iniFile.ReadBoolean("General", "EnableMilitaryClock", EnableMilitaryClock);
        
        
        RealLifeWeatherSyncEnabled = iniFile.ReadBoolean("WeatherSync", "RealLifeWeatherSyncEnabled", RealLifeWeatherSyncEnabled);
        APIKey = iniFile.ReadString("WeatherSync", "APIKey", APIKey);
        Location = iniFile.ReadString("WeatherSync", "Location", Location);
        
        
        
        EnableAlwaysOnUI = iniFile.ReadBoolean("AlwaysOnUI", "EnableAlwaysOnUI", EnableAlwaysOnUI);
        AlwaysOnUIX = iniFile.ReadSingle("AlwaysOnUI", "AlwaysOnUIX", AlwaysOnUIX);
        AlwaysOnUIY = iniFile.ReadSingle("AlwaysOnUI", "AlwaysOnUIY", AlwaysOnUIY);
        AlwaysOnUITextureSize = iniFile.ReadSingle("AlwaysOnUI", "AlwaysOnUITextureSize", AlwaysOnUITextureSize);
        AlwaysOnUIFontSize = iniFile.ReadSingle("AlwaysOnUI", "AlwaysOnUIFontSize", AlwaysOnUIFontSize);
    }
 }