using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P3D.Legacy.MapEditor.BMFont
{
    public interface IFontRenderer : IDisposable
    {
        void DrawText(SpriteBatch spriteBatch, string text, Rectangle borders, Color color, bool centerHeight = true);
        void DrawText(SpriteBatch spriteBatch, string text, Vector2 position, Color color);

        void DrawTextCentered(SpriteBatch spriteBatch, string text, Rectangle borders, Color color);
        void DrawTextCenteredStretched(SpriteBatch spriteBatch, string text, Rectangle borders, Color color);

        void DrawTextStretched(SpriteBatch spriteBatch, string text, Rectangle borders, Color color);
    }
}