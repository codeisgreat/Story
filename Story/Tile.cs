using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Story
{

    /// <summary>
    /// Stores the appearance and collision behavior of a tile.
    /// </summary>
    struct Tile
    {
        public const int Width = 64;
        public const int Height = 48;
        public const float VerticalScale = 1.5f;
        public static readonly Vector2 Size = new Vector2(Width, Height);

        public Texture2D Texture;
        public bool Transparent;

        public Tile(Texture2D Texture, bool Transparent)
        {
            this.Texture = Texture;
            this.Transparent = Transparent;
        }
    }
}
