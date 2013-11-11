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
    class Outhouse
    {
        private Texture2D outhouseOpen;
        private Texture2D outhouseClosed;
        private LinePrimitive collisionBoxLine;
        public bool isOpen;
        private const int DeadSpace = 28;

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        Vector2 position;

        public Level Level
        {
            get { return level; }
        }
        Level level;

        private Rectangle localBounds;
        /// <summary>
        /// Gets a rectangle which bounds this player in world space.
        /// </summary>
        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X + localBounds.X);
                int top = (int)Math.Round(Position.Y);

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }

        public Outhouse(Level level, Vector2 position)
        {
            this.level = level;
            this.Position = position;
            LoadContent();
            this.position.Y -= outhouseOpen.Height / 2 * Tile.VerticalScale - 24; //24 is how far down into ground we want the outhouse
            this.position.X -= outhouseOpen.Width / 2 - DeadSpace / 2; //28 is the dead space before open outhouse
        }

        private void LoadContent()
        {
            outhouseClosed = Level.Content.Load<Texture2D>("Objects/OuthouseClosed");
            outhouseOpen = Level.Content.Load<Texture2D>("Objects/OuthouseOpen");

            //~28 blank pixels on the side of open outhouse so both images line up (open/closed)
            int width = (int)(outhouseOpen.Width - DeadSpace);
            int left = DeadSpace;
            int height = (int)(outhouseOpen.Height);
            int top = 0;

            //Apply
            localBounds = new Rectangle(left, top, width, height);
        }

        public void OutHouseOpenClose(bool isOpen)
        {
            this.isOpen = isOpen;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 DrawPostion = Position;
            DrawPostion.X -= level.Player.Position.X - Main.BackBufferWidth / 2;
            DrawPostion.Y -= level.Player.Position.Y - level.Player.HoverDistance - Main.BackBufferHeight / Tile.VerticalScale;


            //Onscreen test
            if (level.IsOnScreen(DrawPostion, outhouseOpen.Width, outhouseOpen.Height))
            {
                //Draw open/closed
                if (isOpen)
                    spriteBatch.Draw(outhouseOpen, DrawPostion, Color.White);
                else
                    spriteBatch.Draw(outhouseClosed, DrawPostion, Color.White);

                //Draw collision boxes
                if (Main.DebugMode)
                {
                    if (collisionBoxLine == null)
                    {
                        collisionBoxLine = new LinePrimitive(spriteBatch.GraphicsDevice, Level);

                        collisionBoxLine.AddVector(new Vector2(BoundingRectangle.X, BoundingRectangle.Y));

                        collisionBoxLine.AddVector(new Vector2(BoundingRectangle.X + BoundingRectangle.Width, BoundingRectangle.Y));
                        collisionBoxLine.AddVector(new Vector2(BoundingRectangle.X + BoundingRectangle.Width, BoundingRectangle.Y + BoundingRectangle.Height));
                        collisionBoxLine.AddVector(new Vector2(BoundingRectangle.X, BoundingRectangle.Y + BoundingRectangle.Height));

                        collisionBoxLine.AddVector(new Vector2(BoundingRectangle.X, BoundingRectangle.Y));

                    }

                    collisionBoxLine.Render(spriteBatch, Color.White);
                }
            }
        }
    }
}
