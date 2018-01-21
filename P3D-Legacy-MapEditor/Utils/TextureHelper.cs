using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.Legacy.MapEditor.Data.Models
{
    public static class TextureHelper
    {
        public static KeyValuePair<Texture2D, bool> CropTexture(Texture2D texture, Rectangle rectangle)
        {
            var pixels = new Color[rectangle.Width * rectangle.Height];
            texture.GetData(0, rectangle, pixels, 0, rectangle.Width * rectangle.Height);

            var hasTransparent = false;
            for (var i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].A < 255)
                {
                    hasTransparent = true;
                    break;
                }
            }

            var newTex = new Texture2D(texture.GraphicsDevice, rectangle.Width, rectangle.Height);
            newTex.SetData(pixels);

            return new KeyValuePair<Texture2D, bool>(newTex, hasTransparent);
        }
    }
}