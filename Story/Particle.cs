using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using System.Diagnostics;

namespace Story
{
    public class Particle
    {

        private Texture2D texture;
        public Vector2 Position;
        public Vector2 velocity;

        public float rotation;
        public float scale;
        public Color color;
        public int width;
        public int height;
        public Vector2 origin;
        public bool alive;
        public float opacity;

        public Texture2D Texture
        {
            get { return texture; }
            set
            {
                texture = value;
                origin = new Vector2(texture.Width / 2, texture.Height / 2);
            }
        }

        public Particle()
        {
            //Arbitrary defaults
            Position = Vector2.Zero;
            velocity = Vector2.Zero;
            origin = Vector2.Zero;
            color = Color.White;
            rotation = 0f;
            scale = 0.5f;
            opacity = 0f;
            alive = false;
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle DrawRectangle)
        {
            color.A = (byte)opacity;
            spriteBatch.Draw(texture, DrawRectangle,
                new Rectangle(0, 0, texture.Width, texture.Height), color, rotation,
                origin, SpriteEffects.None, 0);
        }

    }
}
