using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.IO;

namespace NexusDungeon.Core.Game
{
    /*
    class Level : IDisposable
    {
        
        //Entités dans le niveau
        public Player Player { get; set; }
        private List<Enemy> enemies = new List<Enemy>();

        //Positions
        private Vector2 start;
        private Point exit;

        //Sounds
        private SoundEffect music;
        private SoundEffect exitReachedSound;

        //Propriétés
        private Tile[,] tiles;
        public bool ReachedExit { get; set; }
        public ContentManager Content { get; set; }
        public int Height
        {
            //get { return 0 /*tiles.GetLength(1); }
        }
        public int Width
        {
            get;
            set;
        }

        //Constructeur
        public Level(Stream fileStream)
        {
            LoadTiles(fileStream);
            exitReachedSound = Content.Load<SoundEffect>("Sounds/forest");
        }

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
                    return new Tile(null, TileCollision.Passable);
                //Mur
                case '#':
                    return new Tile("Mur", TileCollision.Impassable);
                //Sol
                case '-':
                    return new Tile("Sol", TileCollision.Passable);
                //Enemis
                case 'A':
                    return new Tile(x, y, "MonsterA");
                case 'B':
                    return new Tile(x, y, "MonsterB");
                case 'C':
                    return new Tile(x, y, "MonsterC");
                case 'D':
                    return new Tile(x, y, "MonsterD");
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        //Méthodes









    }*/
}
