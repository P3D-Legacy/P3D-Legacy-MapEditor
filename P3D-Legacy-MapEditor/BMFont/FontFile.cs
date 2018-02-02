using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using P3D.Legacy.MapEditor.Utils;

namespace P3D.Legacy.MapEditor.BMFont
{
    public class FontFile : IDisposable
    {
        public static FontFile[] GetFonts(GraphicsDevice graphicsDevice, string name)
        {
            var fontsPaths = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Fonts"), $"{name}_*.fnt", SearchOption.TopDirectoryOnly);
            return fontsPaths.Select(fontPath => new FontFile(graphicsDevice, fontPath)).ToArray();
        }

        public int Size => XmlFontData.Info.Size < 0 ? XmlFontData.Info.Size * -1 : XmlFontData.Info.Size;
        public int LineHeight => XmlFontData.Common.LineHeight;

        public XmlFontFile XmlFontData { get; }
        public Texture2D[] Textures { get; }
        public Dictionary<char, XmlFontChar> GlyphMap { get; } = new Dictionary<char, XmlFontChar>();

        public FontFile(GraphicsDevice graphicsDevice, string fontPath)
        {
            using (var stream = File.Open(fontPath, FileMode.Open, FileAccess.Read))
            using (var textReader = new StreamReader(stream))
                XmlFontData = (XmlFontFile)new XmlSerializer(typeof(XmlFontFile)).Deserialize(textReader);

            Textures = new Texture2D[XmlFontData.Pages.Count];
            foreach (var fontPage in XmlFontData.Pages)
            {
                var texturePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Fonts", fontPage.File);
                Textures[fontPage.ID] = TextureHandler.LoadTexture(graphicsDevice, texturePath);
            }

            foreach (var glyph in XmlFontData.Chars)
                GlyphMap.Add((char) glyph.ID, glyph);
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

        public void Dispose()
        {
            XmlFontData?.Dispose();
            GlyphMap?.Clear();
        }
    }
}