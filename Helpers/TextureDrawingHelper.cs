using Rage;
using System.Collections.Generic;
using System.Drawing;

namespace DynamicWeather.Helpers
{
    public class TextureDrawingHelper
    {
        public static void Draw(Rage.Graphics g, List<Texture> textures)
        {
            SizeF size = Game.Resolution;
            for (int index = 0; index < textures.Count; index++)
            {
                Texture texture = textures[index];
                g.DrawTexture(texture, size.Width / (textures.Count + 1) * (index + 1), size.Height / 2, 96, 96);
            }
        }

        public static void Draw(Rage.Graphics g, List<Texture> textures, float x, float y, float width, float height)
        {
            SizeF size = Game.Resolution;
            for (int index = 0; index < textures.Count; index++)
            {
                Texture texture = textures[index];
                g.DrawTexture(texture, x, y, width, height);
            }
        }

        public static void Draw(Rage.Graphics g, Texture texture, float x, float y, float width, float height)
        {
            if (texture != null)
                g.DrawTexture(texture, x, y, width, height);
            else
                return;
        }
    }
}
