using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace VaderpiXX
{
    static class EffectManager
    {
        #region Members

        private const int PRELOADED_EXPLOSION_EFFECTS = 256;
        private const int PRELOADED_POINT_EFFECTS = 2048;

        public static List<Particle> ExplosionEffects = new List<Particle>(PRELOADED_EXPLOSION_EFFECTS);
        public static List<Particle> PointEffects = new List<Particle>(PRELOADED_POINT_EFFECTS);

        private static Random rand = new Random();
        public static Texture2D Texture;
        public static Rectangle ParticleFrame;
        public static List<Rectangle> ExplosionFrames = new List<Rectangle>(8);

        private static Queue<Particle> freeExplosionParticles = new Queue<Particle>(PRELOADED_EXPLOSION_EFFECTS);
        private static Queue<Particle> freePointParticles = new Queue<Particle>(PRELOADED_POINT_EFFECTS);

        #endregion

        #region Methods

        public static void Initialize(Texture2D texture, Rectangle particleFrame,
                                      Rectangle initialExplosionFrame, int explosionFameCount)
        {
            Texture = texture;
            ParticleFrame = particleFrame;
            ExplosionFrames.Clear();

            for (int x = 1; x < explosionFameCount; x++)
            {
                initialExplosionFrame.Offset(initialExplosionFrame.Width, 0);
                ExplosionFrames.Add(initialExplosionFrame);
            }

            // Generate free explosion particles:
            for (int i = 0; i < PRELOADED_EXPLOSION_EFFECTS; i++)
            {
                freeExplosionParticles.Enqueue(new Particle(Vector2.Zero,
                                         Texture,
                                         ExplosionFrames[rand.Next(0, ExplosionFrames.Count - 1)],
                                         Vector2.Zero,
                                         Vector2.Zero,
                                         0.0f,
                                         0,
                                         Color.White,
                                         Color.White));
            }

            // Generate free explosion particles:
            for (int i = 0; i < PRELOADED_POINT_EFFECTS; i++)
            {
                freePointParticles.Enqueue(new Particle(Vector2.Zero,
                                         Texture,
                                         ParticleFrame,
                                         Vector2.Zero,
                                         Vector2.Zero,
                                         0.0f,
                                         0,
                                         Color.White,
                                         Color.White));
            }
        }

        public static Vector2 RandomDirection(float scale)
        {
            Vector2 direction;

            do
            {
                direction = new Vector2(rand.Next(0, 101) - 50,
                                        rand.Next(0, 101) - 50);
            } while (direction.Length() == 0);

            direction.Normalize();
            direction *= scale;

            return direction;
        }

        public static void Update(GameTime gameTime)
        {
            // Explosion effects
            for (int x = ExplosionEffects.Count - 1; x >= 0; --x)
            {
                if (ExplosionEffects[x].IsActive)
                {
                    ExplosionEffects[x].Update(gameTime);
                }
                else
                {
                    freeExplosionParticles.Enqueue(ExplosionEffects[x]);
                    ExplosionEffects.RemoveAt(x);
                }
            }

            // Point effects
            for (int x = PointEffects.Count - 1; x >= 0; --x)
            {
                if (PointEffects[x].IsActive)
                {
                    PointEffects[x].Update(gameTime);
                }
                else
                {
                    freePointParticles.Enqueue(PointEffects[x]);
                    PointEffects.RemoveAt(x);
                }
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            foreach (var particle in ExplosionEffects)
            {
                particle.Draw(spriteBatch);
            }

            foreach (var particle in PointEffects)
            {
                particle.Draw(spriteBatch);
            }
        }

        public static void AddCustomExplosion(Vector2 location, Vector2 momentum, int minPointCount,
                                              int maxPointCount, int minPieceCount, int maxPieceCount,
                                              float pieceSpeedScale, int duration, Color initialColor,
                                              Color finalColor, bool sound)
        {
            //float explosionMaxSpeed = 30.0f;
            float explosionMaxSpeed = pieceSpeedScale;

            int pointSpeedMin = (int)pieceSpeedScale * 2;
            int pointSpeedMax = (int)pieceSpeedScale * 3;

            Vector2 pieceLocation = location - new Vector2(ExplosionFrames[0].Width / 2,
                                                           ExplosionFrames[0].Height / 2);

            int pieceCount = rand.Next(minPieceCount, maxPieceCount + 1);

            for (int x = 0; x < pieceCount; x++)
            {
                if (freeExplosionParticles.Count == 0)
                {
                    ExplosionEffects.Add(new Particle(pieceLocation,
                                         Texture,
                                         ExplosionFrames[rand.Next(0, ExplosionFrames.Count - 1)],
                                         RandomDirection(pieceSpeedScale) + momentum,
                                         Vector2.Zero,
                                         ((float)rand.NextDouble()) * explosionMaxSpeed,
                                         duration,
                                         initialColor,
                                         finalColor));
                }
                else
                {
                    Particle p = freeExplosionParticles.Dequeue();
                    p.Reinitialize(pieceLocation,
                                   Texture,
                                   RandomDirection(pieceSpeedScale) + momentum,
                                   Vector2.Zero,
                                   ((float)rand.NextDouble()) * explosionMaxSpeed,
                                   duration,
                                   initialColor,
                                   finalColor);
                    ExplosionEffects.Add(p);
                }
            }

            int pointsCount = rand.Next(minPointCount, maxPointCount + 1);

            for (int x = 0; x < maxPointCount; x++)
            {
                Vector2 randDirection = RandomDirection((float)rand.Next(pointSpeedMin, pointSpeedMax + 1));
                Vector2 scatterRandDirection = randDirection / randDirection.Length();
                int scatter = rand.Next(0, 10);

                if (freePointParticles.Count == 0)
                {
                    PointEffects.Add(new Particle(location + scatter * scatterRandDirection,
                                             Texture,
                                             ParticleFrame,
                                             randDirection + momentum,
                                             Vector2.Zero,
                                             ((float)rand.NextDouble()) * explosionMaxSpeed,
                                             duration,
                                             initialColor,
                                             finalColor));
                }
                else
                {
                    Particle p = freePointParticles.Dequeue();
                    p.Reinitialize(location + scatter * scatterRandDirection,
                                   Texture,
                                   randDirection + momentum,
                                   Vector2.Zero,
                                   ((float)rand.NextDouble()) * explosionMaxSpeed,
                                   duration,
                                   initialColor,
                                   finalColor);
                    PointEffects.Add(p);
                }
			}

            if (sound)
                SoundManager.PlayExplosion();
        }

        public static void AddExplosion(Vector2 location, Vector2 momentum, Color tint, bool sound)
        {
            AddCustomExplosion(location,
                               momentum,
                               10,
                               20,
                               5,
                               10,
                               20.0f,
                               40, 
                               tint,
                               tint * 0.0f,
                               sound);
        }

        public static void AddLargeExplosion(Vector2 location, Vector2 momentum, Color tint, bool sound)
        {
            AddCustomExplosion(location,
                               momentum,
                               20,
                               30,
                               10,
                               15,
                               25.0f,
                               60,
                               tint,
                               tint * 0.0f,
                               sound);
        }

        public static void AddSparksEffect(Vector2 location, Vector2 impectVelocity, Vector2 momentum, Color tintColor, bool sound)
        {
            int particleCount = rand.Next(5, 10);

            for (int x = 0; x < particleCount; x++)
            {
                Vector2 randDirection = RandomDirection((float)rand.Next(10, 20));
                Vector2 scatterRandDirection = randDirection / randDirection.Length();
                int scatter = rand.Next(0, 5);

                if (freePointParticles.Count == 0)
                {
                    PointEffects.Add(new Particle(location + scatter * scatterRandDirection - (impectVelocity / 60),
                                                  Texture,
                                                  ParticleFrame,
                                                  randDirection + momentum * 0.75f,
                                                  Vector2.Zero,
                                                  40.0f,
                                                  40,
                                                  tintColor,
                                                  tintColor * 0.0f));
                }
                else
                {
                    Particle p = freePointParticles.Dequeue();
                    p.Reinitialize(location + scatter * scatterRandDirection - (impectVelocity / 60),
                                   Texture,
                                   randDirection + momentum * 0.75f,
                                   Vector2.Zero,
                                   40.0f,
                                   40,
                                   tintColor,
                                   tintColor * 0.0f);
                    PointEffects.Add(p);
                }
            }

            if (sound)
                SoundManager.PlayAsteroidHitSound();
        }

        public static void AddLargeSparksEffect(Vector2 location, Vector2 impectVelocity, Vector2 momentum,
                                                Color tintColor, bool sound)
        {
            int particleCount = rand.Next(10, 15);

            for (int x = 0; x < particleCount; x++)
            {
                if (freePointParticles.Count == 0)
                {
                    PointEffects.Add(new Particle(location - (impectVelocity / 60),
                                                  Texture,
                                                  ParticleFrame,
                                                  RandomDirection((float)rand.Next(10, 20)) * 3 + momentum * 0.25f,
                                                  Vector2.Zero,
                                                  60.0f,
                                                  30,
                                                  tintColor * 0.75f,
                                                  tintColor * 0.0f));
                }
                else
                {
                    Particle p = freePointParticles.Dequeue();
                    p.Reinitialize(location - (impectVelocity / 60),
                                   Texture,
                                   RandomDirection((float)rand.Next(10, 20)) * 3 + momentum * 0.25f,
                                   Vector2.Zero,
                                   60.0f,
                                   30,
                                   tintColor * 0.75f,
                                   tintColor * 0.0f);
                    PointEffects.Add(p);
                }
            }

            if (sound)
                SoundManager.PlayHitSound();
        }

        public static void Reset()
        {
            ExplosionEffects.Clear();
            PointEffects.Clear();
        }

        #endregion

        #region Activated / Deactivated

        public static void Activated(StreamReader reader)
        {
            // Explosion effects
            int expCount = Int32.Parse(reader.ReadLine());
            ExplosionEffects.Clear();

            for (int i = 0; i < expCount; ++i)
            {
                Particle p = new Particle(Vector2.Zero,
                                         Texture,
                                         ExplosionFrames[rand.Next(0, ExplosionFrames.Count - 1)],
                                         Vector2.Zero,
                                         Vector2.Zero,
                                         0.0f,
                                         0,
                                         Color.White,
                                         Color.White);
                p.Activated(reader);
                ExplosionEffects.Add(p);
            }

            // Point effects
            int pointsCount = Int32.Parse(reader.ReadLine());
            PointEffects.Clear();

            for (int i = 0; i < pointsCount; ++i)
            {
                Particle p = new Particle(Vector2.Zero,
                                         Texture,
                                         ParticleFrame,
                                         Vector2.Zero,
                                         Vector2.Zero,
                                         0.0f,
                                         0,
                                         Color.White,
                                         Color.White);
                p.Activated(reader);
                PointEffects.Add(p);
            }

            // Free explosion effects
            int freeExpCount = Int32.Parse(reader.ReadLine());
            freeExplosionParticles.Clear();

            for (int i = 0; i < freeExpCount; ++i)
            {
                Particle p = new Particle(Vector2.Zero,
                                         Texture,
                                         ExplosionFrames[rand.Next(0, ExplosionFrames.Count - 1)],
                                         Vector2.Zero,
                                         Vector2.Zero,
                                         0.0f,
                                         0,
                                         Color.White,
                                         Color.White);
                p.Activated(reader);
                freeExplosionParticles.Enqueue(p);
            }

            // Free point effects
            int freePointsCount = Int32.Parse(reader.ReadLine());
            freePointParticles.Clear();

            for (int i = 0; i < freePointsCount; ++i)
            {
                Particle p = new Particle(Vector2.Zero,
                                         Texture,
                                         ParticleFrame,
                                         Vector2.Zero,
                                         Vector2.Zero,
                                         0.0f,
                                         0,
                                         Color.White,
                                         Color.White);
                p.Activated(reader);
                freePointParticles.Enqueue(p);
            }
        }

        public static void Deactivated(StreamWriter writer)
        {
            // Explosion effects
            writer.WriteLine(ExplosionEffects.Count);

            for (int i = 0; i < ExplosionEffects.Count; ++i)
            {
                ExplosionEffects[i].Deactivated(writer);
            }

            // Point effects
            writer.WriteLine(PointEffects.Count);

            for (int i = 0; i < PointEffects.Count; ++i)
            {
                PointEffects[i].Deactivated(writer);
            }

            // Free explosion effects
            writer.WriteLine(freeExplosionParticles.Count);

            foreach (var freeExplosion in freeExplosionParticles)
            {
                freeExplosion.Deactivated(writer);
            }

            // Free point effects
            writer.WriteLine(freePointParticles.Count);

            foreach (var freePoints in freePointParticles)
            {
                freePoints.Deactivated(writer);
            }
        }

        #endregion
    }
}
