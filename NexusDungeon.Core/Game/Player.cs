using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
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

        //Sounds
        private Song _walk_sound;

        //Bounds
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
        public Vector2 Position { get; set; }
        public Vector2 NextPosition { get; set; }

        //Stats
        public float _speed;
        public Level Level
        {
            get;
        }
        public Microsoft.Xna.Framework.Game game;
        public SpriteBatch spriteBatch;


        //Constructeur
        public Player(Microsoft.Xna.Framework.Game game, SpriteBatch spriteBatch) : base(game, spriteBatch)
        {
            this.game = game;
            this.spriteBatch = spriteBatch;
            LoadContent();
           
            Reset(game);
        }

        /*public Player(Level level, Vector2 position) : base(game, spriteBatch)
        {
            this.Level = level;

            LoadContent();

            Reset(position);
        }*/

        //Méthodes Monogame
        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            _idle_Animation = new Animation(Game.Content.Load<Texture2D>("Sprites/Player/idle"), 0.2f,true);
            _walk_Top_Animation = new Animation(Game.Content.Load<Texture2D>("Sprites/Player/top"), 0.06f, true);
            _walk_Bot_Animation = new Animation(Game.Content.Load<Texture2D>("Sprites/Player/bot"), 0.06f, true);
            _walk_Left_Animation = new Animation(Game.Content.Load<Texture2D>("Sprites/Player/left"), 0.06f, true);
            _walk_Right_Animation = new Animation(Game.Content.Load<Texture2D>("Sprites/Player/right"), 0.06f, true);

            _walk_sound = Game.Content.Load<Song>("Sprites/Sounds/footsteps");
           
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            NextPosition = Position;
            var keyboardState = Keyboard.GetState();
            if (keyboardState.GetPressedKeyCount() == 0)
            {
                animationPlayer.PlayAnimation(_idle_Animation);
                
            }
            else
            {
               
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    NextPosition = Vector2.Add(Position, new Vector2(-(_speed), 0));
                    animationPlayer.PlayAnimation(_walk_Left_Animation);
                    
                }
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    NextPosition = Vector2.Add(Position, new Vector2(_speed, 0));
                    animationPlayer.PlayAnimation(_walk_Right_Animation);
                   
                }
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    NextPosition = Vector2.Add(Position, new Vector2(0, -(_speed)));
                    animationPlayer.PlayAnimation(_walk_Top_Animation);
                    
                }
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    NextPosition = Vector2.Add(Position, new Vector2(0, (_speed)));
                    animationPlayer.PlayAnimation(_walk_Bot_Animation);
                    
                }
            }

        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            
            animationPlayer.Draw(gameTime, _spriteBatch, Position, flip);
        }

        /// <summary>
        /// Reset la position et les stats du joueur
        /// </summary>
        /// <param name="game">Instance actuelle du jeu</param>
        public void Reset(Microsoft.Xna.Framework.Game game)
        {
            animationPlayer.PlayAnimation(_idle_Animation);

            int tmpx = game.Window.ClientBounds.Width / 2;
            int tmpy = game.Window.ClientBounds.Height / 2;
            Position = new Vector2(tmpx, tmpy);

            _speed = 5;
        }

        public void Reset(Vector2 position)
        {
            Position = position;
            
            animationPlayer.PlayAnimation(_idle_Animation);
        }

        public static implicit operator PlayerIndex(Player v)
        {
            throw new NotImplementedException();
        }
    }
}
