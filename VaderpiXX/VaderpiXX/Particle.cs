using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace VaderpiXX
{
    class Particle : Sprite
    {
        #region Members

        private Vector2 acceleration;
        private float maxSpeed;
        private int initialDuration;
        private int remainingDuration;
        private Color initialColor;
        private Color finalColor;
        
        #endregion

        #region Constructors

        public Particle(Vector2 location, Texture2D texture, Rectangle initialFrame,
                        Vector2 velocity, Vector2 acceleration, float maxSpeed,
                        int duration, Color initialColor, Color finalColor)
            : base(location, texture, initialFrame, velocity)
        {
            this.acceleration = acceleration;
            this.maxSpeed = maxSpeed;
            this.initialDuration = duration;
            this.remainingDuration = duration;
            this.initialColor = initialColor;
            this.finalColor = finalColor;
        }

        #endregion

        #region Methods

        public void Reinitialize(Vector2 location, Texture2D texture,
                                 Vector2 velocity, Vector2 acceleration, float maxSpeed,
                                 int duration, Color initialColor, Color finalColor)
        {
            this.Location = location;
            this.Velocity = velocity;
            this.Frame = 0;

            this.acceleration = acceleration;
            this.maxSpeed = maxSpeed;
            this.initialDuration = duration;
            this.remainingDuration = duration;
            this.initialColor = initialColor;
            this.finalColor = finalColor;
        }

        public override void Update(GameTime gameTime)
        {
            if (IsActive)
            {
                velocity += acceleration;

                if (velocity.Length() > maxSpeed)
                {
                    velocity.Normalize();
                    velocity *= maxSpeed;
                }

                TintColor = Color.Lerp(initialColor,
                                       finalColor,
                                       this.DurationProgress);
                remainingDuration--;

                base.Update(gameTime);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                base.Draw(spriteBatch);
            }
            
        }

        #endregion

        #region Activate/Deactivate

        public new void Activated(StreamReader reader)
        {
            // Sprite
            base.Activated(reader);

            this.acceleration.X = Single.Parse(reader.ReadLine());
            this.acceleration.Y = Single.Parse(reader.ReadLine());

            this.maxSpeed = Single.Parse(reader.ReadLine());

            this.initialDuration = Int32.Parse(reader.ReadLine());
            this.remainingDuration = Int32.Parse(reader.ReadLine());

            this.initialColor = new Color(Int32.Parse(reader.ReadLine()),
                                          Int32.Parse(reader.ReadLine()),
                                          Int32.Parse(reader.ReadLine()),
                                          Int32.Parse(reader.ReadLine()));

            this.finalColor = new Color(Int32.Parse(reader.ReadLine()),
                                        Int32.Parse(reader.ReadLine()),
                                        Int32.Parse(reader.ReadLine()),
                                        Int32.Parse(reader.ReadLine()));
        }

        public new void Deactivated(StreamWriter writer)
        {
            // Sprite
            base.Deactivated(writer);

            writer.WriteLine(acceleration.X);
            writer.WriteLine(acceleration.Y);

            writer.WriteLine(maxSpeed);

            writer.WriteLine(initialDuration);
            writer.WriteLine(remainingDuration);

            writer.WriteLine((int)initialColor.R);
            writer.WriteLine((int)initialColor.G);
            writer.WriteLine((int)initialColor.B);
            writer.WriteLine((int)initialColor.A);

            writer.WriteLine((int)finalColor.R);
            writer.WriteLine((int)finalColor.G);
            writer.WriteLine((int)finalColor.B);
            writer.WriteLine((int)finalColor.A);
        }

        #endregion

        #region Properties

        public int ElapsedDuration
        {
            get
            {
                return initialDuration - remainingDuration;
            }
        }

        public float DurationProgress
        {
            get
            {
                return (float)ElapsedDuration / (float)initialDuration;
            }
        }

        public bool IsActive
        {
            get
            {
                return remainingDuration > 0;
            }
        }

        #endregion
    }
}
