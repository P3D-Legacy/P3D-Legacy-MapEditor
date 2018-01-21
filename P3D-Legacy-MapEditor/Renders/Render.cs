using Microsoft.Xna.Framework.Graphics;

using P3D.Legacy.MapEditor.Data;
using P3D.Legacy.MapEditor.Data.World;
using P3D.Legacy.MapEditor.Utils;
using P3D.Legacy.MapEditor.World;

namespace P3D.Legacy.MapEditor.Renders
{
    public class Render : IRender
    {
        private GraphicsDevice GraphicsDevice { get; }

        private BasicEffect BasicEffect { get; set; }
        private Level Level { get; set; }

        private Camera Camera { get; set; }

        //private readonly CubePrimitive _cube;

        public Render(GraphicsDevice graphicsDevice, Camera camera, LevelInfo levelInfo)
        {
            GraphicsDevice = graphicsDevice;
            Camera = camera;

            if (levelInfo != null)
                Level = new Level(levelInfo, graphicsDevice);

            //_cube = new CubePrimitive();
        }
        
        public void Initialize(GraphicsDevice graphicsDevice)
        {
            BasicEffect = new BasicEffect(GraphicsDevice);
            BasicEffect.TextureEnabled = true;
            BasicEffect.VertexColorEnabled = true;
            BasicEffect.FogEnabled = false;

            Level.UpdateLighting(BasicEffect);
            Level.SetWeather(BasicEffect, Weather.Clear);
            /*
            BasicEffect.EnableDefaultLighting();
            BasicEffect.AmbientLightColor = new Vector3(0.1f);
            BasicEffect.DiffuseColor = new Vector3(1.0f);
            BasicEffect.SpecularColor = new Vector3(0.25f);
            BasicEffect.SpecularPower = 5.0f;
            */

            //_cube.Initialize(graphicsDevice);
        }

        public void Draw(GraphicsDevice graphicsDevice)
        {
            Camera.BeforeDraw(graphicsDevice);

            BasicEffect.View = Camera.View;
            BasicEffect.Projection = Camera.Projection;


            Level?.Draw(BasicEffect, Camera.Position);
            /*
            BaseModel model = null;
            foreach (var baseModel in BaseModel.TotalStaticModels.Where(m => m.Entity.EntityID != "ScriptBlock"))
            {
                if (baseModel.Intersects(Camera.Mouse, Camera.View, Camera.Projection, GraphicsDevice.Viewport))
                {
                    model = baseModel;
                    break;
                }
            }


            if(model == null)
                return;
            */

            /*
            var buffers = SS.CreateBoundingBoxBuffers(model.BoundingBox, GraphicsDevice);
            graphicsDevice.SetVertexBuffer(buffers.Vertices);
            graphicsDevice.Indices = buffers.Indices;
            foreach (var pass in BasicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0,
                    buffers.VertexCount, 0, buffers.PrimitiveCount);
            }
            */


            //_cube.World = model.EntityWorld;
            //_cube.Recalc();
            //_cube.Draw(BasicEffect, new Color(Color.LightGreen, 0.5f));
        }
    }
}
