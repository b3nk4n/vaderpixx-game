using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace VaderpiXX
{
    class AsteroidManager
    {
        #region Members

        private int screenWidth = 800;
        private int screenHeight = 480;

        private Rectangle initialFrame;
        private int asteroidFrames;
        private Texture2D texture;

        private List<WallAsteroid> asteroids = new List<WallAsteroid>(42);

        private Random rand = new Random();

        private bool isActive = true;

        public const int CRASH_POWER_MIN = 50;
        public const int CRASH_POWER_MAX = 66;

        #endregion

        #region Constructors

        public AsteroidManager(Texture2D texture, Rectangle initialFrame,
                               int asteroidFrames, int screenWidth, int screenHeight)
        {
            this.texture = texture;
            this.initialFrame = initialFrame;
            this.asteroidFrames = asteroidFrames;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;

            initWalls();
        }

        

        #endregion

        #region Methods
        
        private void initWalls()
        {
            initWallAtPosition(95);
            initWallAtPosition(285);
            initWallAtPosition(465);
            initWallAtPosition(655);
        }

        private void initWallAtPosition(int x)
        {
            //AddAsteroid(new Vector2(x - 15, 363));
            //AddAsteroid(new Vector2(x, 363));
            //AddAsteroid(new Vector2(x + 15, 363));

            //AddAsteroid(new Vector2(x - 30, 376));
            //AddAsteroid(new Vector2(x - 15, 376));
            //AddAsteroid(new Vector2(x, 376));
            //AddAsteroid(new Vector2(x + 15, 376));
            //AddAsteroid(new Vector2(x + 30, 376));

            //AddAsteroid(new Vector2(x - 30, 389));
            //AddAsteroid(new Vector2(x + 30, 389));



            AddAsteroid(new Vector2(x - 15, 362));
            AddAsteroid(new Vector2(x, 362));
            AddAsteroid(new Vector2(x + 15, 362));

            AddAsteroid(new Vector2(x - 30, 372));
            AddAsteroid(new Vector2(x - 15, 372));
            AddAsteroid(new Vector2(x, 372));
            AddAsteroid(new Vector2(x + 15, 372));
            AddAsteroid(new Vector2(x + 30, 372));

            AddAsteroid(new Vector2(x - 30, 381));
            AddAsteroid(new Vector2(x - 15, 381));
            AddAsteroid(new Vector2(x, 381));
            AddAsteroid(new Vector2(x + 15, 381));
            AddAsteroid(new Vector2(x + 30, 381));

            AddAsteroid(new Vector2(x - 30, 390));
            AddAsteroid(new Vector2(x + 30, 390));
        }

        public void AddAsteroid(Vector2 location)
        {
            WallAsteroid newAsteroid = new WallAsteroid(location,
                                                        texture,
                                                        initialFrame,
                                                        WallAsteroid.HITS_TO_DESTROY_ELEMENT);

            newAsteroid.TintColor = Color.Lime;

            for (int x = 1; x < asteroidFrames; x++)
            {
                newAsteroid.AddFrame(new Rectangle(initialFrame.X + (x * initialFrame.Width),
                                                   initialFrame.Y,
                                                   initialFrame.Width,
                                                   initialFrame.Height));
            }
            
            newAsteroid.Rotation = MathHelper.ToRadians((float)rand.Next(0, 360));
            newAsteroid.CollisionRadius = 15;
            Asteroids.Add(newAsteroid);
        }

        private void AddAsteroidAfterResume(float locationX, float locationY, float rotation,
                                            int remainingHits)
        {
            WallAsteroid newAsteroid = new WallAsteroid(new Vector2(locationX, locationY),
                                                        texture,
                                                        initialFrame,
                                                        remainingHits);

            for (int x = 1; x < asteroidFrames; x++)
            {
                newAsteroid.AddFrame(new Rectangle(initialFrame.X + (x * initialFrame.Width),
                                                   initialFrame.Y,
                                                   initialFrame.Width,
                                                   initialFrame.Height));
            }

            newAsteroid.Rotation = rotation;
            newAsteroid.CollisionRadius = 15;
            Asteroids.Add(newAsteroid);
        }

        public void Clear()
        {
            Asteroids.Clear();
        }

        private Vector2 randomLocation()
        {
            Vector2 location = Vector2.Zero;
            bool locationOK = true;
            int tryCount = 0;

            do
            {
                locationOK = true;
                switch (rand.Next(0, 3))
                {
                    case 0:
                        location.X = -initialFrame.Width;
                        location.Y = rand.Next(0, screenHeight / 2);
                        break;

                    case 1:
                        location.X = screenWidth;
                        location.Y = rand.Next(0, screenHeight / 2);
                        break;

                    case 2:
                        location.X = rand.Next(0, screenWidth);
                        location.Y = -initialFrame.Height;
                        break;
                }

                foreach (var asteroid in Asteroids)
                {
                    if (asteroid.isBoxColliding(new Rectangle((int)location.X,
                                                              (int)location.Y,
                                                              initialFrame.Width,
                                                              initialFrame.Height)))
                    {
                        locationOK = false;
                    }
                }

                ++tryCount;

                if (tryCount > 5 && locationOK == false)
                {
                    location = new Vector2(-500, -500);
                    locationOK = true;
                }
            } while (locationOK == false);

            return location;
        }

        public void Update(GameTime gameTime)
        {
            foreach (var asteroid in Asteroids)
            {
                asteroid.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var asteroid in Asteroids)
            {
                asteroid.Draw(spriteBatch);
            }
        }

        public void Reset(bool resetAll)
        {
            if (resetAll)
                foreach (var asteroid in Asteroids)
                {
                    asteroid.Destroy();
                } 

            resetOpacity();
            
            foreach (var asteroid in Asteroids)
            {
                asteroid.Reset();
            } 
        }

        private void resetOpacity()
        {
            for (int i = 0; i < asteroids.Count; ++i)
            {
                if (asteroids[i].IsDestroyed)
                    asteroids[i].Opacity = -(asteroids[i].Location.X * 0.001f); 
            }
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            int count = Int32.Parse(reader.ReadLine());

            asteroids.Clear();

            for (int i = 0; i < count; ++i)
            {
                AddAsteroidAfterResume(Single.Parse(reader.ReadLine()),
                                       Single.Parse(reader.ReadLine()),
                                       Single.Parse(reader.ReadLine()),
                                       Int32.Parse(reader.ReadLine()));
                asteroids[i].WallActivated(reader);
            }

            this.isActive = Boolean.Parse(reader.ReadLine());
        }

        public void Deactivated(StreamWriter writer)
        {
            int realAsteroidsCount = asteroids.Count;
            
            writer.WriteLine(realAsteroidsCount);
            
            for (int i = 0; i < realAsteroidsCount; ++i)
            {
                writer.WriteLine(asteroids[i].Location.X);
                writer.WriteLine(asteroids[i].Location.Y);
                writer.WriteLine(asteroids[i].Rotation);
                writer.WriteLine(asteroids[i].RemainingSustainingHits);
                asteroids[i].WallDeactivated(writer);
            }

            writer.WriteLine(this.isActive);
        }

        #endregion

        #region Properties

        public List<WallAsteroid> Asteroids
        {
            get
            {
                return this.asteroids;
            }
        }

        public bool IsActive
        {
            set
            {
                this.isActive = value;
            }
            get
            {
                return this.isActive;
            }
        }

        #endregion
    }
}
