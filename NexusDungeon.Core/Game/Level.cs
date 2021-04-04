using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;

namespace NexusDungeon.Core.Game
{
    public class Level : IDisposable
    {
        //Structure d'un niveau
        private Tile[,] tiles;
        private Matrix globalTransformation;
        int backbufferWidth, backbufferHeight;
        Vector2 baseScreenSize = new Vector2(512, 288);

        //Entités dans le niveau
        public Player Player { get; set; }
        private List<Enemy> enemies = new List<Enemy>();

        //Positions
        private Vector2 start;
        private Point exit = InvalidPosition;
        private static readonly Point InvalidPosition = new Point(-1, -1);

        //Sounds (En cours)
        private SoundEffect music;
        private Song exitReachedSound;
        private SoundEffect chestSound;

        //Propriétés du Level
        public bool ReachedExit { get; set; }
        private Microsoft.Xna.Framework.Game game;
        private NexusDungeonGame nexusgame;
        private SpriteBatch spriteBatch;

        //Contenu du Level
        public ContentManager Content { get; set; }
        public int Height
        {
            get { return tiles.GetLength(1); }
        }
        public int Width
        {
            get { return tiles.GetLength(0); }
        }
        

        //################################################################################################################################################################//
        //Constructeur
        public Level(Microsoft.Xna.Framework.Game game,SpriteBatch spriteBatch,IServiceProvider serviceProvider, Stream fileStream, int levelIndex)
        {
            this.game = game;
            this.nexusgame = (NexusDungeonGame)game;
            this.spriteBatch = spriteBatch;

            Content = new ContentManager(serviceProvider, "Content");

            ScalePresentationArea();

            LoadTiles(fileStream);

            MediaPlayer.Play(game.Content.Load<Song>("Sprites/Sounds/dungeon"));
            MediaPlayer.MoveNext();

            

            exitReachedSound = Content.Load<Song>("Sprites/Sounds/forest");
        }

        //################################################################################################################################################################//
        //METHODES DE CHARGEMENT
        
        private void LoadTiles(Stream fileStream)
        {
            backbufferWidth = game.GraphicsDevice.PresentationParameters.BackBufferWidth;
            backbufferHeight = game.GraphicsDevice.PresentationParameters.BackBufferHeight;
            float horScaling = backbufferWidth / baseScreenSize.X;
            float verScaling = backbufferHeight / baseScreenSize.Y;

            // Chargement du level et verification de la taille
            int width;
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(fileStream))
            {
                string line = reader.ReadLine();
                width = line.Length;
                while (line != null)
                {
                    lines.Add(line);
                    if (line.Length != width)
                        throw new Exception(String.Format("The length of line {0} is different from all preceeding lines.", lines.Count));
                    line = reader.ReadLine();
                }
            }

            // Creation de la grille de tuiles
            tiles = new Tile[width, lines.Count];
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    char tileType = lines[y][x];
                    tiles[x, y] = LoadTile(tileType, (int)(x*horScaling), (int)(y*verScaling));
                }
            }

            // Verification présence d'un début et d'une fin dans le level
            if (Player == null)
                throw new NotSupportedException("A level must have a starting point.");
            if (exit == InvalidPosition)
                throw new NotSupportedException("A level must have an exit.");

            System.Diagnostics.Debug.WriteLine("Width" + Width + "Height" + Height);

        }

        /// <summary>
        /// Charge une tuile de texture en fonction du caractère rencontré dans le fichier texte et la position
        /// </summary>
        /// <param name="tileType">Type de tuile de texture</param>
        /// <param name="x">Position en largeur</param>
        /// <param name="y">Position en hauteur</param>
        /// <returns>Retourne l'objet Tile créé dans certains cas</returns>
        private Tile LoadTile(char tileType, int x, int y)
        {
            switch (tileType)
            {
                //Zone vide
                case '.':
                    return new Tile(Content.Load<Texture2D>("Sprites/vide"), TileCollision.Passable);
                //Mur
                case '#':
                    return new Tile(Content.Load<Texture2D>("Sprites/mur"), TileCollision.Passable);
                //Sol
                case '_':
                    return new Tile(Content.Load<Texture2D>("Sprites/sol"), TileCollision.Passable);
                //Sol
                case 'X':
                    return LoadExitTile(x, y);
                //Depart
                case '1':
                    return LoadStartTile(x, y);
                //Enemis (En cours)
                /*case 'A':
                    return new Tile(x, y, "MonsterA");
                case 'B':
                    return new Tile(x, y, "MonsterB");
                case 'C':
                    return new Tile(x, y, "MonsterC");
                case 'D':
                    return new Tile(x, y, "MonsterD");*/
                // Unknown tile type character
                default:
                    throw new NotSupportedException(String.Format("Type de tuile non supporté '{0}' a la position {1}, {2}.", tileType, x, y));
            }
        }

        /// <summary>
        /// Chargement de la tuile de sortie du level
        /// </summary>
        /// <param name="x">Position en largeur</param>
        /// <param name="y">Position en hauteur</param>
        /// <returns>Instance de la tuile</returns>
        private Tile LoadExitTile(int x, int y)
        {
            if (exit != InvalidPosition)
                throw new NotSupportedException("A level may only have one exit.");

            exit = GetBounds(x, y).Center;
            System.Diagnostics.Debug.WriteLine("Position de la tuile exit :" + exit);

            return new Tile(Content.Load<Texture2D>("Sprites/sortie"), TileCollision.Passable);
        }

        /// <summary>
        /// Chargement de la tuile d'entrée du level
        /// </summary>
        /// <param name="x">Position en largeur</param>
        /// <param name="y">Position en heuteur</param>
        /// <returns>Instance de la tuile</returns>
        private Tile LoadStartTile(int x, int y)
        {
            if (Player != null)
                throw new NotSupportedException("A level may only have one starting point.");

            start = RectangleUtils.GetBottomCenter(GetBounds(x, y));
            Player = new Player(this, start);
            nexusgame.setLevelPlayer(Player);

            return new Tile(Content.Load<Texture2D>("Sprites/depart"), TileCollision.Passable);
        }

        /// <summary>
        /// Déchargement de l'ancien level
        /// </summary>
        public void Dispose()
        {
            Content.Unload();
        }




        //################################################################################################################################################################//
        //METHODES GERANT LES COLLISIONS



        /// <summary>
        /// Retourne le type de collision d'une tuile de texture à une position donnée
        /// </summary>
        public TileCollision GetCollision(int x, int y)
        {
            if (x < 0 || x >= (Width))
                return TileCollision.Impassable;
            if (y < 0 || y >= (Height))
                return TileCollision.Passable;
            return tiles[x, y].Collision;
        }

        /// <summary>
        /// Retourne la zone occupée par une tuile
        /// </summary>        
        public Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height);
        }




        //################################################################################################################################################################//
        //METHODE DE MISE A JOUR DES LEVELS ET LEURS ELEMENTS
    


        /// <summary>
        /// Méthode de mise à jour du level
        /// </summary>
        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {  
            if (!ReachedExit)
            {
                ScalePresentationArea();
                Player.Update(gameTime,keyboardState);
                if (
                    Player.PositionLevel.X > exit.X-50 
                    && Player.PositionLevel.X < exit.X + 50 &&
                    Player.PositionLevel.Y > exit.Y-50
                    && Player.PositionLevel.Y < exit.Y + 100)
                {
                    OnExitReached();
                }
            }

        }

        

        /// <summary>
        /// Mise a jour des enemis (en cours)
        /// </summary>
        private void UpdateEnemies(GameTime gameTime)
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.Update(gameTime);



            }
        }


        /// <summary>
        /// Quand le joueur a atteint la fin du niveau
        /// </summary>
        private void OnExitReached()
        {
            //Player.OnReachedExit();
            //exitReachedSound.Play();
     
            //System.Threading.Thread.Sleep(7000);
            nexusgame.LoadNextLevel();
            
            ReachedExit = true;
        }

     

        //################################################################################################################################################################//
        //METHODES D'AFFICHAGE DES ELEMENTS




        /// <summary>
        /// Méthode draw du level
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            DrawTiles(spriteBatch);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, null);
            Player.Draw(gameTime, spriteBatch);
            spriteBatch.DrawString(Content.Load<SpriteFont>("Sprites/Font/Hud"), "Level "+ ((NexusDungeonGame)game).levelIndex+"\nVies : 5", Vector2.One, Color.White);

            //Affichage des enemis dans le level (en cours)
            foreach (Enemy enemy in enemies)
                enemy.Draw(gameTime);

        }

        /// <summary>
        /// Méthode de rendu upscale du jeu
        /// </summary>
        public void ScalePresentationArea()
        {
            backbufferWidth = game.GraphicsDevice.PresentationParameters.BackBufferWidth;
            backbufferHeight = game.GraphicsDevice.PresentationParameters.BackBufferHeight;
            float horScaling = backbufferWidth / baseScreenSize.X;
            float verScaling = backbufferHeight / baseScreenSize.Y;
            Vector3 screenScalingFactor = new Vector3(horScaling, verScaling, 1);
            globalTransformation = Matrix.CreateScale(screenScalingFactor);
            //System.Diagnostics.Debug.WriteLine("Screen Size - Width[" + game.GraphicsDevice.PresentationParameters.BackBufferWidth + "] Height [" + game.GraphicsDevice.PresentationParameters.BackBufferHeight + "]");
        }

        /// <summary>
        /// Affichage de toutes les tuiles du level
        /// </summary>
        private void DrawTiles(SpriteBatch spriteBatch)
        {
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    Texture2D texture = tiles[x, y].Texture;
                    if (texture != null)
                    {
                        Vector2 position = new Vector2(x, y) * Tile.Size;
                        spriteBatch.Draw(texture, position, Color.White);
                    }
                }
            }
        }

    }
}
