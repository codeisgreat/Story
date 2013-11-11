using System;
using Microsoft.Xna.Framework;

namespace Story
{
    struct Circle
    {
        private Vector2 Center;
        private float Radius;

        public Circle(Vector2 Position, float Radius)
        {
            Center = Position;
            this.Radius = Radius;
        }

        public bool Intersects(Rectangle Rectangle)
        {
            Vector2 V = new Vector2(MathHelper.Clamp(Center.X, Rectangle.Left, Rectangle.Right),
                                    MathHelper.Clamp(Center.Y, Rectangle.Top, Rectangle.Bottom));

            Vector2 Direction = Center - V;
            float DistanceSquared = Direction.LengthSquared();

            return ((DistanceSquared > 0) && (DistanceSquared < Radius * Radius));
        }
    }
}
