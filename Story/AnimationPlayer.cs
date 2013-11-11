using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Story
{
    /// <summary>
    /// Controls playback of an Animation.
    /// </summary>
    struct AnimationPlayer
    {

        public Animation Animation;
        public int FrameIndex;
        private float time;

        public Vector2 Origin
        {
            get { return new Vector2(Animation.FrameWidth / 2.0f, Animation.FrameHeight); }
        }

        public void PlayAnimation(Animation Animation)
        {
            // If this animation is already running, do not restart it.
            if (this.Animation == Animation)
                return;

            // Start the new animation.
            this.Animation = Animation;
            this.FrameIndex = 0;
            this.time = 0.0f;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, float rotation, SpriteEffects spriteEffects, bool Underwater)
        {
            if (Animation == null)
                throw new NotSupportedException("No animation is currently playing.");

            //Process passing time.
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while (time > Animation.FrameTime)
            {
                time -= Animation.FrameTime;

                //Advance the frame index; looping or clamping as appropriate.
                if (Animation.IsLooping)
                    FrameIndex = (FrameIndex + 1) % Animation.FrameCount;
                else
                    FrameIndex = Math.Min(FrameIndex + 1, Animation.FrameCount - 1);
            }

            //Calculate the source rectangle of the current frame.
            Rectangle Source = new Rectangle(FrameIndex * Animation.Texture.Height, 0, Animation.Texture.Height, Animation.Texture.Height);

            Color DrawColor = new Color(255, 255, 255, 255);
            if (Underwater)
            {
                DrawColor.R = 255;
                DrawColor.G = 255;
                DrawColor.B = 255;
                DrawColor.A = 255;
            }

            //Draw the current frame.
            spriteBatch.Draw(Animation.Texture, position, Source, DrawColor, rotation, Origin, 1.0f, spriteEffects, 1f);
        }
    }
}
