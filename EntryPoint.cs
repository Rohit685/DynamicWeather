using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DynamicWeather.Enums;
using DynamicWeather.Helpers;
using DynamicWeather.Models;
using Rage;
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
            currentForecast = new Forecast(Settings.TimeInterval);
            GameFiber.StartNew(GameTimeImproved.Process);
            GameFiber.StartNew(currentForecast.Process);
            while (true)
            {
                GameFiber.Yield();
                if (IsKeyDownRightNow() && !drawing) 
                {
                    Start();
                    drawing = true;
                }
                else if (!IsKeyDownRightNow() && drawing)
                {
                    Stop();
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
            currentForecast.DrawForecast(e.Graphics);
        }

        private static bool IsKeyDownRightNow()
        {
            return Settings.GlobalModifierKey.Equals(Keys.None)
                ? Game.IsKeyDownRightNow(Settings.ShowForecastKey)
                : (Game.IsKeyDownRightNow(Settings.GlobalModifierKey) &&
                   Game.IsKeyDownRightNow(Settings.ShowForecastKey));
        }
    }
}   