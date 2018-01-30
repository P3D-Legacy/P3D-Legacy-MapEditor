namespace Gemini.Modules.D3D.Controls
{
    public interface IDrawingSurface 
    {
        double DrawTime { get; }

        void Invalidate();
    }
}