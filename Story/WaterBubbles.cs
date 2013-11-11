using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Story
{
    class WaterBubbles
    {
        private LinePrimitive CollisionBoxLine;
        private ParticleEmitter BubbleEmitter;

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        } private Vector2 position;

        public Level Level
        {
            get { return level; }
        } private Level level;

        private Rectangle LocalBounds;
        /// <summary>
        /// Gets a rectangle which bounds this player in world space.
        /// </summary>
        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X + LocalBounds.X);
                int top = (int)Math.Round(Position.Y) - LocalBounds.Height / 2;

                return new Rectangle(left, top, LocalBounds.Width, LocalBounds.Height);
            }
        }

        public WaterBubbles(Level Level, Vector2 Position)
        {
            this.level = Level;
            this.Position = Position;
            LoadContent();
        }

        private void LoadContent()
        {
            BubbleEmitter = new ParticleEmitter(new Vector2(Main.BackBufferWidth / 2 + 32, Main.BackBufferHeight / Tile.VerticalScale + 48),
                new Color(128, 128, 128, 255), new Vector2(-128, -128), new Vector2(Main.BackBufferWidth + 128, Main.BackBufferHeight + 128), new Vector2(-0.4f, -2.00f), new Vector2(0.4f, -0.5f),
                new Vector2(-6, -12), new Vector2(6, 12), 4000, 4, 32, 0.0025f, .20f, 0.001f, 0.3f, false, 2500.0f);

            BubbleEmitter.LoadContent(Level.Content.Load<Texture2D>("Particles/BreathParticle"));
            BubbleEmitter.StartEffect();

            int Width = 64;
            int Left = 0;
            int Height = 64;
            int Top = 0;

            //Apply
            LocalBounds = new Rectangle(Left, Top, Width, Height);
        }


        public void Update(GameTime GameTime)
        {
            if (!BubbleEmitter.CreateEffect)
                BubbleEmitter.StartEffect();
            BubbleEmitter.Update(GameTime);
        }

        public void Draw(GameTime GameTime, SpriteBatch SpriteBatch)
        {
            //Onscreen test
            // if !(level.IsOnScreen(DrawPostion, outhouseOpen.Width, outhouseOpen.Height))
            //{
            //   return;
            //}

            BubbleEmitter.Draw(GameTime, SpriteBatch, Level.Player.Position - new Vector2(0, Level.Player.HoverDistance) - Position);
            //Draw collision boxes
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

                CollisionBoxLine.Render(SpriteBatch, Color.White);
            }

        } //End draw
    }
}
