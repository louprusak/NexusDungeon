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
    
    class Level : IDisposable
    {
        //Structure
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

        //Sounds
        private SoundEffect music;
        private Song exitReachedSound;
        private SoundEffect chestSound;

        //Font
        private SpriteFont hudFont;

        //Propriétés du Level
        public bool ReachedExit { get; set; }
        private Microsoft.Xna.Framework.Game game;
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

            // Load the level and ensure all of the lines are the same length.
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

            // Allocate the tile grid.
            tiles = new Tile[width, lines.Count];

            // Loop over every tile position,
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    // to load each tile.
                    char tileType = lines[y][x];
                    tiles[x, y] = LoadTile(tileType, (int)(x*horScaling), (int)(y*verScaling));
                }
            }

            // Verify that the level has a beginning and an end.
            if (Player == null)
                throw new NotSupportedException("A level must have a starting point.");
            if (exit == InvalidPosition)
                throw new NotSupportedException("A level must have an exit.");
        }

        /// <summary>
        /// Charge une tuile de texture en fonction du caractère rencontré dans le fichier texte et la position
        /// </summary>
        /// <param name="tileType">Type de tuile de texture</param>
        /// <param name="x">Position en largeur</param>
        /// <param name="y">Position en hauteur</param>
        /// <returns>Retourne l'objet Tile créée dans certains cas</returns>
        private Tile LoadTile(char tileType, int x, int y)
        {
            switch (tileType)
            {
                //Zone vide
                case '.':
                    return new Tile(Content.Load<Texture2D>("Sprites/vide"), TileCollision.Impassable);
                //Mur
                case '#':
                    return new Tile(Content.Load<Texture2D>("Sprites/mur"), TileCollision.Impassable);
                //Sol
                case '_':
                    return new Tile(Content.Load<Texture2D>("Sprites/sol"), TileCollision.Passable);
                //Sol
                case 'X':
                    return LoadExitTile(x, y);
                //Depart
                case '1':
                    return LoadStartTile(x, y);
                //Enemis
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
                    throw new NotSupportedException(String.Format("Unsupported tile type character '{0}' at position {1}, {2}.", tileType, x, y));
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
        /// Gets the collision mode of the tile at a particular location.
        /// This method handles tiles outside of the levels boundries by making it
        /// impossible to escape past the left or right edges, but allowing things
        /// to jump beyond the top of the level and fall off the bottom.
        /// </summary>
        public TileCollision GetCollision(int x, int y)
        {
            // Prevent escaping past the level ends.
            if (x < 0 || x >= Width)
                return TileCollision.Impassable;
            // Allow jumping past the level top and falling through the bottom.
            if (y < 0 || y >= Height)
                return TileCollision.Passable;

            return tiles[x, y].Collision;
        }

        /// <summary>
        /// Gets the bounding rectangle of a tile in world space.
        /// </summary>        
        public Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height);
        }




        //################################################################################################################################################################//
        //METHODE DE MISE A JOUR DES LEVELS ET LEURS ELEMENTS
    


        /// <summary>
        /// Updates all objects in the world, performs collision between them,
        /// and handles the time limit with scoring.
        /// </summary>
        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            
            if (!ReachedExit)
            {
                ScalePresentationArea();

                Player.Update(gameTime,keyboardState);
                

                // Falling off the bottom of the level kills the player.
                //if (Player.BoundingRectangle.Top >= Height * Tile.Height)
                    //OnPlayerKilled(null);

                //UpdateEnemies(gameTime);

                // The player has reached the exit if they are standing on the ground and
                // his bounding rectangle contains the center of the exit tile. They can only
                // exit when they have collected all of the gems.
                if (//Player.IsAlive &&
                   // Player.IsOnGround &&
                    Player.BoundingRectangle.Contains(exit))
                {
                    OnExitReached();
                }
            }

        }

        

        /// <summary>
        /// Animates each enemy and allow them to kill the player.
        /// </summary>
        private void UpdateEnemies(GameTime gameTime)
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.Update(gameTime);

                // Touching an enemy instantly kills the player
                //if (enemy.BoundingRectangle.Intersects(Player.BoundingRectangle))
                //{
                    //OnPlayerKilled(enemy);
                //}
            }
        }

        

        /// <summary>
        /// Called when the player is killed.
        /// </summary>
        /// <param name="killedBy">
        /// The enemy who killed the player. This is null if the player was not killed by an
        /// enemy, such as when a player falls into a hole.
        /// </param>
        private void OnPlayerKilled(Enemy killedBy)
        {
            //Player.OnKilled(killedBy);
        }

        /// <summary>
        /// Called when the player reaches the level's exit.
        /// </summary>
        private void OnExitReached()
        {
            //Player.OnReachedExit();
            //exitReachedSound.Play();
            ReachedExit = true;
        }

        /// <summary>
        /// Restores the player to the starting point to try the level again.
        /// </summary>
        public void StartNewLife()
        {
            Player.Reset(start);
        }





        //################################################################################################################################################################//
        //METHODES D'AFFICHAGE DES ELEMENTS




        /// <summary>
        /// Draw everything in the level from background to foreground.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, globalTransformation);
            //for (int i = 0; i <= EntityLayer; ++i)
                //spriteBatch.Draw(layers[i], Vector2.Zero, Color.White);

            DrawTiles(spriteBatch);

            //foreach (Gem gem in gems)
            //gem.Draw(gameTime, spriteBatch);
            //spriteBatch.End();
            //spriteBatch.Begin();
            //spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, globalTransformation);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, null);
            Player.Draw(gameTime, spriteBatch);
            spriteBatch.DrawString(Content.Load<SpriteFont>("Sprites/Font/Hud"), "Level "+ ((NexusDungeonGame)game).levelIndex, Vector2.One, Color.White);


            foreach (Enemy enemy in enemies)
                enemy.Draw(gameTime);

            

            //for (int i = EntityLayer + 1; i < layers.Length; ++i)
            //spriteBatch.Draw(layers[i], Vector2.Zero, Color.White);

        }

        public void ScalePresentationArea()
        {
            //Work out how much we need to scale our graphics to fill the screen
            backbufferWidth = game.GraphicsDevice.PresentationParameters.BackBufferWidth;
            backbufferHeight = game.GraphicsDevice.PresentationParameters.BackBufferHeight;
            float horScaling = backbufferWidth / baseScreenSize.X;
            float verScaling = backbufferHeight / baseScreenSize.Y;
            Vector3 screenScalingFactor = new Vector3(horScaling, verScaling, 1);
            globalTransformation = Matrix.CreateScale(screenScalingFactor);
            System.Diagnostics.Debug.WriteLine("Screen Size - Width[" + game.GraphicsDevice.PresentationParameters.BackBufferWidth + "] Height [" + game.GraphicsDevice.PresentationParameters.BackBufferHeight + "]");
        }

        /// <summary>
        /// Draws each tile in the level.
        /// </summary>
        private void DrawTiles(SpriteBatch spriteBatch)
        {
            // For each tile position
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    // If there is a visible tile in that position
                    Texture2D texture = tiles[x, y].Texture;
                    if (texture != null)
                    {
                        // Draw it in screen space.
                        Vector2 position = new Vector2(x, y) * Tile.Size;
                        spriteBatch.Draw(texture, position, Color.White);
                    }
                }
            }
        }

    }
}
