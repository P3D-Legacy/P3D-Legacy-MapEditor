using System;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using P3D.Legacy.MapEditor.Renders;
using P3D.Legacy.MapEditor.Utils;

namespace RenderTest
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public static Point DefaultResolution => new Point(800, 600);

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        RenderTarget2D renderTarget;
        Effect fxaaEffect;

        public bool useFXAA = true;
        bool doOnce;

        private float fxaaQualitySubpix = 0.75f;
        private float fxaaQualityEdgeThreshold = 0.166f;
        private float fxaaQualityEdgeThresholdMin = 0.0833f;


        private Camera3DMonoGame Camera;
        private Render Render;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.ApplyChanges();

            IsFixedTimeStep = false;

            graphics.PreferredBackBufferWidth = 1440;
            graphics.PreferredBackBufferHeight = 900;

            graphics.ApplyChanges();

            //Window.AllowUserResizing = true;
            //Window.ClientSizeChanged += OnResize;
        }
        public void OnResize(object sender, EventArgs e)
        {
            if (graphics.GraphicsDevice.Viewport.Width < DefaultResolution.X || graphics.GraphicsDevice.Viewport.Height < DefaultResolution.Y)
            {
                Resize(DefaultResolution);
                return;
            }
        }
        public void Resize(Point size)
        {
            if (size.X < DefaultResolution.X || size.Y < DefaultResolution.Y)
                return;

            graphics.PreferredBackBufferWidth = size.X;
            graphics.PreferredBackBufferHeight = size.Y;

            graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
            graphics.ApplyChanges();

            Components.Add(new DebugComponent(this));

            renderTarget = new RenderTarget2D(GraphicsDevice, GraphicsDevice.PresentationParameters.BackBufferWidth, GraphicsDevice.PresentationParameters.BackBufferHeight,
                false, GraphicsDevice.PresentationParameters.BackBufferFormat,
                GraphicsDevice.PresentationParameters.DepthStencilFormat);
            fxaaEffect = new Effect(GraphicsDevice, File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "FXAA3.11.mgfx")));

            var path = "C:\\GitHub\\Maps\\Goldenrod\\goldenrod.dat";
            //var path = "C:\\GitHub\\Maps\\YourRoom\\yourroom.dat";
            //var path = "C:\\GitHub\\Maps\\UnderwaterCave\\main.dat";
            //var path = "C:\\GitHub\\Maps\\Kolben\\devoffices.dat";
            var text = File.ReadAllText(path);
            var levelInfo = LevelLoader.Load(text, path);
            Camera = new Camera3DMonoGame(this);
            //var t = Stopwatch.StartNew();
            Render = new Render(GraphicsDevice, Camera, levelInfo);
            Render.Initialize(GraphicsDevice);
            //t.Stop();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Camera.Update(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.F))
            {
                useFXAA = !useFXAA;
                doOnce = false;
            }

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.Clear(ClearOptions.Stencil, Color.Transparent, 0, 0);
            Render.Draw(GraphicsDevice);
            GraphicsDevice.SetRenderTarget(null);

            if (useFXAA)
            {
                if (!doOnce)
                {
                    float w = renderTarget.Width;
                    float h = renderTarget.Height;

                    fxaaEffect.CurrentTechnique = fxaaEffect.Techniques["ppfxaa_PC"];
                    fxaaEffect.Parameters["fxaaQualitySubpix"].SetValue(fxaaQualitySubpix);
                    fxaaEffect.Parameters["fxaaQualityEdgeThreshold"].SetValue(fxaaQualityEdgeThreshold);
                    fxaaEffect.Parameters["fxaaQualityEdgeThresholdMin"].SetValue(fxaaQualityEdgeThresholdMin);

                    fxaaEffect.Parameters["invViewportWidth"].SetValue(1f / w);
                    fxaaEffect.Parameters["invViewportHeight"].SetValue(1f / h);
                    fxaaEffect.Parameters["texScreen"].SetValue(renderTarget);
                    doOnce = true;
                }


                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, fxaaEffect);
                spriteBatch.Draw(renderTarget, new Rectangle(0, 0, renderTarget.Width, renderTarget.Height), Color.White);
                spriteBatch.End();
            }
            else
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
                spriteBatch.Draw(renderTarget, Vector2.Zero, Color.White);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
