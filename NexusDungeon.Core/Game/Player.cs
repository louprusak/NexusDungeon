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
        // Animations
        private Animation _idle_Animation;
        private Animation _walk_Top_Animation;
        private Animation _walk_Bot_Animation;
        private Animation _walk_Left_Animation;
        private Animation _walk_Right_Animation;

        private AnimationPlayer animationPlayer;
        private SpriteEffects flip = SpriteEffects.None;

        private Rectangle localBounds;

        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - animationPlayer.Origin.X) + localBounds.X;
                int top = (int)Math.Round(Position.Y - animationPlayer.Origin.Y) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }

        //Position
        public Vector2 Position { get; set; } = Vector2.One;
        public Vector2 NextPosition { get; set; }
        public float _speed;

        //Constructeur
        public Player(Microsoft.Xna.Framework.Game game, SpriteBatch spriteBatch) : base(game, spriteBatch)
        {
            LoadContent();
            animationPlayer.PlayAnimation(_idle_Animation);
        }

        //Méthodes Monogame
        public override void Initialize()
        {
            base.Initialize();
            _speed = 5;
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            _idle_Animation = new Animation(Game.Content.Load<Texture2D>("Sprites/Player/idle"), 0.2f,true);
            _walk_Top_Animation = new Animation(Game.Content.Load<Texture2D>("Sprites/Player/top"), 0.06f, true);
            _walk_Bot_Animation = new Animation(Game.Content.Load<Texture2D>("Sprites/Player/bot"), 0.06f, true);
            _walk_Left_Animation = new Animation(Game.Content.Load<Texture2D>("Sprites/Player/left"), 0.06f, true);
            _walk_Right_Animation = new Animation(Game.Content.Load<Texture2D>("Sprites/Player/right"), 0.06f, true);
           
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            var keyboardState = Keyboard.GetState();
            if (keyboardState.GetPressedKeyCount() == 0)
            {
                animationPlayer.PlayAnimation(_idle_Animation);
            }
            else
            {
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    Position = Vector2.Add(Position, new Vector2(-5, 0));
                    animationPlayer.PlayAnimation(_walk_Left_Animation);
                }
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    Position = Vector2.Add(Position, new Vector2(5, 0));
                    animationPlayer.PlayAnimation(_walk_Right_Animation);
                }
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    Position = Vector2.Add(Position, new Vector2(0, -5));
                    animationPlayer.PlayAnimation(_walk_Top_Animation);
                }
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    Position = Vector2.Add(Position, new Vector2(0, 5));
                    animationPlayer.PlayAnimation(_walk_Bot_Animation);
                }
            }

        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            
            animationPlayer.Draw(gameTime, _spriteBatch, Position, flip);
        }


        public static implicit operator PlayerIndex(Player v)
        {
            throw new NotImplementedException();
        }
    }
}
