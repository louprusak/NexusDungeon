using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NexusDungeon.Core.Game;
using System.Collections.Generic;

namespace NexusDungeon.Core
{
    public class NexusDungeonGame : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Player Player { get; set; }
        public List<GameObject> GameObjects { get; set; }

        public NexusDungeonGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            _spriteBatch = new SpriteBatch(graphicsDevice: GraphicsDevice);
            Player = new Player(this, _spriteBatch);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);

            foreach(var gameObject in GameObjects)
            {
                gameObject.Update(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();

            foreach(var gameObject in GameObjects)
            {
                gameObject.Draw(gameTime);
            }

            
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
