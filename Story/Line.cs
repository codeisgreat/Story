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
    class LinePrimitive
    {
        private Texture2D Pixel;
        private List<Vector2> Vectors = new List<Vector2>();
        public Vector2 Position = Vector2.Zero;

        public int CountVectors
        {
            get
            {
                return Vectors.Count;
            }
        }

        public Level Level
        {
            get { return level; }
        }
        private Level level;

        public LinePrimitive(GraphicsDevice Device, Level level)
        {
            Color[] Pixels = new Color[1];
            Pixel = new Texture2D(Device, 1, 1, false, SurfaceFormat.Color);
            Pixels[0] = Color.White;
            Pixel.SetData<Color>(Pixels);

            this.level = level;
        }

        ~LinePrimitive()
        {

        }

        public void AddVector(Vector2 vector)
        {
            Vectors.Add(vector);
        }

        public void InsertVector(int Index, Vector2 Vector)
        {
            this.Vectors.Insert(Index, Vector);
        }

        public void RemoveVector(Vector2 vector)
        {
            Vectors.Remove(vector);
        }

        public void ClearVectors()
        {
            Vectors.Clear();
        }

        public void Render(SpriteBatch SpriteBatch, Color Color)
        {
            if (Vectors.Count < 2) return; for (int i = 1; i < Vectors.Count; i++)
            {
                Vector2 Vector1 = (Vector2)Vectors[i - 1]; 
                Vector2 Vector2 = (Vector2)Vectors[i];
                Vector2 PositionOffset = new Vector2(level.Player.Position.X - Main.BackBufferWidth / 2, level.Player.Position.Y - level.Player.HoverDistance - Main.BackBufferHeight / Tile.VerticalScale);
                float Distance = Vector2.Distance(Vector1, Vector2);
                float Angle = (float)Math.Atan2((double)(Vector2.Y - Vector1.Y), (double)(Vector2.X - Vector1.X));

                SpriteBatch.Draw(Pixel, Position - PositionOffset + Vector1, null, Color, Angle, Vector2.Zero, new Vector2(Distance, 2), SpriteEffects.None, 0);
            }
        }
    }
}