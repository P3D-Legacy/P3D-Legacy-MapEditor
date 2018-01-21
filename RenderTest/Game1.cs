using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using P3D.Legacy.MapEditor.Renders;
using P3D.Legacy.MapEditor.Utils;
using P3D.Legacy.MapEditor.World;

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

            Components.Add(new DebugComponent(this));

            //var path = "C:\\GitHub\\Maps\\Goldenrod\\goldenrod.dat";
            var path = "C:\\GitHub\\Maps\\YourRoom\\yourroom.dat";
            //var path = "C:\\GitHub\\Maps\\UnderwaterCave\\main.dat";
            var text = File.ReadAllText(path);
            var levelInfo = LevelLoader.Load(text, path);
            Camera = new Camera3DMonoGame(GraphicsDevice);
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

            InputManager.Update(gameTime);
            Camera.Update(gameTime);
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            Render.Draw(GraphicsDevice);
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
