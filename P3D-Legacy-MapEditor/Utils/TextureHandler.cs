using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.Legacy.MapEditor.Data.Models
{
    public static class TextureHandler
    {
        // So we create only once all the cropped textures
        private static Dictionary<KeyValuePair<string, Rectangle>, KeyValuePair<Texture2D, bool>> CroppedTextures { get; }
            = new Dictionary<KeyValuePair<string, Rectangle>, KeyValuePair<Texture2D, bool>>();

        public static KeyValuePair<Texture2D, bool> GetCroppedTexture(string texturePath, Rectangle textureRectangle) =>
            GetCroppedTexture(new KeyValuePair<string, Rectangle>(texturePath, textureRectangle));
        public static KeyValuePair<Texture2D, bool> GetCroppedTexture(KeyValuePair<string, Rectangle> key) =>
            !CroppedTextures.ContainsKey(key) ? default(KeyValuePair<Texture2D, bool>) : CroppedTextures[key];

        public static bool HasCroppedTexture(string texturePath, Rectangle textureRectangle)
            => HasCroppedTexture(new KeyValuePair<string, Rectangle>(texturePath, textureRectangle));
        public static bool HasCroppedTexture(KeyValuePair<string, Rectangle> key)
            => CroppedTextures.ContainsKey(key);

        public static void AddCroppedTexture(string texturePath, Rectangle textureRectangle, KeyValuePair<Texture2D, bool> value)
            => AddCroppedTexture(new KeyValuePair<string, Rectangle>(texturePath, textureRectangle), value);
        public static void AddCroppedTexture(KeyValuePair<string, Rectangle> key, KeyValuePair<Texture2D, bool> value)
            => CroppedTextures.Add(key, value);


        private static Dictionary<string, Texture2D> LoadedTextures { get; } = new Dictionary<string, Texture2D>();
        public static Texture2D LoadTexture(GraphicsDevice graphicsDevice, EntityInfo entity)
        {
            if (LoadedTextures.TryGetValue(entity.TexturePath, out var texture))
                return texture;

            var path = Path.Combine(entity.Parent.TexturesLocation, $"{entity.TexturePath}.png");

            var stream = File.OpenRead(path);
            //var textLoader = new TextureLoader(graphicsDevice);
            //texture = textLoader.FromStreamFast(stream, false);

            texture = Texture2D.FromStream(graphicsDevice, File.OpenRead(path));

            var pixels = new Color[texture.Width * texture.Height];
            texture.GetData(pixels);
            for (var i = 0; i < pixels.Length; i++)
            {
                if (pixels[i] == Color.Magenta)
                    pixels[i] = Color.TransparentBlack;
            }
            texture.SetData(pixels);

            stream.Dispose();
            LoadedTextures.Add(entity.TexturePath, texture);
            return texture;
        }
        public static Texture2D LoadTexture(GraphicsDevice graphicsDevice, string path)
        {
            if (LoadedTextures.TryGetValue(path, out var texture))
                return texture;

            var stream = File.OpenRead(path);
            //var textLoader = new TextureLoader(graphicsDevice);
            //texture = textLoader.FromStreamFast(stream, false);

            texture = Texture2D.FromStream(graphicsDevice, File.OpenRead(path));

            var pixels = new Color[texture.Width * texture.Height];
            texture.GetData(pixels);
            for (var i = 0; i < pixels.Length; i++)
            {
                if (pixels[i] == Color.Magenta)
                    pixels[i] = Color.TransparentBlack;
            }
            texture.SetData(pixels);

            stream.Dispose();
            LoadedTextures.Add(path, texture);
            return texture;
        }



        public static void Dispose()
        {
            foreach (var pair in CroppedTextures)
                pair.Value.Key.Dispose();
            CroppedTextures.Clear();

            foreach (var pair in LoadedTextures)
                pair.Value.Dispose();
            LoadedTextures.Clear();
        }
    }
}