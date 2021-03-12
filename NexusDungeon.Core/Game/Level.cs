using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
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
        Vector2 baseScreenSize = new Vector2(1024, 576);

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

            LoadTiles(fileStream);
            

            exitReachedSound = Content.Load<Song>("Sprites/Sounds/forest");
        }

        //################################################################################################################################################################//

        


        public void PlayMusic()
        {
            //Musique d'ambiance du niveau
            try
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Play(Content.Load<Song>("Sprites/Sounds/dungeon"));
                MediaPlayer.Volume = (float)0.3;
            }
            catch { }
        }


        #region LoadTiles

        private void LoadTiles(Stream fileStream)
        {
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
                    tiles[x, y] = LoadTile(tileType, x, y);
                }
            }

            // Verify that the level has a beginning and an end.
            if (Player == null)
                throw new NotSupportedException("A level must have a starting point.");
            if (exit == InvalidPosition)
                throw new NotSupportedException("A level must have an exit.");
        }

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

        private Tile LoadExitTile(int x, int y)
        {
            if (exit != InvalidPosition)
                throw new NotSupportedException("A level may only have one exit.");

            exit = GetBounds(x, y).Center;

            return new Tile(Content.Load<Texture2D>("Sprites/sortie"), TileCollision.Passable);
        }

        private Tile LoadStartTile(int x, int y)
        {
            if (Player != null)
                throw new NotSupportedException("A level may only have one starting point.");

            start = RectangleUtils.GetBottomCenter(GetBounds(x, y));
            Player = new Player(this.game,this.spriteBatch,this, start);

            return new Tile(Content.Load<Texture2D>("Sprites/depart"), TileCollision.Passable);
        }


        public void Dispose()
        {
            Content.Unload();
        }

        #endregion

        //################################################################################################################################################################//

        #region Bounds and collision

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



        #endregion

        //################################################################################################################################################################//

        #region Update

        /// <summary>
        /// Updates all objects in the world, performs collision between them,
        /// and handles the time limit with scoring.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            
            if (!ReachedExit)
            {
                if (backbufferHeight != game.GraphicsDevice.PresentationParameters.BackBufferHeight ||
                backbufferWidth != game.GraphicsDevice.PresentationParameters.BackBufferWidth)
                {
                    ScalePresentationArea();
                }

                Player.Update(gameTime);
                

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

        #endregion

        //################################################################################################################################################################//

        #region Draw

        /// <summary>
        /// Draw everything in the level from background to foreground.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, globalTransformation);
            //for (int i = 0; i <= EntityLayer; ++i)
                //spriteBatch.Draw(layers[i], Vector2.Zero, Color.White);

            DrawTiles(spriteBatch);

            //foreach (Gem gem in gems)
                //gem.Draw(gameTime, spriteBatch);

            Player.Draw(gameTime
                //, spriteBatch
                );

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

        #endregion






    }
}
