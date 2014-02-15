using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Story
{
    class Entity
    {
        #region Variables
        //Flip effect
        protected SpriteEffects Flip = SpriteEffects.None;
        // Animations
        protected Animation IdleAnimation;
        protected Animation RunAnimation;
        protected Animation JumpAnimation;
        protected Animation GlideAnimation;
        protected Animation AttackAnimation;
        protected Animation UnderwaterAnimation;
        protected Animation CelebrateAnimation;
        protected Animation DeathAnimation;
        protected AnimationPlayer Sprite;
        // Sounds
        protected SoundEffect DeathSound;
        protected SoundEffect JumpSound;
        protected SoundEffect FallSound;

        private ParticleEmitter DeathEmitter;   //Emits red (blood) particles upon death
        private Vector2 DeathCoords;            //Coords upon death (used to offset drawing)

        //Projectiles the entity owns
        protected List<Projectile> Projectiles = new List<Projectile>();

        //Collision related
        private bool CeilingCollision = false;
        private CollisionObject LastTouchedObj;
        private LinePrimitive CollisionBoxLine;

        private const int InstaKill = 999; //0xDEAD; //Value that represents instakill

        public bool IsAlive;
        public int Health = 10;
        public float MindPower = 8000.0f;
        public float BlinkTimer = 0.0f;

        protected bool Sink = false;
        protected bool Glide = false;
        protected bool GlideFatigued = false;
        protected bool IsAttacking = false;
        protected bool Underwater = false;
        protected bool BreathingUW = false;

        //PHYSICS
        protected float Speed;
        protected Vector2 Movement; //Current analog movement
        public Vector2 Position;
        public Vector2 Velocity;
        //STANDARD
        protected float GroundDragFactor = .58f; //.58 normal
        protected float MaxMoveSpeed = 360.0f;
        protected float Rotation = 0.0f;
        protected float MaxJumpTime = 0.40f;
        private float MoveAcceleration = 14000.0f;
        private float AirDragFactor = 0.65f;
        private const float RotationSpeed = 0.045f;
        private const float JumpLaunchVelocity = -4000.0f;
        private const float GravityAcceleration = 4000.0f;
        private const float MaxGlideFallSpeed = 150.0f;
        private const float MaxFallSpeed = 600.0f;
        private const float JumpControlPower = 0.14f;
        //UNDERWATER
        private const float UnderwaterJumpTime = 0.80f;
        private const float UnderwaterJumpLaunchVelocity = -3000.0f;
        private const float UnderwaterGravityAcceleration = 2000.0f;
        private const float UnderwaterMaxFallSpeed = 200.0f;
        private const float UnderwaterJumpControlPower = 0.07f;
        //CURRENT STATE
        private float ActualMaxJumpTime;
        private float ActualJumpLaunchVelocity;
        private float ActualGravityAcceleration;
        private float ActualMaxFallSpeed;
        private float ActualJumpControlPower;
        //BOUNCE/HOVER
        public float Bounce = 0.0f;
        public float HoverDistance = 0.0f;
        private float MaxHoverDistance = 64.0f;
        private float MaxSinkDistance = 24.0f;
        private float ActualHoverDistance = 0.0f;
        private float HoverSpeed = 320.0f;
        protected bool CanHover = false;
        //JUMP STATE/INPUT
        protected bool JumpButtonHeld; //Used to start/contribute to a jump
        protected bool WasJumping;
        protected float JumpTime;
        public bool IsOnGround;

        public Level Level
        {
            get { return level; }
        } protected Level level;

        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - LocalBounds.Width / 2);
                int top = (int)Math.Round(Position.Y - HoverDistance - LocalBounds.Height + LocalBounds.Top);

                return new Rectangle(left, top, LocalBounds.Width, LocalBounds.Height);
            }
        } protected Rectangle LocalBounds;


        //For NPC use only
        protected int NPCDirection = 1;

        #endregion

        #region Initialization

        public Entity(Level level, Vector2 Position)
        {
            this.level = level;

            //Load death emitter
            DeathEmitter = new ParticleEmitter(new Vector2(0, 0),
new Color(255, 0, 255, 255), new Vector2(0, 0), new Vector2(Main.BackBufferWidth, Main.BackBufferHeight), new Vector2(-0.8f, -0.8f), new Vector2(0.8f, 0.8f),
new Vector2(-16, -16), new Vector2(16, 16), 1500, 20, 32, 0f, .75f, 0.0009f, 0.8f, false, 500.0f);

            Reset(Position);
        }

        public void LoadContent()
        {
            //Death emitter particle
            DeathEmitter.LoadContent(Level.Content.Load<Texture2D>("Particles/DeathParticle"));
        }

        #endregion

        #region Update

        protected void Update(GameTime GameTime)
        {
            //Update blink timer
            if (BlinkTimer > 0.0f)
                BlinkTimer -= GameTime.ElapsedGameTime.Milliseconds;
            if (BlinkTimer < 0.0f)
                BlinkTimer = 0.0f;

            if (Underwater)
            {
                //Underwater physics
                ActualGravityAcceleration = UnderwaterGravityAcceleration;
                ActualJumpControlPower = UnderwaterJumpControlPower;
                ActualMaxFallSpeed = UnderwaterMaxFallSpeed;
                ActualMaxJumpTime = UnderwaterJumpTime;
                ActualJumpLaunchVelocity = UnderwaterJumpLaunchVelocity;
            }
            else
            {
                //Normal physics
                ActualGravityAcceleration = GravityAcceleration;
                ActualJumpControlPower = JumpControlPower;
                ActualMaxJumpTime = MaxJumpTime;
                ActualJumpLaunchVelocity = JumpLaunchVelocity;

                //Slow down fall speed when gliding
                if (Glide)
                    ActualMaxFallSpeed = MaxGlideFallSpeed;
                else
                    ActualMaxFallSpeed = MaxFallSpeed;
            }

            if (!IsAlive)
                DeathEmitter.Update(GameTime);
        }

        #endregion

        #region Physics & Collisions

        public void ApplyPhysics(GameTime GameTime, bool IsPlayer)
        {
            float Elapsed = (float)GameTime.ElapsedGameTime.TotalSeconds;

            Vector2 PreviousPosition = Position;

            Velocity.X += Movement.X * MoveAcceleration * Elapsed;
            Velocity.Y = MathHelper.Clamp(Velocity.Y + ActualGravityAcceleration * Elapsed, -ActualMaxFallSpeed, ActualMaxFallSpeed);
            Velocity.Y = DoJump(Velocity.Y, GameTime);

            //Slowly restore entity rotation mid jump
            if (!IsOnGround || LastTouchedObj == null || !LastTouchedObj.IsTriangle)
            {
                //Adjust depending on what direction entity is rotated
                if (Rotation < 0)
                {
                    Rotation += RotationSpeed;
                    if (Rotation > 0)
                        Rotation = 0;
                }
                if (Rotation > 0)
                {
                    Rotation -= RotationSpeed;
                    if (Rotation < 0)
                        Rotation = 0;
                }
            }

            //Apply drag in X direction
            if (IsOnGround)
                Velocity.X *= GroundDragFactor;
            else
                Velocity.X *= AirDragFactor;

            //Prevent the entity from running faster than his top speed.            
            Velocity.X = MathHelper.Clamp(Velocity.X, -MaxMoveSpeed, MaxMoveSpeed);

            //Apply velocity.
            Position += Velocity * Elapsed;

            // Deal with any collision
            //Actual collision for entity
            HandleCollisions(GameTime, false, IsPlayer);
            //Collision to adjust entity position for hover distance
            if (HoverDistance != 0)
                HandleCollisions(GameTime, true, IsPlayer);

            //Check if player is 'sinking' (floating lower to ground)
            if (Sink)
                ActualHoverDistance = MaxSinkDistance;
            else
                ActualHoverDistance = MaxHoverDistance;

            //Apply floating due to hover
            if (CanHover && IsAlive)
            {
                if (HoverDistance < ActualHoverDistance)
                {
                    //Float upward if not colliding with a ceiling
                    if (!CeilingCollision)
                        HoverDistance += HoverSpeed * Elapsed;
                    if (HoverDistance > ActualHoverDistance)
                        HoverDistance = ActualHoverDistance;
                }

                else if (HoverDistance > ActualHoverDistance)
                {
                    HoverDistance -= HoverSpeed * Elapsed;
                    if (HoverDistance < ActualHoverDistance)
                        HoverDistance = ActualHoverDistance;
                }

                if (HoverDistance < 0)
                    HoverDistance = 0;
            }


            // If the collision stopped us from moving, reset the velocity to zero.
            if (Position.X == PreviousPosition.X)
                Velocity.X = 0;

            if (Position.Y == PreviousPosition.Y)
                Velocity.Y = 0;

        }

        private bool CeilingCollisionPrevious;
        private void HandleCollisions(GameTime GameTime, bool HoverCollision, bool IsPlayer)
        {
            //When walking down a ramp the entity constantly flies off by a few pixels.
            //Simply increase the range for the collision check by a small amount (4)
            //and grab the player if they are withen that range
            const float RampBufferY = 4.0f;

            //Buffer to allow for small stairs or uneven ground
            const float TopBufferY = 24.0f;
            float InitialHoverDistance = HoverDistance;
            if (HoverCollision)
                Position.Y -= InitialHoverDistance;
            else
                IsOnGround = false;

            Underwater = false;
            CeilingCollisionPrevious = CeilingCollision;
            CeilingCollision = false;

            //These are used to check distances to each of the sides of a triangle
            float Left;
            float Right;
            float Top;
            float Bottom;

            float EntityWidth = LocalBounds.Width;
            float EntityHeight = LocalBounds.Height;

            float Slope;
            float IntendedY;
            bool IsRight;
            bool IsTriangle;

            CollisionObject OldLastTouched = LastTouchedObj;
            //NewObj = false;

            //foreach (CollisionObject collisionObj in level.CollisionObjects)
            for (int ecx = 0; ecx < level.CollisionObjects.Count; ecx++)
            {
                //Check if able to collide with object
                if (!(IsPlayer && level.CollisionObjects[ecx].IsNPCOnly))
                {
                    IsRight = false;
                    IsTriangle = false;
                    bool Inside = false;
                    bool UpsideDown = false; //For upside down triangles
                    IntendedY = level.CollisionObjects[ecx].Y;
                    Slope = 0.0f;

                    //Set up variables for a rectangle object
                    if (!level.CollisionObjects[ecx].IsTriangle)
                    {
                        //Calculate position of each side
                        Left = level.CollisionObjects[ecx].X - Main.BackBufferWidth / 2 - EntityWidth / 2;
                        Right = level.CollisionObjects[ecx].X - Main.BackBufferWidth / 2 + level.CollisionObjects[ecx].Width + EntityWidth / 2;
                        Top = level.CollisionObjects[ecx].Y;
                        Bottom = level.CollisionObjects[ecx].Y + level.CollisionObjects[ecx].Height + EntityHeight;

                        //Test if inside shape
                        if (Position.X > Left && Position.X < Right && Position.Y > Top && Position.Y < Bottom)
                            Inside = true;
                    }

                    //Set up variables for a triangle object
                    else
                    {
                        IsTriangle = true;
                        //Check direction of triangle
                        if (level.CollisionObjects[ecx].Coords[3].X > level.CollisionObjects[ecx].Coords[2].X)
                            IsRight = true;
                        float Tempx;
                        float Tempy;

                        //Calculate position of each side
                        if (IsRight)
                        {
                            Left = level.CollisionObjects[ecx].X - Main.BackBufferWidth / 2;
                            Right = level.CollisionObjects[ecx].X - Main.BackBufferWidth / 2 + level.CollisionObjects[ecx].Width;
                        }
                        else
                        {
                            Left = level.CollisionObjects[ecx].X - Main.BackBufferWidth / 2 + level.CollisionObjects[ecx].Width;
                            Right = level.CollisionObjects[ecx].X - Main.BackBufferWidth / 2;
                        }

                        if (level.CollisionObjects[ecx].Coords[1].Y < level.CollisionObjects[ecx].Coords[3].Y)
                        {
                            Tempx = level.CollisionObjects[ecx].Coords[3].X;
                            Tempy = level.CollisionObjects[ecx].Coords[3].Y;
                        }
                        else //Triangle is upside down and thus functions as a roof
                        {
                            Tempx = level.CollisionObjects[ecx].Coords[1].X;
                            Tempy = level.CollisionObjects[ecx].Coords[1].Y;
                            UpsideDown = true;
                        }
                        //Calculate slope of triangle
                        Top = level.CollisionObjects[ecx].Y - TopBufferY;
                        Bottom = level.CollisionObjects[ecx].Y + level.CollisionObjects[ecx].Height + EntityHeight;
                        Slope = (level.CollisionObjects[ecx].Coords[1].Y - level.CollisionObjects[ecx].Coords[3].Y) / (level.CollisionObjects[ecx].Coords[1].X - level.CollisionObjects[ecx].Coords[3].X);
                        float Yint = -(Slope * Tempx - Tempy);
                        IntendedY = Slope * Position.X + Yint;

                        //Test if inside rectangle
                        if (Position.X > Left && Position.X < Right && Position.Y > Top && Position.Y < Bottom)
                        {
                            //Set new top position that relates to triangle shape instead of rectangle
                            if (!UpsideDown)
                                Top = IntendedY - RampBufferY;
                            else
                                Bottom = IntendedY;

                            //Test if inside triangle within rectangle
                            if (Position.Y >= Top)
                                Inside = true;
                        }

                    }

                    //Waterline test
                    if ((int)((Position.Y) / Tile.Height) > level.WaterLine)
                        Underwater = true;

                    if (Inside && level.CollisionObjects[ecx].Damaging && IsPlayer && Main.GodMode)
                    {
                        //Godmode vs damaging collision box, do nothing.
                    }
                    //Damaging collision object (Can't modify player position)
                    else if (Inside && level.CollisionObjects[ecx].Damaging)
                    {
                        if (HoverCollision)
                            GetHit(level.CollisionObjects[ecx].Damage);
                        else if (!CanHover)
                            GetHit(level.CollisionObjects[ecx].Damage);
                    }
                    //Liquid collision object (Can't modify player position)
                    else if (Inside && level.CollisionObjects[ecx].Liquid)
                        Underwater = true;
                    //Normal collision object
                    else if (Inside)
                    {
                        //Calculate distances to each side
                        Left = Math.Abs(Position.X - Left);
                        Right = Math.Abs(Position.X - Right);
                        Top = Math.Abs(Position.Y - Top);
                        Bottom = Math.Abs(Position.Y - Bottom);

                        if (UpsideDown)
                        {
                            float Temp = Top;
                            //top = bottom;
                            //bottom = temp;
                        }

                        //Set up last touched object
                        if (!level.CollisionObjects[ecx].IsNPCOnly)
                        {
                            if (level.CollisionObjects[ecx] != LastTouchedObj)
                            {
                                //NewObj = true;
                                LastTouchedObj = level.CollisionObjects[ecx];
                            }
                        }
                        //Top closest test
                        if (!UpsideDown && (Top < Left || IsTriangle && !IsRight) && (Top < Right || IsTriangle && IsRight) && Top < Bottom || (Top <= TopBufferY && !UpsideDown))
                        {
                            //Always perform collisions unless it is a passable collision, in which case
                            //Only collide if the player is moving downward (or not at all)
                            if (!level.CollisionObjects[ecx].Passable || (level.CollisionObjects[ecx].Passable && Velocity.Y >= 0))
                            {
                                if (CanHover && !CeilingCollisionPrevious) //Smooths out transitioning over bumps
                                    HoverDistance = Math.Abs(IntendedY - (Position.Y - HoverDistance));

                                Position.Y = IntendedY;
                                Velocity.Y = 0;
                                IsOnGround = true;

                                //Collision with moving object
                                if (level.CollisionObjects[ecx].Moving)
                                {
                                    if (Velocity.X <= 1 && Velocity.X >= level.CollisionObjects[ecx].VelocityX * level.CollisionObjects[ecx].DirectionX)
                                        Position.X += level.CollisionObjects[ecx].Movement.X; //< 1 away
                                    else if (Velocity.X >= -1 && Velocity.X <= level.CollisionObjects[ecx].VelocityX * level.CollisionObjects[ecx].DirectionX)
                                        Position.X += level.CollisionObjects[ecx].Movement.X;

                                    if (Velocity.Y < 1)
                                        Position.Y += level.CollisionObjects[ecx].Movement.Y;
                                }

                                if (Rotation < Slope)
                                {
                                    Rotation += RotationSpeed;
                                    if (Rotation > Slope)
                                        Rotation = Slope;
                                }
                                else
                                {
                                    Rotation -= RotationSpeed;
                                    if (Rotation < Slope)
                                        Rotation = Slope;
                                }
                            }
                        }

                        //left closest test
                        else if ((Left < Right || IsTriangle && IsRight) && Left < Top && Left < Bottom && !(IsTriangle && !IsRight) && !level.CollisionObjects[ecx].Passable)
                        {
                            if (CanHover && HoverDistance > Math.Abs(IntendedY - Position.Y) && !HoverCollision)
                            {
                                JumpTime = ActualMaxJumpTime;
                                HoverDistance = Math.Abs(IntendedY - (Position.Y - HoverDistance));
                                Position.Y = IntendedY;
                            }
                            else //Set position and reverse direction
                            {
                                Position.X = Position.X - Left;
                                //  velocity.X = 0;
                                NPCDirection *= -1;
                            }
                        }
                        //right closest test
                        else if (Right < Left && Right < Top && Right < Bottom && !(IsTriangle && IsRight) && !level.CollisionObjects[ecx].Passable)
                        {
                            if (CanHover && HoverDistance > Math.Abs(IntendedY - Position.Y) && !HoverCollision)
                            {
                                JumpTime = ActualMaxJumpTime;
                                HoverDistance = Math.Abs(IntendedY - (Position.Y - HoverDistance));
                                Position.Y = IntendedY;
                            }
                            else //Set position and reverse direction
                            {
                                Position.X = Position.X + Right;
                                //  velocity.X = 0;
                                NPCDirection *= -1;

                            }
                        }
                        //Bottom closest test
                        else if ((Bottom <= Left || IsTriangle && !IsRight) && (Bottom < Right || IsTriangle && IsRight) && Bottom < Top && !level.CollisionObjects[ecx].Passable)
                        {
                            CeilingCollision = true;
                            if (!IsTriangle)
                                Position.Y = IntendedY + level.CollisionObjects[ecx].Height + EntityHeight;
                            else
                                Position.Y = (level.CollisionObjects[ecx].Y + level.CollisionObjects[ecx].Height) + EntityHeight;
                            LastTouchedObj = OldLastTouched;
                        }
                        else //Fix for some minor issue where player gets stuck at bottom of a passable collision object
                        {
                            if (level.CollisionObjects[ecx].Passable)
                            {
                                if (HoverDistance == MaxHoverDistance && Velocity.Y > 0 && !HoverCollision)
                                {
                                    HoverDistance = Math.Abs(IntendedY - (Position.Y - HoverDistance));
                                    Position.Y = IntendedY;
                                }
                            }
                        }
                    }
                }
            } //End Foreach

            if (HoverCollision) //Undo crap used in a temporary collision test
                Position.Y += InitialHoverDistance;

        } //End HandleCollisions()

        #endregion

        #region Methods

        public double RandomDouble(double Start, double End)
        {
            return (level.Random.NextDouble() * Math.Abs(End - Start)) + Start;
        }

        public void Reset(Vector2 position)
        {
            Position = position;
            Velocity = Vector2.Zero;
            IsAlive = true;
            Sprite.PlayAnimation(IdleAnimation);
        }

        private float DoJump(float velocityY, GameTime gameTime)
        {
            if (JumpButtonHeld)
            {
                // Begin or continue a jump
                if ((!WasJumping && IsOnGround) || JumpTime > 0.0f)
                {
                    if (JumpTime == 0.0f)
                    {
                        if (!Main.Muted)
                            JumpSound.Play();
                    }
                    JumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (!IsAttacking)
                        Sprite.PlayAnimation(JumpAnimation);
                    else
                    {
                        Sprite.PlayAnimation(AttackAnimation);
                    }
                }

                // If we are in the ascent of the jump
                if (JumpTime > 0.0f && JumpTime <= ActualMaxJumpTime)
                {
                    // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
                    velocityY = ActualJumpLaunchVelocity * (1.0f - (float)Math.Pow(JumpTime / ActualMaxJumpTime, ActualJumpControlPower));
                }
                else
                {
                    // Reached the apex of the jump
                    JumpTime = 0.0f;
                }
            }
            else
            {
                // Continues not jumping or cancels a jump in progress
                JumpTime = 0.0f;
            }

            //if (EnemyType == Story.EnemyType.Player)
            WasJumping = JumpButtonHeld;

            return velocityY;
        }

        public void GetHit(int damage)
        {
            //Only do collisions when player is not blinking if or if "instakill"
            if (BlinkTimer <= 0.0f || damage == InstaKill)
            {
                if (Health - damage > 0)
                {
                    //hurt player
                    Health -= damage;
                    //if (EnemyType == EnemyType.Player)
                    //    BlinkTimer = 1500.0f;
                    //else
                    BlinkTimer = 1000.0f;

                    if (Health < 0)
                        Health = 0;
                }
                else
                {
                    Health = 0;
                    IsAlive = false;

                    DeathCoords = level.Player.Position;
                    DeathEmitter.StartPosition.X = BoundingRectangle.X - level.Player.Position.X + Main.BackBufferWidth / 2 + BoundingRectangle.Width / 2;
                    DeathEmitter.StartPosition.Y = BoundingRectangle.Y - level.Player.Position.Y + Main.BackBufferHeight / Tile.VerticalScale + BoundingRectangle.Height / 2;
                    DeathEmitter.StartEffect();
                }
            }
        }

        public void OnReachedExit()
        {
            Sprite.PlayAnimation(CelebrateAnimation);
        }

        #endregion

        #region Draw

        public void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
        {
            //Draw entity
            Vector2 DrawPosition = Position;
            DrawPosition.X -= level.Player.Position.X - Main.BackBufferWidth / 2;
            DrawPosition.Y -= level.Player.Position.Y - level.Player.HoverDistance - Main.BackBufferHeight / Tile.VerticalScale + Bounce + HoverDistance;

            if (level.IsOnScreen(DrawPosition, BoundingRectangle.Width, BoundingRectangle.Height))
            {
                //Flip the sprite to face the way we are moving.
                if (Velocity.X > 0)
                    Flip = SpriteEffects.FlipHorizontally;
                else if (Velocity.X < 0)
                    Flip = SpriteEffects.None;

                float Fraction = BlinkTimer / 100 - (float)Math.Truncate(BlinkTimer / 100);
                if (Fraction < 0.6f && IsAlive) //Blink 40% of the time if blinking
                    Sprite.Draw(GameTime, SpriteBatch, DrawPosition, Rotation, Flip, Underwater);

                //DEBUG MODE: Draw collision boxes
                if (Main.DebugMode && IsAlive)
                {
                    if (CollisionBoxLine == null)
                        CollisionBoxLine = new LinePrimitive(SpriteBatch.GraphicsDevice, Level);

                    CollisionBoxLine.ClearVectors();
                    CollisionBoxLine.AddVector(new Vector2(BoundingRectangle.X, BoundingRectangle.Y));

                    CollisionBoxLine.AddVector(new Vector2(BoundingRectangle.X + BoundingRectangle.Width, BoundingRectangle.Y));
                    CollisionBoxLine.AddVector(new Vector2(BoundingRectangle.X + BoundingRectangle.Width, BoundingRectangle.Y + BoundingRectangle.Height));
                    CollisionBoxLine.AddVector(new Vector2(BoundingRectangle.X, BoundingRectangle.Y + BoundingRectangle.Height));

                    CollisionBoxLine.AddVector(new Vector2(BoundingRectangle.X, BoundingRectangle.Y));

                    CollisionBoxLine.Render(SpriteBatch, Color.Orange);
                }
            }

            //Draw projectiles
            //foreach (Projectile Proj in Projectiles)
            for (int ecx = 0; ecx < Projectiles.Count; ecx++)
                Projectiles[ecx].Draw(GameTime, SpriteBatch);

            //Draw death emitter
            DeathEmitter.Draw(GameTime, SpriteBatch, level.Player.Position - DeathCoords - new Vector2(0, level.Player.HoverDistance));
        }

        #endregion
    }
}