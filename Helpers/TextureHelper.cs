using Rage;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace DynamicWeather.Helpers
{
    internal class TextureHelper
    {
        internal static List<Texture> textures = new List<Texture>();
        internal static void Draw(Rage.Graphics g, List<Texture> textures)
        {
            SizeF size = Game.Resolution;
            for (int index = 0; index < textures.Count; index++)
            {
                Texture texture = textures[index];
                g.DrawTexture(texture, size.Width / (textures.Count + 1) * (index + 1), size.Height / 2, 96, 96);
            }
        }

        internal static void Draw(Rage.Graphics g, List<Texture> textures, float x, float y, float width, float height)
        {
            SizeF size = Game.Resolution;
            for (int index = 0; index < textures.Count; index++)
            {
                Texture texture = textures[index];
                g.DrawTexture(texture, x, y, width, height);
            }
        }

        internal static void Draw(Rage.Graphics g, Texture texture, float x, float y, float width, float height)
        {
            if (texture != null)
                g.DrawTexture(texture, x, y, width, height);
            else
                return;
        }
        
        internal static void LoadAllTextures()
        {
            foreach (var texture in Directory.GetFiles(@"Plugins/Textures"))
            {
                textures.Add(Game.CreateTextureFromFile(texture));
            }
        }
    }
}
