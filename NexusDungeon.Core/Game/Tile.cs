using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NexusDungeon.Core.Game
{
    public enum TileCollision
    {
        /// <summary>
        /// Une tuile traversable (passable) permet au joueur de se déplacer dessus.
        /// </summary>
        Passable = 0,

        /// <summary>
        /// Une tuile non traversable (impassable) ne permet pas au joueur de la traverser, il en peut donc pas se déplacer dessus
        /// </summary>
        Impassable = 1,
    }

    public class Tile
    {
        //Texture de la tuile
        public Texture2D Texture;

        //Type de collision de la tuile
        public TileCollision Collision;

        //Largeur de la tuile
        public const int Width = 16;

        //Hauteur de la tuile
        public const int Height = 16;

        //Taille de la tuile (en vecteur)
        public static readonly Vector2 Size = new Vector2(Width, Height);

        /// <summary>
        /// Constructeur de la tuile
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="collision"></param>
        public Tile(Texture2D texture, TileCollision collision)
        {
            Texture = texture;
            Collision = collision;
        }
    }
}