using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexusDungeon.Core.Game
{
    class Player : GameObject
    {
        private Texture2D _texture;
        public Vector2 Position { get; set; } = Vector2.One;
        public float _speed;

        public Player(Microsoft.Xna.Framework.Game game, SpriteBatch spriteBatch) : base(game, spriteBatch)
        {
            LoadContent();
        }

        public override void Initialize()
        {
            base.Initialize();
            _speed = 5;
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            _texture = Game.Content.Load<Texture2D>("Sprites/guy");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            var keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                Position = Vector2.Add(Position, new Vector2(-5, 0));
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                Position = Vector2.Add(Position, new Vector2(5, 0));
            }
            if (keyboardState.IsKeyDown(Keys.Up))
            {
                Position = Vector2.Add(Position, new Vector2(0, -5));
            }
            if (keyboardState.IsKeyDown(Keys.Down))
            {
                Position = Vector2.Add(Position, new Vector2(0, 5));
            }
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            _spriteBatch.Draw(_texture, Position, Color.White);
        }


        public static implicit operator PlayerIndex(Player v)
        {
            throw new NotImplementedException();
        }
    }
}
