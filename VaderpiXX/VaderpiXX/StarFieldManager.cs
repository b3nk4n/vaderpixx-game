using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace VaderpiXX
{
    class StarFieldManager
    {
        #region Members

        private List<Sprite> stars;
        private int screenWidth = 800;
        private int screenHeight = 480;
        private Random rand = new Random();
        private Color[] colors = { Color.White,
                                   Color.Green,
                                   Color.GreenYellow,
                                   Color.Lime,
                                   Color.SlateGray};
        private float speedFactor = 1.0f;
        private float currentSpeedFactor = 1.0f;

        #endregion

        #region Constructors

        public StarFieldManager(int screenWidth, int screenHeight, int starCount,
                                Vector2 starVelocity, Texture2D texture, Rectangle frameRect)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;

            stars = new List<Sprite>(starCount + 1);

            for (int x = 0; x < starCount; x++)
            {
                stars.Add(new Sprite(new Vector2(rand.Next(0, screenWidth), rand.Next(0, screenHeight)),
                                     texture,
                                     frameRect,
                                     starVelocity));
                Color starColor = colors[rand.Next(0, colors.Length)];
                starColor *= (float)rand.Next(30, 80) / 100.0f;
                stars[stars.Count - 1].TintColor = starColor;
            }
        }

        #endregion

        #region Methods

        public void Update(GameTime gameTime)
        {
            adjustCurrentSpeedFactor();

            foreach (var star in stars)
            {
                Vector2 oldVel = star.Velocity;
                star.Velocity = oldVel * currentSpeedFactor;
                star.Update(gameTime);
                star.Velocity = oldVel;

                if (star.Location.Y > screenHeight)
                {
                    star.Location = new Vector2(rand.Next(0, screenWidth), 0);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var star in stars)
            {
                star.Draw(spriteBatch);
            }
        }

        private void adjustCurrentSpeedFactor()
        {
            if (speedFactor > currentSpeedFactor)
            {
                currentSpeedFactor += 0.25f;
            }
            else if (speedFactor < currentSpeedFactor)
            {
                currentSpeedFactor -= 0.025f;
            }
        }

        #endregion

        public float SpeedFactor
        {
            set
            {
                this.speedFactor = value;
            }
            get
            {
                return this.speedFactor;
            }
        }
    }
}
