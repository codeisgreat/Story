using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Story
{
    class Enemy : Entity
    {
        private EnemyType CurrentEnemyType;
        private float JumpTimer = 0.0f; //Used in faking input
        private double JumpVariation = 1d;

        public Enemy(Level level, Vector2 position, EnemyType EnemyType)
            : base(level, position)
        {

            this.CurrentEnemyType = EnemyType;

            //Load stats based on type
            switch (CurrentEnemyType)
            {
                case EnemyType.Velociraptor:
                    JumpVariation = RandomDouble(0.8d, 1.2d);
                    MaxJumpTime = 0.32f;
                    //Set random base speed
                    Speed = (float)RandomDouble(0.6d, 1.2d);
                    Health = 1;
                    MaxMoveSpeed = 400f;
                    break;
                case EnemyType.Dunkleosteus:
                    Speed = (float)RandomDouble(0.6d, 1.2d);
                    Health = 1;
                    MaxMoveSpeed = 200f;
                    break;
                case EnemyType.Pterodactyl:
                    Speed = (float)RandomDouble(0.6d, 1.2d);
                    Health = 1;
                    MaxMoveSpeed = 200f;
                    break;
            }

            //Randomly assign direction
            if (level.Random.Next(0, 2) == 0)
            {
                Flip = SpriteEffects.None;
                NPCDirection = -1;
            }
            else
            {
                Flip = SpriteEffects.FlipHorizontally;
                NPCDirection = 1;
            }

            LoadEnemyContent();
        }

        public void LoadEnemyContent()
        {
            int Width;
            int Left;
            int Height;
            int Top;

            string SpriteSet = "";
            int DeadSpaceX = 0;
            int DeadSpaceY = 0;

            switch (CurrentEnemyType)
            {
                case EnemyType.Velociraptor:

                    // Load animated textures.
                    IdleAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Raptor/Idle"), 0.1f, true);
                    RunAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Raptor/run"), Speed / 10f, true);
                    JumpAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Raptor/Jump"), 0.1f, false);
                    AttackAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Raptor/Attack"), 0.1f, false);
                    CelebrateAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Celebrate"), 0.1f, false);
                    DeathAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Die"), 0.1f, false);

                    DeadSpaceX = 24;
                    DeadSpaceY = 24;

                    Width = (int)(IdleAnimation.FrameWidth) - DeadSpaceX;
                    Left = DeadSpaceX;
                    Height = (int)(IdleAnimation.FrameWidth - DeadSpaceY);
                    Top = 0;

                    //Apply
                    LocalBounds = new Rectangle(Left, Top, Width, Height);

                    // Load sounds.            
                    DeathSound = Level.Content.Load<SoundEffect>("Sounds/PlayerKilled");
                    JumpSound = Level.Content.Load<SoundEffect>("Sounds/PlayerJump");
                    FallSound = Level.Content.Load<SoundEffect>("Sounds/PlayerFall");
                    break;
                case EnemyType.Dunkleosteus:
                    // Load animations.
                    SpriteSet = "Sprites/" + EnemyType.Dunkleosteus.ToString() + "/";
                    RunAnimation = new Animation(Level.Content.Load<Texture2D>(SpriteSet + "Idle"), 0.1f, true);
                    JumpAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Jump"), 0.1f, false);
                    IdleAnimation = new Animation(Level.Content.Load<Texture2D>(SpriteSet + "Idle"), 0.15f, true);
                    Sprite.PlayAnimation(IdleAnimation);

                    // Calculate bounds within texture size.

                    DeadSpaceY = 84;
                    CanHover = true;
                    //MaxHoverDistance = 48f;
                    Width = (int)(IdleAnimation.FrameWidth);
                    Left = 0;
                    Height = (int)(IdleAnimation.FrameWidth) - DeadSpaceY;
                    Top = 0;

                    LocalBounds = new Rectangle(Left, Top, Width, Height);

                    // Load sounds.            
                    DeathSound = Level.Content.Load<SoundEffect>("Sounds/PlayerKilled");
                    JumpSound = Level.Content.Load<SoundEffect>("Sounds/PlayerJump");
                    FallSound = Level.Content.Load<SoundEffect>("Sounds/PlayerFall");
                    break;
                case EnemyType.Pterodactyl:
                    // Load animations.
                    SpriteSet = "Sprites/" + EnemyType.Pterodactyl.ToString() + "/";
                    RunAnimation = new Animation(Level.Content.Load<Texture2D>(SpriteSet + "Flying"), 0.09f, true);
                    JumpAnimation = new Animation(Level.Content.Load<Texture2D>("Sprites/Player/Jump"), 0.1f, false);
                    IdleAnimation = RunAnimation;
                    Sprite.PlayAnimation(IdleAnimation);

                    // Calculate bounds within texture size.

                    DeadSpaceX = 24;
                    DeadSpaceY = 52;
                    //MaxHoverDistance = 48f;
                    CanHover = true;

                    Width = (int)(IdleAnimation.FrameWidth) - DeadSpaceX;
                    Left = DeadSpaceX;
                    Height = (int)(IdleAnimation.FrameWidth) - 48;
                    Top = -16;

                    LocalBounds = new Rectangle(Left, Top, Width, Height);

                    // Load sounds.            
                    DeathSound = Level.Content.Load<SoundEffect>("Sounds/PlayerKilled");
                    JumpSound = Level.Content.Load<SoundEffect>("Sounds/PlayerJump");
                    FallSound = Level.Content.Load<SoundEffect>("Sounds/PlayerFall");
                    break;
            }

            LoadContent();
        }

        public void UpdateEnemy(GameTime GameTime)
        {
            Update(GameTime);

            Vector2 LastPosition = Position;

            SimulateInput(GameTime);

            ApplyPhysics(GameTime, false);

            if (IsAlive && IsOnGround)
            {
                //if (Glide == true)
                //    sprite.PlayAnimation(glideAnimation);
                if (Math.Abs(Velocity.X) - 0.02f > 0)
                    Sprite.PlayAnimation(RunAnimation);
                else
                    Sprite.PlayAnimation(IdleAnimation);
            }

            if (CanHover)
                Rotation = 0f;

            // Clear input.
            Movement.X = 0.0f;
            //JumpButtonHeld = false;
        }

        public void Attack()
        {
            switch (CurrentEnemyType)
            {
                case EnemyType.Velociraptor:
                    IsAttacking = true;
                    JumpButtonHeld = true;
                    WasJumping = false;
                    break;
                default:
                    break;
            }
        }

        private void SimulateInput(GameTime gameTime)
        {
            switch (CurrentEnemyType)
            {
                case EnemyType.Velociraptor:

                    JumpTimer += (float)(gameTime.ElapsedGameTime.Milliseconds * JumpVariation);

                    if (JumpTimer > 4000.0f && IsOnGround)
                    {
                        WasJumping = true;
                        JumpButtonHeld = false;
                        JumpTimer = 0.0f;
                    }
                    else if (JumpTimer > 3500.0f)
                    {
                        WasJumping = false;
                        JumpButtonHeld = true;
                    }

                    Movement.X = Speed * NPCDirection;
                    break;
                default:
                    Movement.X = Speed * NPCDirection;
                    break;
            }
        }

        public enum EnemyType
        {
            Velociraptor,
            Dunkleosteus,
            Pterodactyl,
        }
    }
}
