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
    class CollisionObject
    {

        public LinePrimitive line;

        public Vector2[] Coords = new Vector2[4];
        public float Left;
        public float Right;
        public float Top;
        public float Bottom;
        public float Width;
        public float Height;
        public float X;
        public float Y;
        public bool Liquid = false;
        public bool Passable = false;
        public bool Damaging = false;
        public bool Moving = false;
        public int Damage = 0;
        public Vector2 Movement = Vector2.Zero;
        public Vector2 MinOffset = Vector2.Zero;
        public Vector2 MaxOffset = Vector2.Zero;
        public Vector2 Start;
        public int DirectionX = 1;
        public int DirectionY = 1;
        public float VelocityX = 0f;
        public float VelocityY = 0f;

        public Color Color;
        public bool complete = false;

        //public CollisionType ObjectType;
        public bool IsTriangle;
        public bool IsNPCOnly;

        private Texture2D VertexNode;
        private float VertexNodeRadius = 0;

        public Level Level
        {
            get { return level; }
        }
        Level level;

        public CollisionObject(float x, float y, Level level, bool IsTriangle)// CollisionType ObjectType)
        {
            //this.ObjectType = ObjectType;
            this.IsTriangle = IsTriangle;

            //Yolo
            x = (int)x;
            y = (int)y;

            Coords[0] = new Vector2(x, y);
            Coords[1] = new Vector2(x, y);
            Coords[2] = new Vector2(x, y);
            Coords[3] = new Vector2(x, y);

            Left = Coords[2].X;
            Right = Coords[3].X;
            Top = Coords[0].Y;
            Bottom = Coords[3].Y;
            Width = Right - Left;
            Height = Bottom - Top;

            X = Left + Main.BackBufferWidth / 2;
            Y = Top;

            Start.X = X - Main.BackBufferWidth / 2;
            Start.Y = Y;

            this.level = level;

            LoadContent();
        }
        //Empty constructor
        public CollisionObject() { }

        private void LoadContent()
        {
            VertexNode = level.Content.Load<Texture2D>("Debug/VertexNode");
            VertexNodeRadius = VertexNode.Width / 2;
        }

        public void UpdateObject()
        {
            Left = Coords[2].X;
            Right = Coords[3].X;
            Top = Coords[0].Y;
            Bottom = Coords[3].Y;
            Width = Right - Left;
            Height = Bottom - Top;

            X = Left + Main.BackBufferWidth / 2;
            Y = Top;

            Start.X = X - Main.BackBufferWidth / 2;
            Start.Y = Y;

            if (IsNPCOnly)
                Color = Color.Blue;
            else if (Liquid)
                Color = Color.Pink;
            else if (Passable)
                Color = Color.LightGreen;
            else if (Damaging)
                Color = Color.Gray;
            else
                Color = Color.Yellow;
        }

        public bool ChangeLines = false;

        private void UpdateLines(GraphicsDevice Device)
        {
            //Create lines between each verticies
            line = new LinePrimitive(Device, level);
            line.AddVector(Coords[0]);
            line.AddVector(Coords[2]);
            line.AddVector(Coords[3]);
            line.AddVector(Coords[1]);
            line.AddVector(Coords[0]); //loop back around to complete drawing

            ChangeLines = false;
        }

        public void Draw(SpriteBatch SpriteBatch)
        {
            if (ChangeLines == true)
                UpdateLines(SpriteBatch.GraphicsDevice);

            line.Render(SpriteBatch, Color);

            Vector2 PositionOffset = new Vector2(level.Player.Position.X - Main.BackBufferWidth / 2 + VertexNodeRadius,
                level.Player.Position.Y - level.Player.HoverDistance - Main.BackBufferHeight / Tile.VerticalScale + VertexNodeRadius);

            for (int ecx = 0; ecx < 4; ecx++)
                SpriteBatch.Draw(VertexNode, Coords[ecx] - PositionOffset, Color.White);
        }
    }
}
