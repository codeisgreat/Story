using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Story
{
    class Player : Entity
    {
        public const int MaxPlayerHP = 10;
        public const float MindPowerMax = 8000.0f;

        private Vector2 BreathCoords; //Players coords when the bubblez start
        private ParticleEmitter BreathEmitter;      //Underwater bubblez
        private ParticleEmitter CloudEmitter;

        private const float MoveStickScale = 1.0f;
        private float PlayerAttackTimer = 0.0f;

        public Player(Level level, Vector2 position)
            : base(level, position)
        {
            //Load breath emitter
            BreathEmitter = new ParticleEmitter(new Vector2(0, 0),
new Color(255, 255, 255, 255), new Vector2(0, 0), new Vector2(Main.BackBufferWidth, Main.BackBufferHeight), new Vector2(-0.2f, -1.8f), new Vector2(0.2f, 0.2f),
new Vector2(-4, -8), new Vector2(4, -8), 1500, 1, 32, 0f, .55f, 0.0006f, 0.8f, true, 0f);

            Health = MaxPlayerHP;
            CanHover = true;

            //Set physics stuff
            Speed = 1.0f;
            switch (level.CurrentEnvironment)
            {
                case Level.Environment.Grass:
                    GroundDragFactor = 0.58f;
                    break;
                case Level.Environment.Snow:
                    GroundDragFactor = 0.90f;
                    break;
                default:
                    GroundDragFactor = 0.58f;
                    break;
            }

            //Default right
            Flip = SpriteEffects.FlipHorizontally;

            LoadPlayerContent();
        }

        public void LoadPlayerContent()
        {
            int Width;
            int Left;
            int Height;
            int Top;

            //Load breath emitter
            BreathEmitter.LoadContent(Level.Content.Load<Texture2D>("Particles/BreathParticle"));

            //Load cloud Emitter
            CloudEmitter = new ParticleEmitter(new Vector2(Main.BackBufferWidth / 2, Main.BackBufferHeight / Tile.VerticalScale - 16),
                new Color(255, 255, 255, 255), new Vector2(0, 0), new Vector2(Main.BackBufferWidth, Main.BackBufferHeight), new Vector2(-1.0f, -0.1f), new Vector2(1.0f, 0.6f),
                new Vector2(-16, -4), new Vector2(16, 0), 1500, 5, 32, 0.0025f, .35f, 0.0006f, 0.6f, true, 0.0f);
            CloudEmitter.LoadContent(Level.Content.Load<Texture2D>("Particles/WindParticle"));

            // Load animated textures.
            IdleAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Idle"), 0.1f, false);
            RunAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Idle"), 0.15f, false);
            JumpAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Idle"), 0.1f, false);
            GlideAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Player_Glide"), 0.15f, false);
            AttackAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Player_Attack"), 0.1f, false);
            UnderwaterAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Player_Underwater"), 0.1f, false);
            CelebrateAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Idle"), 0.1f, false);
            DeathAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Idle"), 0.1f, false);

            Width = (int)(IdleAnimation.FrameWidth);
            Left = 0;
            Height = (int)(IdleAnimation.FrameWidth);
            Top = 0;

            //Apply
            LocalBounds = new Rectangle(Left, Top, Width, Height);

            // Load sounds.            
            DeathSound = Level.Content.Load<SoundEffect>("Sounds/PlayerKilled");
            JumpSound = Level.Content.Load<SoundEffect>("Sounds/PlayerJump");
            FallSound = Level.Content.Load<SoundEffect>("Sounds/PlayerFall");

            LoadContent();
        }

        public void UpdatePlayer(GameTime GameTime)
        {
            Update(GameTime);

            if (Underwater)
                SubMerge();

            //Allow gliding again once touched back on ground
            if (IsOnGround)
                GlideFatigued = false;

            GetInput(GameTime);
            ApplyPhysics(GameTime, true);

            //Update emitters
            CloudEmitter.Update(GameTime);
            if ((!Underwater || BreathingUW))
            {
                //if (HoverCollision)
                //    BreathEmitter.EndEffect();
                //else if (!CanHover)
                //    BreathEmitter.EndEffect();
            }

            //Either slowly restore mindpower or decrease if underwater
            if (!Underwater || BreathingUW)
            {
                MindPower += GameTime.ElapsedGameTime.Milliseconds;
                if (MindPower >= MindPowerMax)
                    MindPower = MindPowerMax;

                if (BreathingUW)
                    MindPower += GameTime.ElapsedGameTime.Milliseconds;
            }
            else
            {
                MindPower -= GameTime.ElapsedGameTime.Milliseconds / 3;
                //Drowning
                if (MindPower < 0 && !Main.GodMode)
                {
                    GetHit(1);
                    MindPower = 0;
                }
            }

            // Bounce constants
            const float BounceHeight = .18f;
            const float BounceRate = 3.0f;
            const float BounceSync = -0.75f;

            // Bounce along a sine curve over time.         
            Bounce = (float)Math.Sin(GameTime.TotalGameTime.TotalSeconds * BounceRate * BounceSync) * BounceHeight * LocalBounds.Height;

            if (PlayerAttackTimer > 0.0f)
                PlayerAttackTimer -= GameTime.ElapsedGameTime.Milliseconds;
            else
            {
                //Return to old animation & reset variables
                if (Underwater)
                    Sprite.PlayAnimation(UnderwaterAnimation);
                else if (Glide == true)
                    Sprite.PlayAnimation(GlideAnimation);
                else
                    Sprite.PlayAnimation(IdleAnimation);
                IsAttacking = false;
            }


            //foreach (Projectile Proj in Projectiles)
            for (int ecx = 0; ecx < Projectiles.Count; ecx++)
            {
                Projectiles[ecx].Update(GameTime);

                //Out of bounds test
                if ((Projectiles[ecx].Position.X < Position.X - Main.BackBufferWidth * 0.75 ||
                    Projectiles[ecx].Position.X > Position.X + Main.BackBufferWidth * 0.75) &&
                    !Projectiles[ecx].HasCollided)
                {
                    Projectiles[ecx].Collide();
                }
                //Collision test for each enemy (if not already collided)
                else if (!Projectiles[ecx].HasCollided)
                    //foreach (Entity enemy in Level.Enemies)
                    for (int edx = 0; edx < Level.Enemies.Count; edx++)
                        if (Level.Enemies[edx].BlinkTimer == 0.0f && Level.Enemies[edx].IsAlive &&
                            Projectiles[ecx].BoundingRectangle.Intersects(Level.Enemies[edx].BoundingRectangle))
                        {
                            Projectiles[ecx].Collide();
                            Level.Enemies[edx].GetHit(1);
                            break;
                        }
            }

            for (int ecx = Projectiles.Count - 1; ecx == 0; ecx--)
            {
                if (Projectiles[ecx].Dead && Projectiles[ecx].HasCollided)
                    Projectiles.Remove(Projectiles[ecx]);
            }

            if (IsOnGround)
            {
                if (IsAttacking)
                    Sprite.PlayAnimation(AttackAnimation);
                else if (UnderwaterAnimation != null && Underwater && !BreathingUW)
                    Sprite.PlayAnimation(UnderwaterAnimation);
                else if (Glide == true)
                    Sprite.PlayAnimation(GlideAnimation);
                else if (Math.Abs(Velocity.X) - 0.02f > 0)
                    Sprite.PlayAnimation(RunAnimation);
                else
                    Sprite.PlayAnimation(IdleAnimation);
            }

            //Go back to last check point or start of level if dead
            if (!IsAlive)
            {
                Position = level.LastCheckPoint;
                MindPower = MindPowerMax;
                Health = MaxPlayerHP;
                IsAlive = true;
                BlinkTimer = 2000.0f;
            }

            //No rotation for player since they just hover (lazy fix)
            Rotation = 0;

            // Clear input.
            Movement = Vector2.Zero;
            //JumpButtonHeld = false;

            BreathEmitter.Update(GameTime);

            BreathingUW = false;
        } //End update

        private void GetInput(GameTime gameTime)
        {
            // Get input state.
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();

            // Get analog horizontal movement.
            Movement = gamePadState.ThumbSticks.Left * MoveStickScale;

            if ((gamePadState.Triggers.Right > 0.5f && gamePadState != level.LastGamePadState) ||
(keyboardState.IsKeyDown(Keys.Space) && keyboardState != level.LastKeyboardState))
                Attack();

            // Ignore small movements to prevent running in place.
            if (Math.Abs(Movement.X) < 0.5f)
                Movement.X = 0.0f;

            if ((Movement.Y < -0.5f ||
                gamePadState.IsButtonDown(Buttons.B) ||
                keyboardState.IsKeyDown(Keys.Down) ||
                keyboardState.IsKeyDown(Keys.S)) && IsOnGround)
                Sink = true;
            else
                Sink = false;

            // Check if the entity wants to jump.
            if (gamePadState.IsButtonDown(Buttons.A) ||
                keyboardState.IsKeyDown(Keys.Up) ||
                keyboardState.IsKeyDown(Keys.W))
                JumpButtonHeld = true;
            else
                JumpButtonHeld = false;

            if ((gamePadState.IsButtonDown(Buttons.A) && !level.LastGamePadState.IsButtonDown(Buttons.A)) ||
               (keyboardState.IsKeyDown(Keys.Up) && !level.LastKeyboardState.IsKeyDown(Keys.Up)) ||
                (keyboardState.IsKeyDown(Keys.W) && !level.LastKeyboardState.IsKeyDown(Keys.W)) && !GlideFatigued && !IsOnGround && !Underwater)
            {
                //Start gliding & create effect if not already in use
                Glide = true;
            }

            if ((!gamePadState.IsButtonDown(Buttons.A) && level.LastGamePadState.IsButtonDown(Buttons.A)) ||
               (!keyboardState.IsKeyDown(Keys.Up) && level.LastKeyboardState.IsKeyDown(Keys.Up)) ||
                (!keyboardState.IsKeyDown(Keys.W) && level.LastKeyboardState.IsKeyDown(Keys.W)) || GlideFatigued || IsOnGround || Underwater)
            {
                Glide = false;
            }

            if (Glide)
            {
                if (!CloudEmitter.CreateEffect)
                    CloudEmitter.StartEffect();

                //Time limit for gliding
                MindPower -= gameTime.ElapsedGameTime.Milliseconds * 2;
                if (MindPower <= 0f)
                {
                    MindPower = 0.0f;
                    GlideFatigued = true;
                }
            }
            else
            {
                CloudEmitter.EndEffect();
            }


            // If any digital horizontal movement input is found, override the analog movement.
            if (gamePadState.IsButtonDown(Buttons.DPadLeft) ||
                keyboardState.IsKeyDown(Keys.Left) ||
                keyboardState.IsKeyDown(Keys.A))
            {
                Movement.X = -1.0f;
            }
            else if (gamePadState.IsButtonDown(Buttons.DPadRight) ||
                     keyboardState.IsKeyDown(Keys.Right) ||
                     keyboardState.IsKeyDown(Keys.D))
            {
                Movement.X = 1.0f;
            }

        }

        public void BreatheUnderWater()
        {
            BreathingUW = true;
        }

        public void Attack()
        {
            if (!IsAttacking && level.Eggs > 0)
            {
                IsAttacking = true;
                Sprite.PlayAnimation(AttackAnimation);
                level.Eggs--;
                PlayerAttackTimer = 250.0f;
                Projectiles.Add(new Projectile(Level, new Vector2(BoundingRectangle.X + BoundingRectangle.Width / 2, BoundingRectangle.Y + BoundingRectangle.Height / 2), Flip, 24.0f, -Bounce));
            }
        }

        //Go into the water. Live there. Die there.
        public void SubMerge()
        {
            //breath/underwater animation crap
            BreathCoords = level.Player.Position;
            int XOffset = LocalBounds.Width / 2 - 4;
            int YOffset = LocalBounds.Height / 2;

            if (Flip == SpriteEffects.None)
                XOffset = -XOffset;

            BreathEmitter.StartPosition.X = Main.BackBufferWidth / 2 + XOffset;
            BreathEmitter.StartPosition.Y = Main.BackBufferHeight / Tile.VerticalScale - YOffset - Bounce - HoverDistance;

            if (!BreathEmitter.CreateEffect)
                BreathEmitter.StartEffect();
            if (!IsAttacking)
                Sprite.PlayAnimation(UnderwaterAnimation);

        }

        public void DrawPlayer(GameTime GameTime, SpriteBatch SpriteBatch)
        {
            Draw(GameTime, SpriteBatch);
            BreathEmitter.Draw(GameTime, SpriteBatch, level.Player.Position - BreathCoords - new Vector2(0, level.Player.HoverDistance));
            CloudEmitter.Draw(GameTime, SpriteBatch, new Vector2(0, Bounce));
        }

    }
}
