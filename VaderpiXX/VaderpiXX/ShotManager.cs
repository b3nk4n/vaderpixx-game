using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace VaderpiXX
{
    class ShotManager
    {
        #region Members

        public List<Sprite> Shots = new List<Sprite>();
        
        private Rectangle screenBounds;

        private static Texture2D Texture;
        private static Rectangle InitialFrame;
        private static int FrameCount;
        private float shotSpeed;
        private static int CollisionRadius;

        #endregion

        #region Constructors

        public ShotManager(Texture2D texture, Rectangle initialFrame, int frameCount,
                           int collisionRadius, float speed, Rectangle screenBounds)
        {
            ShotManager.Texture = texture;
            ShotManager.InitialFrame = initialFrame;
            ShotManager.FrameCount = frameCount;
            ShotManager.CollisionRadius = collisionRadius;
            this.shotSpeed = speed;
            this.screenBounds = screenBounds;
        }

        #endregion

        #region Methods

        public void FireShot(Vector2 location, Vector2 velocity, bool playerFired, Color tintColor, bool sound)
        {
            Sprite newShot = new Sprite(location,
                                        ShotManager.Texture,
                                        ShotManager.InitialFrame,
                                        velocity);

            newShot.TintColor = tintColor;

            newShot.Velocity *= shotSpeed;
            newShot.RotateTo(velocity);

            for (int x = 0; x < ShotManager.FrameCount; x++)
            {
                newShot.AddFrame(new Rectangle(ShotManager.InitialFrame.X + (x * ShotManager.InitialFrame.Width),
                                               ShotManager.InitialFrame.Y,
                                               ShotManager.InitialFrame.Width,
                                               ShotManager.InitialFrame.Height));
            }
            newShot.CollisionRadius = ShotManager.CollisionRadius;
            Shots.Add(newShot);

            if (sound)
            {
                if (playerFired)
                {
                    SoundManager.PlayPlayerShot();
                }
                else
                {
                    SoundManager.PlayEnemyShot();
                }
            }
        }        

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int x = Shots.Count - 1; x >= 0; --x)
            {
                Shots[x].Update(gameTime);
                if (!screenBounds.Intersects(Shots[x].Destination))
                {
                    if (Shots[x].Location != CollisionManager.OFF_SCREEN)
                    {
                        if (Shots[x].Location.Y < 50) // top
                        {
                            EffectManager.AddLargeSparksEffect(new Vector2(Shots[x].Center.X, -5),
                                                               Shots[x].Velocity,
                                                               Vector2.Zero,
                                                               Color.Red,
                                                               false);
                        }
                        else // bottom
                        {
                            EffectManager.AddLargeSparksEffect(new Vector2(Shots[x].Center.X, 480),
                                                               Shots[x].Velocity,
                                                               Vector2.Zero,
                                                               Color.Lime,
                                                               false);
                        }
                    }

                    Shots.RemoveAt(x);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var shot in this.Shots)
	        {
                if (shot.Location.Y <= 30)
                    shot.TintColor = Color.Red;
                else if (shot.Location.Y < 380)
                    shot.TintColor = Color.White;
                else if (shot.Location.Y < 395)
                    shot.TintColor = new Color(150, 255, 150);
                else if (shot.Location.Y < 410)
                    shot.TintColor = new Color(100, 255, 100);
                else if (shot.Location.Y < 425)
                    shot.TintColor = new Color(50, 255, 50);
                else if (shot.Location.Y < 440)
                    shot.TintColor = new Color(50, 255, 50);
                else
                    shot.TintColor = Color.Lime;

                shot.Draw(spriteBatch);
	        }
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            // Shots
            int shotsCount = Int32.Parse(reader.ReadLine());

            Shots.Clear();

            for (int i = 0; i < shotsCount; ++i)
            {
                Vector2 location = new Vector2(Single.Parse(reader.ReadLine()),
                                               Single.Parse(reader.ReadLine()));
                Vector2 direction = new Vector2(Single.Parse(reader.ReadLine()),
                                                Single.Parse(reader.ReadLine()));

                if (direction != Vector2.Zero)
                    direction.Normalize();

                FireShot(location,
                         direction,
                         true,
                         new Color(Int32.Parse(reader.ReadLine()),
                                   Int32.Parse(reader.ReadLine()),
                                   Int32.Parse(reader.ReadLine()),
                                   Int32.Parse(reader.ReadLine())),
                         false);
            }
        }

        public void Deactivated(StreamWriter writer)
        {
            // Shots
            writer.WriteLine(Shots.Count);

            for (int i = 0; i < Shots.Count; ++i)
            {
                writer.WriteLine(Shots[i].Location.X);
                writer.WriteLine(Shots[i].Location.Y);
                writer.WriteLine(Shots[i].Velocity.X);
                writer.WriteLine(Shots[i].Velocity.Y);
                writer.WriteLine((int)Shots[i].TintColor.R);
                writer.WriteLine((int)Shots[i].TintColor.G);
                writer.WriteLine((int)Shots[i].TintColor.B);
                writer.WriteLine((int)Shots[i].TintColor.A);
            }
        }

        #endregion
    }
}
