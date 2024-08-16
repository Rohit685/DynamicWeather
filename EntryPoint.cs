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
            currentForecast = new Forecast(Settings.TimeInterval);
            currentForecast.Process();
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
            if (drawing)
            {
                currentForecast.DrawForecast(e.Graphics);
            }

            if (Settings.EnableAlwaysOnUI)
            {
                currentForecast.DrawCurrentWeather(e.Graphics);
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
            currentForecast.PauseForecast();
        }

        [ConsoleCommand]
        private static void ResumeForecast()
        {
            currentForecast.ResumeForecast();
        }

        [ConsoleCommand]
        private static void RegenerateForecast()
        {
            currentForecast.RegenerateForecast();
        }
}
}   