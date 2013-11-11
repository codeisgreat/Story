using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Story
{
    class WorldObject
    {

        public Texture2D ObjectTexture;
        public Vector2 Position;
        public LayerDepth layerDepth;
        public String TextureName;
        public Vector2 Movement = Vector2.Zero;
        public int ObjectID
        {
            get { return objectID; }
            set
            {
                DrawColor = Color.White;
                if (value == 3 || value == 4)
                {
                    DrawColor.R = 128;
                    DrawColor.G = 128;
                    DrawColor.B = 128;
                    DrawColor.A = 128;
                }
                objectID = value;
            }
        } public int objectID;

        public bool Moving = false;
        public Vector2 MinOffset = Vector2.Zero;
        public Vector2 MaxOffset = Vector2.Zero;
        public Vector2 Start;
        public int DirectionX = 1;
        public int DirectionY = 1;
        public float VelocityX = 0f;
        public float VelocityY = 0f;

        public Color DrawColor = Color.White;

        public WorldObject(int ObjectID, String TextureName, Texture2D ObjectTexture, Vector2 Position)
        {
            this.ObjectID = ObjectID;
            this.TextureName = TextureName;
            this.ObjectTexture = ObjectTexture;
            this.Position = Position;
            Start = Position;
        }

        //Tests if object is in screen bounds to draw
        public bool IsOnScreen(Vector2 Position, int Width, int Height)
        {
            if (Position.X + Width > 0 && Position.Y + Height > 0 &&
                 Position.X - Width < Main.BackBufferWidth && Position.Y - Height < Main.BackBufferHeight)
                return true;
            return false;
        }

        public void Draw(SpriteBatch SpriteBatch, Vector2 PlayerPosition, float HoverDistance)
        {
            Vector2 DrawPosition = new Vector2(Position.X - (PlayerPosition.X - Main.BackBufferWidth / 2), Position.Y - (PlayerPosition.Y - HoverDistance - Main.BackBufferHeight / Tile.VerticalScale));
            if (IsOnScreen(DrawPosition, ObjectTexture.Width, ObjectTexture.Height))
                SpriteBatch.Draw(ObjectTexture, DrawPosition, DrawColor);
        }

        public enum LayerDepth
        {
            Background,
            Normal,
            Foreground
        }
    }
}
