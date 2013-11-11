using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;


namespace Story
{
    class Projectile
    {
        private Texture2D ProjectileTexture;
        private LinePrimitive CollisionBoxLine;
        private float MoveSpeed = 768.0f;
        private int Direction = 1;
        private SpriteEffects Flip;
        private ParticleEmitter ProjectileHitEmitter;
        public bool Dead = false; //Projectile state
        public bool HasCollided = false;

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        } private Vector2 position;

        public Level Level
        {
            get { return level; }
        } private Level level;

        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X + localBounds.X);
                int top = (int)Math.Round(Position.Y);

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        } private Rectangle localBounds;

        public Projectile(Level level, Vector2 position, SpriteEffects Flip, float offsetX, float offsetY)
        {
            this.level = level;
            this.Position = position;
            this.Flip = Flip;
            if (Flip != SpriteEffects.FlipHorizontally)
                Direction = -1;

            ProjectileHitEmitter = new ParticleEmitter(new Vector2(0, 0),
new Color(255, 255, 255, 255), new Vector2(0, 0), new Vector2(Main.BackBufferWidth, Main.BackBufferHeight), new Vector2(-0.2f, -1.0f), new Vector2(2.4f, 0.4f),
new Vector2(0, -8), new Vector2(0, 0), 8500, 2, 32, 0.0025f, 0.25f, 0.006f, 0.2f, false, 250.0f);


            LoadContent();
            this.position.Y -= ProjectileTexture.Height / 2 * Tile.VerticalScale - offsetY;
            this.position.X -= ProjectileTexture.Width / 2 - offsetX * Direction;
        }

        private void LoadContent()
        {
            ProjectileTexture = Level.Content.Load<Texture2D>("Sprites/Player/EggProjectile");
            ProjectileHitEmitter.LoadContent(Level.Content.Load<Texture2D>("Particles/ProjectileParticle"));

            int width = (int)(ProjectileTexture.Width);
            int left = 0;
            int height = (int)(ProjectileTexture.Height);
            int top = 0;

            localBounds = new Rectangle(left, top, width, height);
        }

        public void Update(GameTime GameTime)
        {
            ProjectileHitEmitter.Update(GameTime);
            if (ProjectileHitEmitter.CurrentTime > ProjectileHitEmitter.LiveTime)
                Dead = true;
            position.X += MoveSpeed * Direction * (float)GameTime.ElapsedGameTime.TotalSeconds;
        }

        private Vector2 HitCoords;
        public void Collide()
        {
            HasCollided = true;
            HitCoords = level.Player.Position;

            float StartXOffset = localBounds.Width / 2;
            //Reverse velocities if facing other way
            if (Direction == -1)
            {
                float Temp = ProjectileHitEmitter.MinVelocity.X;
                ProjectileHitEmitter.MinVelocity.X = -ProjectileHitEmitter.MaxVelocity.X;
                ProjectileHitEmitter.MaxVelocity.X = -Temp;
                StartXOffset = -StartXOffset;
            }

            ProjectileHitEmitter.StartPosition.X = position.X - level.Player.Position.X + Main.BackBufferWidth / 2 + StartXOffset;
            ProjectileHitEmitter.StartPosition.Y = position.Y - level.Player.Position.Y + Main.BackBufferHeight / Tile.VerticalScale + localBounds.Height;
            ProjectileHitEmitter.StartEffect();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            ProjectileHitEmitter.Draw(gameTime, spriteBatch, level.Player.Position - HitCoords - new Vector2(0, level.Player.HoverDistance));

            if (!HasCollided)
            {
                Vector2 DrawPosition = Position;
                DrawPosition.X -= level.Player.Position.X - Main.BackBufferWidth / 2;
                DrawPosition.Y -= level.Player.Position.Y - level.Player.HoverDistance - Main.BackBufferHeight / Tile.VerticalScale;

                spriteBatch.Draw(ProjectileTexture, DrawPosition, null, Color.White, 0.0f, new Vector2(0, 0), 1.0f, Flip, 0);

                if (Main.DebugMode)
                {
                    if (CollisionBoxLine == null)
                        CollisionBoxLine = new LinePrimitive(spriteBatch.GraphicsDevice, Level);

                    CollisionBoxLine.ClearVectors();

                    CollisionBoxLine.AddVector(new Vector2(BoundingRectangle.X, BoundingRectangle.Y));

                    CollisionBoxLine.AddVector(new Vector2(BoundingRectangle.X + BoundingRectangle.Width, BoundingRectangle.Y));
                    CollisionBoxLine.AddVector(new Vector2(BoundingRectangle.X + BoundingRectangle.Width, BoundingRectangle.Y + BoundingRectangle.Height));
                    CollisionBoxLine.AddVector(new Vector2(BoundingRectangle.X, BoundingRectangle.Y + BoundingRectangle.Height));

                    CollisionBoxLine.AddVector(new Vector2(BoundingRectangle.X, BoundingRectangle.Y));

                    CollisionBoxLine.Render(spriteBatch, Color.Aquamarine);
                }
            }
        }

    }
}
