using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using NexusDungeon.Core.Game;
using System.Collections.Generic;


namespace NexusDungeon.Core
{
    public class NexusDungeonGame : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D _background;

        private Color[] _colorBackground;

        private Player Player { get; set; }
        public List<GameObject> GameObjects { get; set; } = new List<GameObject>();

        public NexusDungeonGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.PreferredBackBufferWidth= 1920;
          

            base.Initialize();
            _spriteBatch = new SpriteBatch(graphicsDevice: GraphicsDevice);
            Player = new Player(this, _spriteBatch);
            GameObjects.Add(Player);

            _colorBackground = new Color[_background.Width * _background.Height];
            _background.GetData<Color>(_colorBackground);
        }

        protected override void LoadContent()
        {
            this.Content.RootDirectory = "Content";

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            _background = Content.Load<Texture2D>("Sprites/hub");

            //Musique d'ambiance du niveau
            try
            {
                MediaPlayer.IsRepeating = true;
                //MediaPlayer.Play(Content.Load<Song>("Sprites/Sounds/forest"));
            }
            catch { }
            

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
            Player.NextPosition = Player.Position;

            if (CanMove((int)Player.NextPosition.X, (int)Player.NextPosition.Y))
                Player.Position = Player.NextPosition;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();
            _spriteBatch.Draw(_background, new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height), Color.White);

            foreach(var gameObject in GameObjects)
            {
                gameObject.Draw(gameTime);
            }

            
            _spriteBatch.End();

            base.Draw(gameTime);
        }


        private Color GetColorAt(int x, int y)
        {
            Color color = Color.White;
            // La position doit être valide
 
            if (x >= 0 && x < _background.Width && y >= 0 && y < _background.Height)
                color = _colorBackground[x + y * _background.Width];

            return color;
        }

        private bool CanMove(int x, int y)
        {
            // On évite le blanc (0xFFFFFF)
            return GetColorAt(x, y) != Color.White;
        }
    }
}
