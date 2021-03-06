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

        //Propriétés Levels
        public int levelIndex = -1;
        private const int numberOfLevels = 2;
        private Level level;
        private bool onLevel = false;

        //Propriétés Hub
        private Texture2D _background;
        private Color[] _colorBackground;
        private KeyboardState keyboardState;

        //Overlays
        private Texture2D _exitOverlay;
        private Texture2D _playLevelOverlay;
        private Texture2D _nexusdungeon;

        //Objets du jeu
        private Player Player { get; set; }
        private Player LevelPlayer { get; set; }

        
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

        
        /// <summary>
        /// Méthode de chargement du jeu
        /// </summary>
        protected override void LoadContent()
        {
            this.Content.RootDirectory = "Content";

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            _background = Content.Load<Texture2D>("Sprites/Hub");
            //_background = Content.Load<Texture2D>("Sprites/hubtest");

            _exitOverlay = Content.Load<Texture2D>("Sprites/Overlays/exitgame");
            _playLevelOverlay = Content.Load<Texture2D>("Sprites/Overlays/enterdungeon");
            _nexusdungeon = Content.Load<Texture2D>("Sprites/Overlays/nexusdungeon");
            
            ScalePresentationArea();

            _colorBackground = new Color[_background.Width * _background.Height];
            _background.GetData<Color>(_colorBackground);

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

        /// <summary>
        /// Méthode de mise à jour du jeu suite aux actions
        /// </summary>
        /// <param name="gameTime"></param>
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
                System.Diagnostics.Debug.WriteLine("\n\nNOUS SOMMES DANS LE LEVEL");
                level.Update(gameTime,keyboardState);
                LevelPlayer.Update(gameTime, keyboardState);
                LevelPlayer.Position = LevelPlayer.NextPosition;
                System.Diagnostics.Debug.WriteLine("[level] POSITION APRES UPDATE "+ LevelPlayer.PositionLevel.X + "  :  " + LevelPlayer.PositionLevel.Y);
            }
            else
            {
                Player.Update(gameTime,keyboardState);
                
               if (CanMove((int)Player.NextPosition.X, (int)Player.NextPosition.Y))
                {
                    Player.Position = Player.NextPosition;
                    System.Diagnostics.Debug.WriteLine("POSITION APRES UPDATE " + Player.Position.X + "  :  " + Player.Position.Y+"\n\n");
                }
                    
            }
            
            //base.Update(gameTime);
        }


        /// <summary>
        /// Méthode d'affichage du jeu
        /// </summary>
        /// <param name="gameTime"></param>
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
                _spriteBatch.Draw(_nexusdungeon, new Vector2(10,10), Color.White);
                if (Player.Position.X > 600 && Player.Position.X < 650 && Player.Position.Y <= 800 && Player.Position.Y >= 700)
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

        /// <summary>
        /// Permet de récupérer le couleur à une position donnée
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private Color GetColorAt(int x, int y)
        {
            Color color = Color.White;

            // La position doit être valide
            if (x >= 0 && x < _background.Width && y >= 0 && y < _background.Height)
                color = _colorBackground[x + y * _background.Width];

            return color;
        }

        /// <summary>
        /// Vérifie si le personnage peut se déplacer depuis sa position en fonction de la couleur du fond
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool CanMove(int x, int y)
        {
            
            if (GetColorAt((int)(x/3.75), (int)(y/3.5)) == /*Color.Black*/ new Color(248,215,129,255)){
                return true;
            }
            else
            {
                return false;
            }
            
        }

        /// <summary>
        /// Méthode de rendu upscale du jeu
        /// </summary>
        public void ScalePresentationArea()
        {
            backbufferWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            backbufferHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
            float horScaling = backbufferWidth / baseScreenSize.X;
            float verScaling = backbufferHeight / baseScreenSize.Y;
            Vector3 screenScalingFactor = new Vector3(horScaling, verScaling, 1);
            globalTransformation = Matrix.CreateScale(screenScalingFactor);
        
            //System.Diagnostics.Debug.WriteLine("Screen Size - Width[" + GraphicsDevice.PresentationParameters.BackBufferWidth + "] Height [" + GraphicsDevice.PresentationParameters.BackBufferHeight + "]");
        }

        /// <summary>
        /// Méthode de chargement du prochain niveau
        /// </summary>
        public void LoadNextLevel()
        {
            levelIndex = (levelIndex + 1);

            if (levelIndex > numberOfLevels-1)
            {
                onLevel = false;
            }
            else{
                if (level != null)
                    level.Dispose();

                string levelPath = string.Format("Content/Sprites/Levels/level{0}.txt", levelIndex);
                using (Stream fileStream = TitleContainer.OpenStream(levelPath))
                    level = new Level(this, _spriteBatch, Services, fileStream, levelIndex);
            }
        }


        /// <summary>
        /// Méthode de lancement du prochain niveau
        /// </summary>
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

        /// <summary>
        /// Méthode qui retourne le centre de la fenêtre
        /// </summary>
        /// <returns></returns>
        public Vector2 getWindowCenter()
        {
            return new Vector2(baseScreenSize.X / 2, baseScreenSize.Y / 2);
        }


        /// <summary>
        /// Méthode de fermeture du jeu
        /// </summary>
        public void exit()
        {
            Vector2 overlaySize = new Vector2(_exitOverlay.Width, _exitOverlay.Height);
            _spriteBatch.Draw(_exitOverlay, getWindowCenter() - overlaySize / 2, Color.White);
            if (Keyboard.GetState().IsKeyDown(Keys.Y))
            {
                Exit();
            }
        }

        /// <summary>
        /// Setter du player du level
        /// </summary>
        /// <param name="player"></param>
        public void setLevelPlayer(Player player)
        {
            this.LevelPlayer = player;
        }
    }
}
