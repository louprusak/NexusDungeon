using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using NexusDungeon.Core.Game;
using System.Collections.Generic;
using System.IO;

namespace NexusDungeon.Core
{
    public class NexusDungeonGame : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Matrix globalTransformation;
        int backbufferWidth, backbufferHeight;
        Vector2 baseScreenSize = new Vector2(400, 400);
        private int levelIndex = -1;
        private const int numberOfLevels = 1;
        private Level level;
        private Texture2D _background;
        private bool onLevel = false;

        private Color[] _colorBackground;

        private Player Player { get; set; }
        public List<GameObject> GameObjects { get; set; } = new List<GameObject>();

        //################################################################################################################################################################//


        public NexusDungeonGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        //################################################################################################################################################################//


        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.PreferredBackBufferWidth= 1920;
            _graphics.ApplyChanges();

            base.Initialize();
            _spriteBatch = new SpriteBatch(graphicsDevice: GraphicsDevice);
            Player = new Player(this, _spriteBatch);
            GameObjects.Add(Player);

            
        }

        protected override void LoadContent()
        {
            this.Content.RootDirectory = "Content";

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            _background = Content.Load<Texture2D>("Sprites/hub");
            //_background = Content.Load<Texture2D>("Sprites/level");
            _colorBackground = new Color[_background.Width * _background.Height];
            _background.GetData<Color>(_colorBackground);


            ScalePresentationArea();

            //Musique d'ambiance du niveau
            try
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(Content.Load<Song>("Sprites/Sounds/forest"));
                MediaPlayer.Volume = (float) 0.3 ;
            }
            catch { }


            LoadNextLevel();

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
            Player.Update(gameTime);
            if (CanMove((int)Player.NextPosition.X, (int)Player.NextPosition.Y))
                Player.Position = Player.NextPosition;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            _spriteBatch.Begin();


            if (onLevel)
            {
                level.Draw(gameTime, _spriteBatch);
                MediaPlayer.Play(Content.Load<Song>("Sprites/Sounds/dungeon"));
            }
            else
            {
                _spriteBatch.Draw(_background, new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height), Color.White);
                foreach (var gameObject in GameObjects)
                {
                    gameObject.Draw(gameTime);
                }
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
            /*
            if (GetColorAt(x, y) == Color.White)
               return false;
            if (GetColorAt(x, y) != _colorBackground[1024 + 200 * _background.Width])
                return true;
            else
                return false;
            */
            return true;
        }

        public void ScalePresentationArea()
        {
            //Work out how much we need to scale our graphics to fill the screen
            backbufferWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            backbufferHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
            float horScaling = backbufferWidth / baseScreenSize.X;
            float verScaling = backbufferHeight / baseScreenSize.Y;
            Vector3 screenScalingFactor = new Vector3(horScaling, verScaling, 1);
            globalTransformation = Matrix.CreateScale(screenScalingFactor);
            System.Diagnostics.Debug.WriteLine("Screen Size - Width[" + GraphicsDevice.PresentationParameters.BackBufferWidth + "] Height [" + GraphicsDevice.PresentationParameters.BackBufferHeight + "]");
        }

        public void LoadNextLevel()
        {
            // move to the next level
            levelIndex = (levelIndex + 1) % numberOfLevels;

            // Unloads the content for the current level before loading the next one.
            if (level != null)
                level.Dispose();

            // Load the level.
            string levelPath = string.Format("Content/Sprites/Levels/level{0}.txt", levelIndex);
            using (Stream fileStream = TitleContainer.OpenStream(levelPath))
                level = new Level(this,_spriteBatch,Services, fileStream, levelIndex);
        }

        public void PlayLevel()
        {
            
            LoadNextLevel();
            onLevel = true;
        }
    }
}
