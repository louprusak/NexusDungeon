using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexusDungeon.Core.Game
{
    public class GameObject : DrawableGameComponent
    {
        //SpriteBatch de l'objet
        protected readonly SpriteBatch _spriteBatch;

        //################################################################################################################################################################//
        //CONSTRUCTEUR

        public GameObject(Microsoft.Xna.Framework.Game game, SpriteBatch spriteBatch) : base(game)
        {
            _spriteBatch = spriteBatch;
        }
    }
}
