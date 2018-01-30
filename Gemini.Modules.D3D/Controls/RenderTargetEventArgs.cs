using System;
using System.Windows;

namespace Gemini.Modules.D3D.Controls
{
    /// <summary>
    /// Provides data for the Draw event.
    /// </summary>
    public sealed class RenderTargetEventArgs : EventArgs
    {
        public IntPtr RenderTargetPtr { get; private set; }
        public Int32Rect RenderTargetRectangle { get; private set; }

        public void SetRenderTarget(IntPtr renderTargetPtr, Int32Rect renderTargetRectangle)
        {
            if (RenderTargetPtr != IntPtr.Zero && RenderTargetRectangle != Int32Rect.Empty)
            {
                RenderTargetPtr = renderTargetPtr;
                RenderTargetRectangle = renderTargetRectangle;
            }
        }
    }
}