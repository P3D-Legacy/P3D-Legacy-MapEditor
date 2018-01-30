using System;

namespace Gemini.Modules.D3D.Controls
{
    /// <summary>
    /// Provides data for the Draw event.
    /// </summary>
    public sealed class DrawEventArgs : EventArgs
    {
        private readonly IDrawingSurface _drawingSurface;

        public DrawEventArgs(IDrawingSurface drawingSurface)
        {
            _drawingSurface = drawingSurface;
        }

        public void InvalidateSurface()
        {
            _drawingSurface.Invalidate();
        }
    }
}