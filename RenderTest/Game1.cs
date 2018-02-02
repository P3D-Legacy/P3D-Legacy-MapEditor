using System;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using P3D.Legacy.MapEditor.Components;
using P3D.Legacy.MapEditor.Components.Camera;
using P3D.Legacy.MapEditor.Components.Debug;
using P3D.Legacy.MapEditor.Components.ModelSelector;
using P3D.Legacy.MapEditor.Components.Render;
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

        private GraphicsDeviceManager Graphics { get; }

        private Render _render;

        public Game1()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Graphics.SynchronizeWithVerticalRetrace = false;
            Graphics.ApplyChanges();

            IsFixedTimeStep = false;

            Graphics.PreferredBackBufferWidth = 1440;
            Graphics.PreferredBackBufferHeight = 900;

            Graphics.ApplyChanges();

            //Window.AllowUserResizing = true;
            //Window.ClientSizeChanged += OnResize;
        }
        public void OnResize(object sender, EventArgs e)
        {
            if (Graphics.GraphicsDevice.Viewport.Width < DefaultResolution.X || Graphics.GraphicsDevice.Viewport.Height < DefaultResolution.Y)
            {
                Resize(DefaultResolution);
                return;
            }
        }
        public void Resize(Point size)
        {
            if (size.X < DefaultResolution.X || size.Y < DefaultResolution.Y)
                return;

            Graphics.PreferredBackBufferWidth = size.X;
            Graphics.PreferredBackBufferHeight = size.Y;

            Graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
            Graphics.ApplyChanges();

            var path = "C:\\GitHub\\Maps\\Goldenrod\\goldenrod.dat";
            //var path = "C:\\GitHub\\Maps\\YourRoom\\yourroom.dat";
            //var path = "C:\\GitHub\\Maps\\UnderwaterCave\\main.dat";
            //var path = "C:\\GitHub\\Maps\\Kolben\\devoffices.dat";
            var text = File.ReadAllText(path);
            var levelInfo = LevelLoader.Load(text, path);

            var camera = new Camera3DMonoGame(this);
            Components.Add(camera);

            var modelSelector = new ModelSelectorDefault(camera);
            Components.Add(modelSelector);

            _render = new Render(GraphicsDevice, camera, modelSelector, levelInfo);
            Components.Add(_render);

            Components.Add(new DebugTextComponent(GraphicsDevice, Components));

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            base.UnloadContent();

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

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _render.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}
