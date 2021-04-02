using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;

namespace NexusDungeon.Core.Game
{
    public class Player
    {
        // Animations
        private Animation _idle_Animation;
        private Animation _walk_Top_Animation;
        private Animation _walk_Bot_Animation;
        private Animation _walk_Left_Animation;
        private Animation _walk_Right_Animation;
        private Animation _attack_Right_Animation;
        private Animation _attack_Top_Animation;
        private Animation _attack_Left_Animation;
        private Animation _attack_Bot_Animation;
        private AnimationPlayer animationPlayer;
        private SpriteEffects flip = SpriteEffects.None;

        //Sounds
        private Song _walk_sound;

        //Bordures et collisions
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
        public Vector2 PositionLevel { get; set; }
        public Vector2 NextPosition { get; set; }
        public Vector2 NextPositionLevel { get; set; }

        //Stats
        public float _speed;
        public Level Level = null;
        public Microsoft.Xna.Framework.Game game;
        
        public SpriteBatch spriteBatch;



        //################################################################################################################################################################//
        //Constructeur


        public Player(Microsoft.Xna.Framework.Game game, SpriteBatch spriteBatch)
        {
            this.game = game;
            this.spriteBatch = spriteBatch;
            LoadContent();
           
            Reset(game);
        }

        public Player(Level level, Vector2 position)
        {
            this.Level = level;

            LoadContent();
            System.Diagnostics.Debug.WriteLine("Constructeur player level, position :"+ position);
            
            Reset(position);
        }



        //################################################################################################################################################################//
        //METHODES


        protected void LoadContent()
        {
            

            if (game != null)
            {
                _idle_Animation = new Animation(game.Content.Load<Texture2D>("Sprites/Player/idle"), 0.2f, true);
                _walk_Top_Animation = new Animation(game.Content.Load<Texture2D>("Sprites/Player/top"), 0.06f, true);
                _walk_Bot_Animation = new Animation(game.Content.Load<Texture2D>("Sprites/Player/bot"), 0.06f, true);
                _walk_Left_Animation = new Animation(game.Content.Load<Texture2D>("Sprites/Player/left"), 0.06f, true);
                _walk_Right_Animation = new Animation(game.Content.Load<Texture2D>("Sprites/Player/right"), 0.06f, true);
                _attack_Right_Animation = new Animation(game.Content.Load<Texture2D>("Sprites/Player/attack_right"), 0.06f, true);
                _attack_Left_Animation = new Animation(game.Content.Load<Texture2D>("Sprites/Player/attack_left"), 0.06f, true);
                _attack_Top_Animation = new Animation(game.Content.Load<Texture2D>("Sprites/Player/attack_top"), 0.06f, true);
                _attack_Bot_Animation = new Animation(game.Content.Load<Texture2D>("Sprites/Player/attack_bot"), 0.06f, true);
            }
           else if(Level != null)
            {
                _idle_Animation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/idle"), 0.2f, true);
                _walk_Top_Animation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/top"), 0.06f, true);
                _walk_Bot_Animation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/bot"), 0.06f, true);
                _walk_Left_Animation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/left"), 0.06f, true);
                _walk_Right_Animation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/right"), 0.06f, true);
                _attack_Right_Animation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/attack_right"), 0.06f, true);
                _attack_Left_Animation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/attack_left"), 0.06f, true);
                _attack_Top_Animation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/attack_top"), 0.06f, true);
                _attack_Bot_Animation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/attack_bot"), 0.06f, true);
            }
            else
            {
                throw new NotSupportedException("Un player doit avoir un contexte. Soit un game, soit un level.");
            }

            // Calculate bounds within texture size.            
            int width = (int)(_idle_Animation.FrameWidth * 0.4); // ???
            int left = (_idle_Animation.FrameWidth - width) / 2;
            int height = (int)(_idle_Animation.FrameHeight * 0.8);
            int top = _idle_Animation.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);

        }

        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            if(Level == null)
            {
                Position = DeplacementHub(keyboardState);
            }
            else{
                PositionLevel = DeplacementLevel(keyboardState);
            }

            

        }

        /*if (keyboardState.IsKeyDown(Keys.Space))
                    {
                        animationPlayer.PlayAnimation(_attack_Left_Animation);
                    }*/

        private Vector2 DeplacementHub(KeyboardState keyboardState)
        {
            

            if (keyboardState.GetPressedKeyCount() == 0)
            {
                animationPlayer.PlayAnimation(_idle_Animation);
                return Position;
            }
            else
            {
                Vector2 NextPositionTmp = Position;
                System.Diagnostics.Debug.WriteLine("[hub] "+ this);
                System.Diagnostics.Debug.WriteLine("[hub] //DEBUT UPDATE// : Position - X = " + Position.X + " | Y= " + Position.Y);

                if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.Q))
                {
                    NextPositionTmp = Vector2.Add(Position, new Vector2(-(_speed), 0));
                    animationPlayer.PlayAnimation(_walk_Left_Animation);
                }
                if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
                {
                    NextPositionTmp = Vector2.Add(Position, new Vector2(_speed, 0));
                    animationPlayer.PlayAnimation(_walk_Right_Animation);
                }
                if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.Z))
                {
                    NextPositionTmp = Vector2.Add(Position, new Vector2(0, -(_speed)));
                    animationPlayer.PlayAnimation(_walk_Top_Animation);
                }
                if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
                {
                    NextPositionTmp = Vector2.Add(Position, new Vector2(0, (_speed)));
                    animationPlayer.PlayAnimation(_walk_Bot_Animation);
                }

                /*if (Level != null)
                    HandleCollisions();*/

                System.Diagnostics.Debug.WriteLine("[hub] //FIN UPDATE NEXT POSITION// : Position - X = " + NextPositionTmp.X + " | Y= " + NextPositionTmp.Y);
                System.Diagnostics.Debug.WriteLine("[hub] //FIN UPDATE POSITION// : Position - X = " + Position.X + " | Y= " + Position.Y);
                return NextPositionTmp;
            }
        }

        private Vector2 DeplacementLevel(KeyboardState keyboardState)
        {


            if (keyboardState.GetPressedKeyCount() == 0)
            {
                animationPlayer.PlayAnimation(_idle_Animation);
                return PositionLevel;
            }
            else
            {
                Vector2 NextPositionTmp = PositionLevel;
                System.Diagnostics.Debug.WriteLine("[level] " + this);
                System.Diagnostics.Debug.WriteLine("[level] //DEBUT UPDATE// : Position - X = " + PositionLevel.X + " | Y= " + PositionLevel.Y);

                if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.Q))
                {
                    NextPositionTmp = Vector2.Add(PositionLevel, new Vector2(-(_speed), 0));
                    animationPlayer.PlayAnimation(_walk_Left_Animation);
                }
                if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
                {
                    NextPositionTmp = Vector2.Add(PositionLevel, new Vector2(_speed, 0));
                    animationPlayer.PlayAnimation(_walk_Right_Animation);
                }
                if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.Z))
                {
                    NextPositionTmp = Vector2.Add(PositionLevel, new Vector2(0, -(_speed)));
                    animationPlayer.PlayAnimation(_walk_Top_Animation);
                }
                if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
                {
                    NextPositionTmp = Vector2.Add(PositionLevel, new Vector2(0, (_speed)));
                    animationPlayer.PlayAnimation(_walk_Bot_Animation);
                }

                /*if (Level != null)
                    HandleCollisions();*/

                System.Diagnostics.Debug.WriteLine("[level] //FIN UPDATE NEXT POSITION// : Position - X = " + NextPositionTmp.X + " | Y= " + NextPositionTmp.Y);
                System.Diagnostics.Debug.WriteLine("[level] //FIN UPDATE POSITION// : Position - X = " + PositionLevel.X + " | Y= " + PositionLevel.Y);
                return NextPositionTmp;
            }
        }


        public void HandleCollisions()
        {
            Rectangle bounds = BoundingRectangle;

            // Get the player's bounding rectangle and find neighboring tiles.
            int leftTile = (int)Math.Floor((float)bounds.Left / Tile.Width);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.Width)) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) - 1;

            // For each potentially colliding tile,
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    // If this tile is collidable,
                    TileCollision collision = Level.GetCollision(x, y);
                    if (collision != TileCollision.Passable)
                    {
                        // Determine collision depth (with direction) and magnitude.
                        Rectangle tileBounds = Level.GetBounds(x, y);
                        Vector2 depth = RectangleUtils.GetIntersectionDepth(bounds, tileBounds);
                        if (depth != Vector2.Zero)
                        {
                            float absDepthX = Math.Abs(depth.X);
                            float absDepthY = Math.Abs(depth.Y);

                            // Resolve the collision along the shallow axis.
                            if (absDepthY < absDepthX || collision == TileCollision.Platform)
                            {
                                

                                // Ignore platforms, unless we are on the ground.
                                if (collision == TileCollision.Impassable)
                                {
                                    // Resolve the collision along the Y axis.
                                    Position = new Vector2(Position.X, Position.Y + depth.Y);

                                    // Perform further collisions with the new bounds.
                                    bounds = BoundingRectangle;
                                }
                            }
                            else if (collision == TileCollision.Impassable) // Ignore platforms.
                            {
                                // Resolve the collision along the X axis.
                                Position = new Vector2(Position.X + depth.X, Position.Y);

                                // Perform further collisions with the new bounds.
                                bounds = BoundingRectangle;
                            }
                        }
                    }
                }
            }
        }


        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if(Level == null)
            {
                animationPlayer.Draw(gameTime, spriteBatch, Position, flip);
            }
            else
            {
                animationPlayer.Draw(gameTime, spriteBatch, PositionLevel, flip);
            }
            
        }

        /// <summary>
        /// Reset la position et les stats du joueur
        /// </summary>
        /// <param name="game">Instance actuelle du jeu</param>
        public void Reset(Microsoft.Xna.Framework.Game game)
        {
            animationPlayer.PlayAnimation(_idle_Animation);

            //int tmpx = game.Window.ClientBounds.Width / 2;
            //int tmpy = game.Window.ClientBounds.Height / 2;
            //Position = new Vector2(tmpx, tmpy);
            //Position = new Vector2(1256, 436);
            Position = new Vector2(996, 716);
            //Position = new Vector2(291, 251);
            

            _speed = 5;
        }

        public void Reset(Vector2 position)
        {
            PositionLevel = position;
            _speed = 3;
            animationPlayer.PlayAnimation(_idle_Animation);
        }

        
    }
}
