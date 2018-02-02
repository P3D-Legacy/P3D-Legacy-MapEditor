using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.Legacy.MapEditor.BMFont
{
    public class FontRendererScalable : IFontRenderer
    {
        private FontFile FontFile { get; }
        private XmlFontFile XmlFontData { get; }
        private Texture2D[] Textures { get; }
        private Dictionary<char, XmlFontChar> GlyphMap { get; } = new Dictionary<char, XmlFontChar>();
        private int LineHeight => XmlFontData.Common.LineHeight;

        public FontRendererScalable(FontFile fontFile)
        {
            FontFile = fontFile;
            XmlFontData = fontFile.XmlFontData;
            Textures = fontFile.Textures;

            foreach (var glyph in XmlFontData.Chars)
                GlyphMap.Add((char) glyph.ID, glyph);
        }
        private float ScaleToMin(float value)
        {
            //return (float) Math.Round(value * FontScale, MidpointRounding.ToEven) / FontScale;

            var val = (int) value;
            var ttt = value - val;

            if (ttt > 0.00 && ttt < 0.26)
                ttt = 0.25f;
            if (ttt > 0.26 && ttt < 0.51)
                ttt = 0.25f;
            if (ttt > 0.51 && ttt < 0.76)
                ttt = 0.50f;
            if (ttt > 0.76 && ttt < 1.00)
                ttt = 0.75f;

            return ttt + val;
        }

        private static Texture2D _pointTexture;
        private static void DrawRectangle(SpriteBatch spriteBatch, Rectangle rectangle, Color color, int lineWidth)
        {
            if (_pointTexture == null)
            {
                _pointTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _pointTexture.SetData(new Color[] { Color.White });
            }

            spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
            spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width + lineWidth, lineWidth), color);
            spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X + rectangle.Width, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
            spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width + lineWidth, lineWidth), color);
        }

        public Vector2 MeasureText(string text)
        {
            var width = 0;
            var height = LineHeight;
            foreach (var c in text)
            {
                if (GlyphMap.TryGetValue(c, out var fc))
                    width += fc.XAdvance;

                if (c == '\n')
                    height += LineHeight;
            }

            return new Vector2(width, height);
        }

        public void DrawText(SpriteBatch spriteBatch, string text, Rectangle borders, Color color, bool centerHeight = true)
        {
            var size = MeasureText(text);
            var min = Math.Min(borders.Width / size.X, borders.Height / size.Y);
            var scale = ScaleToMin(min);

            //if (scale < 1f)
            //    scale = 1f;

            var dx = borders.X;
            var dy = centerHeight ? borders.Center.Y - (int) (size.Y * scale * 0.5f) : borders.Y;

            foreach (var c in text)
            {
                if (GlyphMap.TryGetValue(c, out var fc))
                {
                    var pos = new Vector2(dx + (int) (fc.XOffset * scale), dy + (int) (fc.YOffset * scale));
                    var src = new Rectangle(fc.X, fc.Y, fc.Width, fc.Height);

                    spriteBatch.Draw(Textures[fc.Page], pos, src, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

                    dx += (int) (fc.XAdvance * scale);
                }

                if (c == '\n')
                {
                    dx = borders.X;
                    dy += (int) (LineHeight * scale);
                }
            }
        }
        public void DrawText(SpriteBatch spriteBatch, string text, Vector2 position, Color color)
        {
            var dx = position.X;
            var dy = position.Y;

            foreach (var c in text)
            {
                if (GlyphMap.TryGetValue(c, out var fc))
                {
                    var pos = new Vector2(dx + fc.XOffset, dy + fc.YOffset);
                    var src = new Rectangle(fc.X, fc.Y, fc.Width, fc.Height);

                    spriteBatch.Draw(Textures[fc.Page], pos, src, color);

                    dx += fc.XAdvance;
                }

                if (c == '\n')
                {
                    dx = position.X;
                    dy +=  LineHeight;
                }
            }
        }

        public void DrawTextStretched(SpriteBatch spriteBatch, string text, Rectangle borders, Color color)
        {
            var size = MeasureText(text);
            var scale = Math.Min(borders.Width / size.X, borders.Height / size.Y);

            var dx = borders.X;
            //var dy = borders.Y;
            var dy = borders.Center.Y - (int)(size.Y * scale * 0.5f);

            foreach (var c in text)
            {
                if (GlyphMap.TryGetValue(c, out var fc))
                {
                    var pos = new Vector2(dx + (int)(fc.XOffset * scale), dy + (int)(fc.YOffset * scale));
                    var src = new Rectangle(fc.X, fc.Y, fc.Width, fc.Height);

                    spriteBatch.Draw(Textures[fc.Page], pos, src, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

                    dx += (int)(fc.XAdvance * scale);
                }
            }
        }

        public void DrawTextCentered(SpriteBatch spriteBatch, string text, Rectangle borders, Color color)
        {
            var size = MeasureText(text);
            var min = Math.Min(borders.Width / size.X, borders.Height / size.Y);
            var scale = ScaleToMin(min);

            var dx = borders.Center.X - (int) (size.X * scale * 0.5f);
            var dy = borders.Center.Y - (int) (size.Y * scale * 0.5f);

            foreach (var c in text)
            {
                if (GlyphMap.TryGetValue(c, out var fc))
                {
                    var pos = new Vector2(dx + (int) (fc.XOffset * scale), dy + (int) (fc.YOffset * scale));
                    var src = new Rectangle(fc.X, fc.Y, fc.Width, fc.Height);

                    spriteBatch.Draw(Textures[fc.Page], pos, src, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

                    dx += (int) (fc.XAdvance * scale);
                }

                if (c == '\n')
                {
                    dx = borders.Center.X - (int) (size.X * scale * 0.5f);
                    dy += (int) (LineHeight * scale);
                }
            }

            //var sizeS = size * scale;
            //DrawRectangle(spriteBatch, borders, Color.Red, 2);
            //DrawRectangle(spriteBatch, new Rectangle(borders.Center.X - (int)(size.X * 0.5), borders.Center.Y -(int)(size.Y * 0.5), (int) (size.X), (int) (size.Y)), Color.Yellow, 2);
            //DrawRectangle(spriteBatch, new Rectangle(borders.Center.X - (int)(sizeS.X * 0.5), borders.Center.Y - (int)(sizeS.Y * 0.5), (int)(sizeS.X), (int)(sizeS.Y)), Color.Blue, 2);
        }
        public void DrawTextCenteredStretched(SpriteBatch spriteBatch, string text, Rectangle borders, Color color)
        {
            var size = MeasureText(text);
            var scale = Math.Min(borders.Width / size.X, borders.Height / size.Y);

            var dx = borders.Center.X - (int)(size.X * scale * 0.5f) - (int)(2 * text.Length * 0.5f);
            var dy = borders.Center.Y - (int)(size.Y * scale * 0.5f);

            foreach (var c in text)
            {
                if (GlyphMap.TryGetValue(c, out var fc))
                {
                    var pos = new Vector2(dx + (int)(fc.XOffset * scale), dy + (int)(fc.YOffset * scale));
                    var src = new Rectangle(fc.X, fc.Y, fc.Width, fc.Height);

                    spriteBatch.Draw(Textures[fc.Page], pos, src, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

                    dx += (int)(fc.XAdvance * scale);
                }
            }
        }

        public void Dispose()
        {
            GlyphMap?.Clear();

            //if(Textures != null)
            //    foreach (var texture in Textures)
            //        texture.Dispose();
        }
    }
}