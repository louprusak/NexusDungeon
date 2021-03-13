using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexusDungeon.Core.Game
{
    struct AnimationPlayer
    {
        /// <summary>
        /// Get l'animation courrante.
        /// </summary>
        private Animation Animation { get; set; }

        /// <summary>
        /// Index de la frame courrante dans l'animation.
        /// </summary>
        private int FrameIndex { get; set; }

        /// <summary>
        /// Durée en secondes pendant laquelle l'image actuelle a été affichée.
        /// </summary>
        private float time;

        /// <summary>
        /// Origine de la frame courrante (en bas au centre).
        /// </summary>
        public Vector2 Origin
        {
            get { return new Vector2(Animation.FrameWidth / 2.0f, Animation.FrameHeight); }
        }

        /// <summary>
        /// Lit une animation ou la poursuit si elle est en cours.
        /// </summary>
        public void PlayAnimation(Animation animation)
        {
            // Si l'animation est déjà jouée
            if (Animation == animation)
                return;

            // Lecture de l'animation
            this.Animation = animation;
            this.FrameIndex = 0;
            this.time = 0.0f;
        }

        /// <summary>
        /// Avance la position temporelle et dessine l'image actuelle de l'animation.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects)
        {
            if (Animation == null)
                throw new NotSupportedException("No animation is currently playing.");

            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while (time > Animation.FrameTime)
            {
                time -= Animation.FrameTime;

                // Avancer l'index de la frame et looper ou continuer en fonction
                if (Animation.IsLooping)
                {
                    FrameIndex = (FrameIndex + 1) % Animation.FrameCount;
                }
                else
                {
                    FrameIndex = Math.Min(FrameIndex + 1, Animation.FrameCount - 1);
                }
            }

            // Calculez le rectangle source de l'image actuelle.
            Rectangle source = new Rectangle(FrameIndex * 32, 0, 32, Animation.Texture.Height);

            // Dessine la frame courrante
            spriteBatch.Draw(Animation.Texture, position, source, Color.White, 0.0f, Origin, 4.0f, spriteEffects, 0.0f);
        }
    }
}
