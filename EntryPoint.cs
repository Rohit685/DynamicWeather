using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Rage;
using Rage.Exceptions;

[assembly: Rage.Attributes.Plugin("DynamicWeather", Author = "Roheat",PrefersSingleInstance = true,Description = "More variety in the sky")]
namespace DynamicWeather
{
    internal static class EntryPoint
    {
        internal static List<Texture> textures = new List<Texture>();
        
        internal enum WeatherType
        {
            Blizzard = 669657108,
            Clear = 916995460,
            Clearing = 1840358669,
            Clouds = 821931868,
            ExtraSunny = -1750463879,
            Foggy = -1368164796,
            Neutral = -1530260698,
            Overcast = -1148613331,
            Rain = 1420204096,
            Snow = -273223690,
            Snowlight = 603685163,
            Thunder = -1233681761,
        }
        
        internal static bool drawing = false;
        internal static void Main()
        {
            LoadAllTextures();
            while (true)
            {
                GameFiber.Yield();
                if (Game.IsKeyDownRightNow(Keys.Tab) && !drawing) 
                {
                    Start();
                    drawing = true;
                }
                else if (!Game.IsKeyDownRightNow(Keys.Tab) && drawing)
                {
                    Stop();
                    drawing = false;
                }

            }
        }

        internal static void LoadAllTextures()
        {
            foreach (var texture in Directory.GetFiles(@"Plugins/Textures"))
            {
                textures.Add(Game.CreateTextureFromFile(texture));
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
        
        private static void Draw(Rage.Graphics g)
        {
            SizeF size = Game.Resolution;
            for (var index = 0; index < textures.Count; index++)
            {
                var texture = textures[index];
                g.DrawTexture(texture,size.Width / (textures.Count + 1) * (index + 1),size.Height / 2, 96,96);
            }
        }
        private static void FrameRender(object sender, GraphicsEventArgs e)
        {
            Draw(e.Graphics);
           
        }
    }
}   