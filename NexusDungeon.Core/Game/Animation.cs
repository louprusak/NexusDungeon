using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexusDungeon.Core.Game
{
    class Animation
    {
        /// <summary>
        /// Texture complète de l'animation
        /// </summary>
        public Texture2D Texture { get; }
        
        /// <summary>
        /// Temps d'affichage de chaque frame
        /// </summary>
        public float FrameTime { get; }

        /// <summary>
        /// Doit-on continuer de jouer l'animation lorsque l'on arrive à la fin de la texture
        /// </summary>
        public bool IsLooping { get; }

        /// <summary>
        /// Nombre d'animations sur la texture (la largeur de chaque frame est de 32 pixel)
        /// </summary>
        public int FrameCount
        {
            get { return Texture.Width / 32; }
        }

        /// <summary>
        /// Largeur d'une frame de l'animation
        /// </summary>
        public int FrameWidth
        {
            get { return Texture.Height; }
        }

        /// <summary>
        /// Hauteur d'une frame de l'animation
        /// </summary>
        public int FrameHeight
        {
            get { return Texture.Height; }
        }

        //################################################################################################################################################################//
        //CONSTRUCTEUR

        public Animation(Texture2D texture, float frameTime, bool isLooping)
        {
            this.Texture = texture;
            this.FrameTime = frameTime;
            this.IsLooping = isLooping;
        }
    }
}
