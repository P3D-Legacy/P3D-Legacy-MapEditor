﻿using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.Legacy.MapEditor.Utils
{
    /// <summary>
    /// Based on http://jakepoz.com/jake_poznanski__background_load_xna.html 
    /// </summary>
    public class TextureLoader
    {
        public TextureLoader(GraphicsDevice graphicsDevice, bool needsBmp = false)
        {
            _graphicsDevice = graphicsDevice;
            _needsBmp = needsBmp;
            _spriteBatch = new SpriteBatch(_graphicsDevice);
        }

        public Texture2D FromFile(string path, bool preMultiplyAlpha = true)
        {
            using (Stream fileStream = File.OpenRead(path))
                return FromStream(fileStream, preMultiplyAlpha);
        }

        /// <summary>
        /// CPU bound alpha premultiply - roughly 30-40ms on X51.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="preMultiplyAlpha"></param>
        /// <returns></returns>
        public Texture2D FromStream(Stream stream, bool preMultiplyAlpha = true)
        {
            var texture = Texture2D.FromStream(_graphicsDevice, stream);
            var data = new Color[texture.Width * texture.Height];
            texture.GetData(data);
            for (var i = 0; i != data.Length; ++i)
                data[i] = Color.FromNonPremultiplied(data[i].ToVector4());
            texture.SetData(data);
            return texture;
        }

        /// <summary>
        /// Uses GPU to do premultiply calcs. Fast, however had problems.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="preMultiplyAlpha"></param>
        /// <returns></returns>
        public Texture2D FromStreamFast(Stream stream, bool preMultiplyAlpha = true)
        {
            Texture2D texture;

            if (_needsBmp)
            {
                // Load image using GDI because Texture2D.FromStream doesn't support BMP
                using (var image = System.Drawing.Image.FromStream(stream))
                {
                    // Now create a MemoryStream which will be passed to Texture2D after converting to PNG internally
                    using (var ms = new MemoryStream())
                    {
                        image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        ms.Seek(0, SeekOrigin.Begin);
                        texture = Texture2D.FromStream(_graphicsDevice, ms);
                    }
                }
            }
            else
            {
                texture = Texture2D.FromStream(_graphicsDevice, stream);
            }

            if (preMultiplyAlpha)
            {
                // Setup a render target to hold our final texture which will have premulitplied alpha values
                using (var renderTarget = new RenderTarget2D(_graphicsDevice, texture.Width, texture.Height))
                {
                    var viewportBackup = _graphicsDevice.Viewport;
                    _graphicsDevice.SetRenderTarget(renderTarget);
                    _graphicsDevice.Clear(Color.Black);

                    // Multiply each color by the source alpha, and write in just the color values into the final texture
                    _spriteBatch.Begin(SpriteSortMode.Immediate, BlendColorBlendState);
                    _spriteBatch.Draw(texture, texture.Bounds, Color.White);
                    _spriteBatch.End();

                    // Now copy over the alpha values from the source texture to the final one, without multiplying them
                    _spriteBatch.Begin(SpriteSortMode.Immediate, BlendAlphaBlendState);
                    _spriteBatch.Draw(texture, texture.Bounds, Color.White);
                    _spriteBatch.End();

                    // Release the GPU back to drawing to the screen
                    _graphicsDevice.SetRenderTarget(null);
                    _graphicsDevice.Viewport = viewportBackup;

                    // Store data from render target because the RenderTarget2D is volatile
                    var data = new Color[texture.Width * texture.Height];
                    renderTarget.GetData(data);

                    // Unset texture from graphic device and set modified data back to it
                    _graphicsDevice.Textures[0] = null;
                    texture.SetData(data);
                }

            }

            return texture;
        }

        private static readonly BlendState BlendColorBlendState = new BlendState
        {
            ColorDestinationBlend = Blend.Zero,
            ColorWriteChannels = ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue,
            AlphaDestinationBlend = Blend.Zero,
            AlphaSourceBlend = Blend.SourceAlpha,
            ColorSourceBlend = Blend.SourceAlpha
        };
        private static readonly BlendState BlendAlphaBlendState = new BlendState
        {
            ColorWriteChannels = ColorWriteChannels.Alpha,
            AlphaDestinationBlend = Blend.Zero,
            ColorDestinationBlend = Blend.Zero,
            AlphaSourceBlend = Blend.One,
            ColorSourceBlend = Blend.One
        };

        private readonly GraphicsDevice _graphicsDevice;
        private readonly SpriteBatch _spriteBatch;
        private readonly bool _needsBmp;
    }
}
