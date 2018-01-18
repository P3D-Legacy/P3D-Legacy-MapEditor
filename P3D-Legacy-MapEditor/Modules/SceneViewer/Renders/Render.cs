using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using P3D.Legacy.MapEditor.Data;
using P3D.Legacy.MapEditor.Modules.SceneViewer.ViewModels;
using P3D.Legacy.MapEditor.Modules.SceneViewer.Views;
using P3D.Legacy.MapEditor.Primitives;
using P3D.Legacy.MapEditor.Utils;
using P3D.Legacy.MapEditor.World;

namespace P3D.Legacy.MapEditor.Modules.SceneViewer.Renders
{
    public class Render : IRender
    {
        private SceneView Control { get; }
        private SceneViewModel SceneViewModel => Control.DataContext as SceneViewModel;
        private GraphicsDevice GraphicsDevice => Control.GraphicsDevice;

        private Effect Effect { get; set; }
        private BasicEffect BasicEffect { get; set; }
        private Level Level { get; set; }

        private ICamera Camera { get; set; }

        private readonly CubePrimitive _cube;

        public Render(SceneView sceneView)
        {
            Control = sceneView;
            Camera = new Camera3D(Control);
            //Camera = new Camera3DNilllzz(UserControl);

            _cube = new CubePrimitive();
        }
        
        public void Initialize(GraphicsDevice graphicsDevice)
        {
            var reader = new BinaryReader(File.Open("C:\\GitHub\\P3D-Legacy-MapEditor — копия\\P3D-Legacy-MapEditor\\Content\\Shader.mgfx", FileMode.Open));
            Effect = new Effect(graphicsDevice, reader.ReadBytes((int) reader.BaseStream.Length));
            reader.Dispose();
            Effect.Parameters["DiffuseColor"].SetValue(new Vector3(1.0f));

            BasicEffect = new BasicEffect(GraphicsDevice);
            BasicEffect.EnableDefaultLighting();
            BasicEffect.AmbientLightColor = new Vector3(0.1f);
            BasicEffect.DiffuseColor = new Vector3(1.0f);
            BasicEffect.SpecularColor = new Vector3(0.25f);
            BasicEffect.SpecularPower = 5.0f;

            Level = new Level(SceneViewModel.LevelInfo, graphicsDevice);
            _cube.Initialize(graphicsDevice);
        }

        public bool HandleMouse(MouseDevice mouse)
        {
            return Camera.HandleMouse(mouse);
        }

        public bool HandleKeyboard(KeyboardDevice keyboard)
        {
            return Camera.HandleKeyboard(keyboard);
        }

        public void Draw(GraphicsDevice graphicsDevice)
        {
            Camera.BeforeDraw(graphicsDevice);

            Effect.Parameters["View"].SetValue(Camera.View);
            Effect.Parameters["Projection"].SetValue(Camera.Projection);

            BasicEffect.View = Camera.View;
            BasicEffect.Projection = Camera.Projection;

            Level.Draw(Effect);

            //_cube.Draw(BasicEffect, Color.LightGreen);
        }
    }
}
