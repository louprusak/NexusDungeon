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
        //Eléments graphiques
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Matrix globalTransformation;
        int backbufferWidth, backbufferHeight;
        // 512/16 = 32 tuiles largeur et 288/16 = 18 tuiles en hauteur car les tuiles font du 16 par 16
        Vector2 baseScreenSize = new Vector2(512, 288);

        //Levels
        public int levelIndex = -1;
        private const int numberOfLevels = 1;
        private Level level;
        private bool onLevel = false;

        //Hub
        private Texture2D _background;
        private Color[] _colorBackground;
        private KeyboardState keyboardState;

        //Overlays
        private Texture2D _exitOverlay;
        private Texture2D _playLevelOverlay;

        //Objets du jeu
        private Player Player { get; set; }
        public List<GameObject> GameObjects { get; set; } = new List<GameObject>();

        //################################################################################################################################################################//
        //CONSTRUCTEUR

        public NexusDungeonGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.IsFullScreen = true;
            IsMouseVisible = true;
            
        }

        //################################################################################################################################################################//
        //METHODES MONOGAME

        

        protected override void LoadContent()
        {
            this.Content.RootDirectory = "Content";

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            _background = Content.Load<Texture2D>("Sprites/hub");

            _colorBackground = new Color[_background.Width * _background.Height];
            _background.GetData<Color>(_colorBackground);

            _exitOverlay = Content.Load<Texture2D>("Sprites/Overlays/exitgame");
            _playLevelOverlay = Content.Load<Texture2D>("Sprites/Overlays/enterdungeon");

            ScalePresentationArea();

            Player = new Player(this, _spriteBatch);

            //Musique d'ambiance du niveau
            try
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(Content.Load<Song>("Sprites/Sounds/forest"));
                MediaPlayer.Volume = (float)0.3;
            }
            catch { }

        }

        protected override void Update(GameTime gameTime)
        {
            //Check sortie du jeu
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Check redimensionnement de la fenêtre graphique
            if (backbufferHeight != GraphicsDevice.PresentationParameters.BackBufferHeight ||
                backbufferWidth != GraphicsDevice.PresentationParameters.BackBufferWidth)
            {
                ScalePresentationArea();
            }

            keyboardState = Keyboard.GetState();

            if (onLevel)
            {
                level.Update(gameTime,keyboardState);
            }
            else
            {
                Player.Update(gameTime,keyboardState);
                
                if (CanMove((int)Player.NextPosition.X, (int)Player.NextPosition.Y))
                    Player.Position = Player.NextPosition;
            }
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            if (onLevel)
            {
                GraphicsDevice.Clear(Color.White);
                _spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, globalTransformation);
                level.Draw(gameTime, _spriteBatch);
                _spriteBatch.End();
            }
            else
            {
                GraphicsDevice.Clear(Color.White);
                _spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, globalTransformation);
                _spriteBatch.Draw(_background, Vector2.Zero, Color.White);
                if (Player.Position.X < 100 && Player.Position.Y <= 800 && Player.Position.Y >= 700)
                {
                    exit();
                }
                if (Player.Position.X <=900 && Player.Position.X >= 800 && Player.Position.Y >= 480 && Player.Position.Y <= 580)
                {
                    PlayLevel();
                }
                _spriteBatch.End();
                _spriteBatch.Begin(SpriteSortMode.Immediate,null,SamplerState.PointClamp,null,null,null,null);
                Player.Draw(gameTime, _spriteBatch);
                _spriteBatch.End();
            }

            base.Draw(gameTime);
            
        }


        //################################################################################################################################################################//
        //METHODES

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
            Vector2 overlaySize = new Vector2(_playLevelOverlay.Width, _playLevelOverlay.Height);

            _spriteBatch.Draw(_playLevelOverlay, getWindowCenter() - overlaySize / 2, Color.White);
            if (Keyboard.GetState().IsKeyDown(Keys.Y))
            {
                LoadNextLevel();
                onLevel = true;
            }
        }

        public Vector2 getWindowCenter()
        {
            return new Vector2(baseScreenSize.X / 2, baseScreenSize.Y / 2);
        }

        public void exit()
        {
            Vector2 overlaySize = new Vector2(_exitOverlay.Width, _exitOverlay.Height);
            _spriteBatch.Draw(_exitOverlay, getWindowCenter() - overlaySize / 2, Color.White);
            if (Keyboard.GetState().IsKeyDown(Keys.Y))
            {
                Exit();
            }
        }
    }
}
