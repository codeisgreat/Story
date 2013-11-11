using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Story
{
    class Collectable
    {
        public const int PointValue = 1;
        public readonly Color Color = Color.White;

        public bool Collected = false;
        public int ObjectID;

        private SoundEffect CollectedSound;
        private Texture2D Texture;
        private Vector2 Origin;
        
        private Vector2 BasePosition;
        private float Bounce;

        public LinePrimitive CollisionBoxLine;

        public Level Level
        {
            get { return level; }
        } private Level level;

        public Vector2 Position
        {
            get
            {
                return BasePosition + new Vector2(0.0f, Bounce);
            }
        }

        public Rectangle BoundingRectangle
        {
            get
            {
                int Left = (int)Math.Round(Position.X + LocalBounds.X - LocalBounds.Width / 2);
                int Top = (int)Math.Round(Position.Y);

                return new Rectangle(Left, Top, LocalBounds.Width, LocalBounds.Height);
            }
        } private Rectangle LocalBounds;

        public Collectable(Level level, Vector2 Position, int ObjectID)
        {
            this.level = level;
            this.BasePosition = Position;
            this.ObjectID = ObjectID;
            LoadContent();
        }

        public void LoadContent()
        {
            if (ObjectID == 0)
                Texture = Level.Content.Load<Texture2D>("Sprites/Egg");
            else if (ObjectID == 1)
                Texture = Level.Content.Load<Texture2D>("Sprites/Gem1");
            else if (ObjectID == 2)
                Texture = Level.Content.Load<Texture2D>("Sprites/Gem2");
            else if (ObjectID == 3)
                Texture = Level.Content.Load<Texture2D>("Sprites/Gem3");

            Origin = new Vector2(0, 0);
            CollectedSound = Level.Content.Load<SoundEffect>("Sounds/GemCollected");

            int Width = (int)(Texture.Width);
            int Left = 0;
            int Height = (int)(Texture.Height);
            int Top = 0;

            //Apply
            LocalBounds = new Rectangle(Left, Top, Width, Height);
        }

        public void Update(GameTime GameTime)
        {
            // Bounce control constants
            const float BounceHeight = 0.18f;
            const float BounceRate = 3.0f;
            const float BounceSync = -0.75f;

            // Bounce along a sine curve over time.
            // Include the X coordinate so that neighboring gems bounce in a nice wave pattern.            
            Bounce = (float)Math.Sin(GameTime.TotalGameTime.TotalSeconds *
                BounceRate + Position.X * BounceSync) * BounceHeight * Texture.Height;
        }

        public void OnCollected()
        {
            Collected = true;
            if (!Main.Muted)
                CollectedSound.Play();
        }

        public void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
        {
            Vector2 DrawPosition = new Vector2(Position.X - (level.Player.Position.X - Main.BackBufferWidth / 2) - BoundingRectangle.Width / 2, Position.Y - (level.Player.Position.Y - level.Player.HoverDistance - Main.BackBufferHeight / Tile.VerticalScale));

            if (level.IsOnScreen(DrawPosition, BoundingRectangle.Width, BoundingRectangle.Height))
            {
                SpriteBatch.Draw(Texture, DrawPosition, null, Color, 0.0f, Origin, 1.0f, SpriteEffects.None, 0.0f);

                if (Main.DebugMode)
                {
                    if (CollisionBoxLine == null)
                    {
                        CollisionBoxLine = new LinePrimitive(SpriteBatch.GraphicsDevice, Level);

                        CollisionBoxLine.AddVector(new Vector2(BoundingRectangle.X, BoundingRectangle.Y));

                        CollisionBoxLine.AddVector(new Vector2(BoundingRectangle.X + BoundingRectangle.Width, BoundingRectangle.Y));
                        CollisionBoxLine.AddVector(new Vector2(BoundingRectangle.X + BoundingRectangle.Width, BoundingRectangle.Y + BoundingRectangle.Height));
                        CollisionBoxLine.AddVector(new Vector2(BoundingRectangle.X, BoundingRectangle.Y + BoundingRectangle.Height));

                        CollisionBoxLine.AddVector(new Vector2(BoundingRectangle.X, BoundingRectangle.Y));

                    }

                    CollisionBoxLine.Render(SpriteBatch, Color.Purple);
                }
            }
        }
    }
}
