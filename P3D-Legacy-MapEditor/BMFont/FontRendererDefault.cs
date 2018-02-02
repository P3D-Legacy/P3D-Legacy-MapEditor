using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.Legacy.MapEditor.BMFont
{
    public class FontRendererDefault : IFontRenderer
    {
        private List<FontFile> Fonts { get; } = new List<FontFile>();

        public FontRendererDefault(FontFile fontFile)
        {
            Fonts.Add(fontFile);
        }
        public FontRendererDefault(IEnumerable<FontFile> fontFiles)
        {
            foreach (var fontFile in fontFiles)
                Fonts.Add(fontFile);
            Fonts = Fonts.OrderByDescending(font => font.Size).ToList();
        }

        private static Texture2D _pointTexture;
        private static void DrawRectangle(SpriteBatch spriteBatch, Rectangle rectangle, Color color, int lineWidth)
        {
            if (_pointTexture == null)
            {
                _pointTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _pointTexture.SetData(new [] { Color.White });
            }

            spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
            spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width + lineWidth, lineWidth), color);
            spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X + rectangle.Width, rectangle.Y, lineWidth, rectangle.Height + lineWidth), color);
            spriteBatch.Draw(_pointTexture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width + lineWidth, lineWidth), color);
        }

        private FontFile GetMinFont(string text, Rectangle borders, out Vector2 size)
        {
            var maxFont = Fonts[0];
            var maxSize = maxFont.MeasureText(text);
            var maxScale = Math.Min(borders.Width / maxSize.X, borders.Height / maxSize.Y);

            var font = Fonts.OrderBy(f => Math.Abs(maxScale - (float) f.Size / (float) maxFont.Size)).First();
            var scale = (float) font.Size / (float) maxFont.Size;
            size = maxSize * scale;
            return font;

            var minFont = Fonts.Count == 1
                ? Fonts.Select(f => new { Font = f, Size = f.MeasureText(text) }).First()
                : Fonts.Select(f => new { Font = f, Size = f.MeasureText(text) }).OrderBy(tuple => Math.Abs(1.0f - Math.Min(borders.Width / tuple.Size.X, borders.Height / tuple.Size.Y))).First();

            //var minFont = Fonts.Count == 1
            //    ? Fonts.Select(f => new {Font = f, Size = f.MeasureText(text)}).First()
            //    : Fonts.Select(f => new {Font = f, Size = f.MeasureText(text)}).OrderBy(tuple => Math.Abs(1.0f - Math.Min(borders.Width / tuple.Size.X, borders.Height / tuple.Size.Y))).First();
            size = minFont.Size;

            return minFont.Font;
        }

        public void DrawText(SpriteBatch spriteBatch, string text, Rectangle borders, Color color, bool centerHeight = true)
        {
            var font = GetMinFont(text, borders, out var size);

            var dx = borders.X;
            var dy = centerHeight ? borders.Center.Y - (int) (size.Y * 0.5f) : borders.Y;

            foreach (var c in text)
            {
                if (font.GlyphMap.TryGetValue(c, out var fc))
                {
                    var pos = new Vector2(dx + fc.XOffset, dy + fc.YOffset);
                    var src = new Rectangle(fc.X, fc.Y, fc.Width, fc.Height);

                    spriteBatch.Draw(font.Textures[fc.Page], pos, src, color);

                    dx += fc.XAdvance;
                }

                if (c == '\n')
                {
                    dx = borders.X;
                    dy += font.LineHeight;
                }
            }
        }
        public void DrawText(SpriteBatch spriteBatch, string text, Vector2 position, Color color)
        {
            var font = Fonts.First();

            var dx = position.X;
            var dy = position.Y;

            foreach (var c in text)
            {
                if (font.GlyphMap.TryGetValue(c, out var fc))
                {
                    var pos = new Vector2(dx + fc.XOffset, dy + fc.YOffset);
                    var src = new Rectangle(fc.X, fc.Y, fc.Width, fc.Height);

                    spriteBatch.Draw(font.Textures[fc.Page], pos, src, color);

                    dx += fc.XAdvance;
                }

                if (c == '\n')
                {
                    dx = position.X;
                    dy += font.LineHeight;
                }
            }
        }

        public void DrawTextStretched(SpriteBatch spriteBatch, string text, Rectangle borders, Color color)
        {
            /*
            var size = MeasureText(text);
            var scale = Math.Min(borders.Width / size.X, borders.Height / size.Y);

            var dx = borders.X;
            //var dy = borders.Y;
            var dy = borders.Center.Y - (int)(size.Y * scale * 0.5f);

            foreach (var c in text)
            {
                XmlFontChar fc;
                if (GlyphMap.TryGetValue(c, out fc))
                {
                    var pos = new Vector2(dx + (int)(fc.XOffset * scale), dy + (int)(fc.YOffset * scale));
                    var src = new Rectangle(fc.X, fc.Y, fc.Width, fc.Height);

                    spriteBatch.Draw(Textures[fc.Page], pos, src, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

                    dx += (int)(fc.XAdvance * scale);
                }
            }
            */
        }

        public void DrawTextCentered(SpriteBatch spriteBatch, string text, Rectangle borders, Color color)
        {
            var font = GetMinFont(text, borders, out var size);

            var dx = borders.Center.X - (int) (size.X * 0.5f);
            var dy = borders.Center.Y - (int) (size.Y * 0.5f);

            foreach (var c in text)
            {
                if (font.GlyphMap.TryGetValue(c, out var fc))
                {
                    var pos = new Vector2(dx + fc.XOffset, dy + fc.YOffset);
                    var src = new Rectangle(fc.X, fc.Y, fc.Width, fc.Height);

                    spriteBatch.Draw(font.Textures[fc.Page], pos, src, color);

                    dx += fc.XAdvance;
                }

                if (c == '\n')
                {
                    dx = borders.Center.X - (int) (size.X * 0.5f);
                    dy += font.LineHeight;
                }
            }
        }
        public void DrawTextCenteredStretched(SpriteBatch spriteBatch, string text, Rectangle borders, Color color)
        {
            /*
            var size = MeasureText(text);
            var scale = Math.Min(borders.Width / size.X, borders.Height / size.Y);

            var dx = borders.Center.X - (int)(size.X * scale * 0.5f) - (int)(2 * text.Length * 0.5f);
            var dy = borders.Center.Y - (int)(size.Y * scale * 0.5f);

            foreach (var c in text)
            {
                XmlFontChar fc;
                if (GlyphMap.TryGetValue(c, out fc))
                {
                    var pos = new Vector2(dx + (int)(fc.XOffset * scale), dy + (int)(fc.YOffset * scale));
                    var src = new Rectangle(fc.X, fc.Y, fc.Width, fc.Height);

                    spriteBatch.Draw(Textures[fc.Page], pos, src, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

                    dx += (int) (fc.XAdvance * scale);
                }
            }
            */
        }

        public void Dispose()
        {
            foreach (var font in Fonts)
                font.Dispose();
        }
    }
}