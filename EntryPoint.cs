using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DynamicWeather.Enums;
using DynamicWeather.Helpers;
using DynamicWeather.Models;
using Rage;
using Rage.Attributes;
using Rage.Exceptions;
using Rage.Native;

[assembly: Rage.Attributes.Plugin("DynamicWeather", Author = "Roheat",PrefersSingleInstance = true,Description = "More variety in the sky")]
namespace DynamicWeather
{
    internal static class EntryPoint
    {
        internal static bool drawing = false;
        internal static Forecast currentForecast = null;
        
        
        internal static void Main()
        {
            Weathers.DeserializeAndValidateXML();
            TextureHelper.LoadAllTextures();
            Settings.ReadSettings();
            GameFiber.WaitUntil(() => !Game.IsLoading);
            GameFiber.StartNew(GameTimeImproved.Process);
            GameFiber.WaitUntil(() => GameTimeImproved.TimeInit);
            if (Settings.RealLifeWeatherSyncEnabled)
            {
                GameFiber.StartNew(() => RealLifeWeatherSync.UpdateWeather());
                GameFiber.WaitUntil(() => RealLifeWeatherSync.networkThread != null && !RealLifeWeatherSync.networkThread.IsAlive);
            }
            else
            {
                currentForecast = new Forecast(Settings.TimeInterval);
                currentForecast.Process();
            }
            Start();
            Game.AddConsoleCommands();
            while (true)
            {
                GameFiber.Yield();
                NativeFunction.Natives.DISABLE_CONTROL_ACTION(2, 243, false);
                if (IsKeyDownRightNow())
                {
                    drawing = true;
                }
                else if (!IsKeyDownRightNow())
                {
                    drawing = false;
                }

            }
        }

        internal static void Start()
        {
            Game.RawFrameRender += FrameRender;
        }

        internal static void Stop()
        {
            Game.RawFrameRender -= FrameRender;
        }

        private static void FrameRender(object sender, GraphicsEventArgs e)
        {
            if (drawing && !RealLifeWeatherSync.isRealLifeWeatherSyncRunning && currentForecast != null)
            {
                currentForecast.DrawForecast(e.Graphics);
            }

            if (Settings.EnableAlwaysOnUI && !RealLifeWeatherSync.isRealLifeWeatherSyncRunning && currentForecast != null)
            {
                currentForecast.DrawCurrentWeather(e.Graphics);
            }

            if (RealLifeWeatherSync.isRealLifeWeatherSyncRunning && RealLifeWeatherSync.RealLifeWeather != null && Settings.EnableAlwaysOnUI)
            {
                RealLifeWeatherSync.RealLifeWeather.Draw(e.Graphics);
            }
        }

        private static bool IsKeyDownRightNow()
        {
            return Settings.GlobalModifierKey.Equals(Keys.None)
                ? Game.IsKeyDownRightNow(Settings.ShowForecastKey)
                : (Game.IsKeyDownRightNow(Settings.GlobalModifierKey) &&
                   Game.IsKeyDownRightNow(Settings.ShowForecastKey));
        }

        internal static void OnUnload(bool Exit)
        {
            Stop();
        }

        [ConsoleCommand]
        private static void PauseForecast()
        {
            if (RealLifeWeatherSync.isRealLifeWeatherSyncRunning)
            {
                Game.LogTrivial("Real life weather sync activated. Invalid command");
                return;
            }
            currentForecast.PauseForecast();
        }

        [ConsoleCommand]
        private static void ResumeForecast()
        {
            if (RealLifeWeatherSync.isRealLifeWeatherSyncRunning)
            {
                Game.LogTrivial("Real life weather sync activated. Invalid command");
                return;
            }
            currentForecast.ResumeForecast();
        }

        [ConsoleCommand]
        private static void RegenerateForecast()
        {
            if (RealLifeWeatherSync.isRealLifeWeatherSyncRunning)
            {
                Game.LogTrivial("Real life weather sync activated. Invalid command");
                return;
            }
            currentForecast.RegenerateForecast();
        }
        
        [ConsoleCommand]
        private static void RefreshWeather()
        {
            if (!RealLifeWeatherSync.isRealLifeWeatherSyncRunning)
            {
                Game.LogTrivial("Real life weather sync deactivated. Invalid command");
                return;
            }
            RealLifeWeatherSync.UpdateWeather();
        }
        
        [ConsoleCommand]
        private static void SwitchToIRLWeather()
        {
            RealLifeWeatherSync.UpdateWeather();
            if (RealLifeWeatherSync.isRealLifeWeatherSyncRunning)
            {
                currentForecast.PauseForecast();
                currentForecast = null;
            }
        }
        
        [ConsoleCommand]
        private static void SwitchToForecast()
        {
            EntryPoint.currentForecast = new Forecast(Settings.TimeInterval);
            EntryPoint.currentForecast.Process();
            RealLifeWeatherSync.isRealLifeWeatherSyncRunning = false;
            RealLifeWeatherSync.RealLifeWeather = null;
        }

        [ConsoleCommand]
        private static void ReloadSettings()
        {
            Settings.ReadSettings();
        }
    }
}   