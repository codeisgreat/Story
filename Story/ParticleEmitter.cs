using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Linq;
using System.Text;

namespace Story
{
    class ParticleEmitter
    {
        Random Random = new Random();

        public Particle[] Particles;
        public bool CreateEffect = false;
        public int next = 0;

        public Vector2 StartPosition;   //Start position of particle
        public Color ParticleColor;     //Color of particle
        public Vector2 MinPosition;     //Position lower limit
        public Vector2 MaxPosition;     //Position upper limit
        public Vector2 MinVelocity;     //Min velocity
        public Vector2 MaxVelocity;     //Max velocity
        public Vector2 MinOffset;       //Min position offset
        public Vector2 MaxOffset;       //Max position offset
        public int MaxParticles;        //Max number on screen at once
        public float ParticleRate;      //Rate of creation
        public int ParticleSize;        //Size of particle (x & y)
        public float RotationSpeed;     //Speed of rotation
        public float OpacitySpeed;      //Fade speed
        public float ScaleSpeed;        //Speed of rescale
        public float ScaleMax;          //Max scale
        public bool KillOnMaxScale;     //Kill particle after too large?
        public float LiveTime;

        public float CurrentTime = 0.0f;

        //Giant constructor with all of the needed particle emitter information
        public ParticleEmitter(Vector2 StartPosition, Color ParticleColor, Vector2 MinPosition, Vector2 MaxPosition,
             Vector2 MinVelocity, Vector2 MaxVelocity, Vector2 MinOffset, Vector2 MaxOffset, int MaxParticles, int ParticleRate,
            int ParticleSize, float RotationSpeed, float OpacitySpeed, float ScaleSpeed, float ScaleMax, bool KillOnMaxScale, float LiveTime)
        {
            this.StartPosition = StartPosition;
            this.ParticleColor = ParticleColor;
            this.MinPosition = MinPosition;
            this.MaxPosition = MaxPosition;
            this.MinVelocity = MinVelocity;
            this.MaxVelocity = MaxVelocity;
            this.MinOffset = MinOffset;
            this.MaxOffset = MaxOffset;
            this.MaxParticles = MaxParticles;
            this.ParticleRate = ParticleRate;
            this.ParticleSize = ParticleSize;
            this.RotationSpeed = RotationSpeed;
            this.OpacitySpeed = OpacitySpeed;
            this.ScaleSpeed = ScaleSpeed;
            this.ScaleMax = ScaleMax;
            this.KillOnMaxScale = KillOnMaxScale;
            this.LiveTime = LiveTime;

            Particles = new Particle[MaxParticles];
        }

        public void LoadContent(Texture2D EffectTexture)
        {
            LoadParticles(EffectTexture);
        }

        public void LoadParticles(Texture2D Texture)
        {
            Particles = new Particle[MaxParticles];

            for (int p = 0; p < Particles.Length; p++)
            {
                Particles[p] = new Particle();
                Particles[p].Position = StartPosition;
                Particles[p].Texture = Texture;
                Particles[p].color = ParticleColor;
                Particles[p].width = ParticleSize;
                Particles[p].height = ParticleSize;
            }

        }
        
        public void Update(GameTime gameTime)
        {
            //CREATE NEW PARTICLES
            if (CreateEffect == true)
            {
                CurrentTime += gameTime.ElapsedGameTime.Milliseconds;

                if (CurrentTime > LiveTime && LiveTime != 0.0f)
                    EndEffect();

                for (int i = 0; i < ParticleRate; i++)
                {
                    //Set initial particle values
                    Particles[next].opacity = ParticleColor.A;
                    Particles[next].color = ParticleColor;
                    Particles[next].scale = 0.0f;
                    Particles[next].alive = true;
                    //Set position/velocity
                    Particles[next].Position = StartPosition;
                    Particles[next].Position.X += RandomInt((int)MinOffset.X, (int)MaxOffset.X);
                    Particles[next].Position.Y += RandomInt((int)MinOffset.Y, (int)MaxOffset.Y);
                    Particles[next].velocity.X = (float)RandomDouble(MinVelocity.X, MaxVelocity.X);
                    Particles[next].velocity.Y = (float)RandomDouble(MinVelocity.Y, MaxVelocity.Y);
                    Particles[next].opacity = Particles[next].color.A;

                    if (++next == MaxParticles - 1)
                        next = 0;
                }
            }

            //UPDATE PARTICLE POSITION
            float EndMultiplier = 1.0f; //Used to speed up disappearance of particles when effect ends
            if (!CreateEffect)
                EndMultiplier = 2.5f;

            //Update all active particles
            for (int i = 0; i < MaxParticles; i++)
            {
                //Update alive only
                if (Particles[i].alive)
                {
                    //Fade
                    Particles[i].opacity -= OpacitySpeed * gameTime.ElapsedGameTime.Milliseconds * EndMultiplier;
                    if (Particles[i].opacity < 0f)
                    {
                        //kill particle
                        Particles[i].alive = false;
                        Particles[i].opacity = 0f;
                    }
                    
                    //Rotate
                    Particles[i].rotation += RotationSpeed * gameTime.ElapsedGameTime.Milliseconds;

                    if (Particles[i].scale < ScaleMax)
                        Particles[i].scale += ScaleSpeed * gameTime.ElapsedGameTime.Milliseconds;
                    else if (KillOnMaxScale)
                        Particles[i].alive = false;

                    if (Particles[i].alive)
                    {
                        //Update position & travel distance
                        Particles[i].Position = Particles[i].Position + Particles[i].velocity;
                    }
                }
            }
        }

        private void ResetPositions()
        {
            for (int p = 0; p < Particles.Length; p++)
            {
                Particles[p].Position = StartPosition;
                Particles[p].scale = 0.0f;
                Particles[p].opacity = Particles[p].color.A;
            }
        }

        public void StartEffect()
        {
            CreateEffect = true;
            next = 0;
        }

        public void EndEffect()
        {
            CurrentTime = 0.0f;
            CreateEffect = false;
        }

        public int RandomInt(int min, int max)
        {
            return Random.Next(min, max + 1);
        }

        public double RandomDouble(double start, double end)
        {
            return (Random.NextDouble() * Math.Abs(end - start)) + start;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 OffSet)
        {
            //End old spritebatch to start a new one with parameters
            spriteBatch.End();

            Rectangle DrawRectangle = new Rectangle();
            //Draw
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            for (int P = 0; P < Particles.Length; P++)
            {
                if (Particles[P].alive == true)
                {
                    DrawRectangle.X = (int)Particles[P].Position.X - (int)OffSet.X;
                    DrawRectangle.Y = (int)Particles[P].Position.Y - (int)OffSet.Y;
                    DrawRectangle.Width = (int)(Particles[P].width * Particles[P].scale);
                    DrawRectangle.Height = (int)(Particles[P].height * Particles[P].scale);
                    if (DrawRectangle.X >= MinPosition.X && DrawRectangle.Y >= MinPosition.Y
                        && DrawRectangle.X + DrawRectangle.Width <= MaxPosition.X && DrawRectangle.Y + DrawRectangle.Height <= MaxPosition.Y)
                        Particles[P].Draw(spriteBatch, DrawRectangle);
                    else
                        Particles[P].alive = false;
                }
            }

            //End & begin to any spritebatch clear effects
            spriteBatch.End();
            spriteBatch.Begin();
        }


    }
}
