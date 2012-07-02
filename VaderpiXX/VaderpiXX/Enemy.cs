using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace VaderpiXX
{
    class Enemy : ILevel
    {
        #region Members

        private Sprite enemySprite;
        private Vector2 gunOffset = new Vector2(25, 25);
        private Vector2 currentTargetPosition;

        private const float DEFAULT_SPEED = 175.0f;
        public const int DEFAULT_UFO_SPEED_MIN = 150;
        public const int DEFAULT_UFO_SPEED_MAX = 200;
        private float speed;

        private const int EnemyRadiusEasy = 19;
        private const int EnemyRadiusSpeeder = 18;
        private const int EnemyRadiusMedium = 18;

        public const float INIT_HITPOINTS = 1.0f;

        private readonly Rectangle EasySource = new Rectangle(0, 200,
                                                              50, 50);
        private readonly Rectangle MediumSource = new Rectangle(0, 250,
                                                                50, 50);
        private readonly Rectangle HardSource = new Rectangle(350, 200,
                                                              50, 50);
        private readonly Rectangle SpeederSource = new Rectangle(350, 250,
                                                                 50, 50);
        private readonly Rectangle TankSource = new Rectangle(350, 150,
                                                              50, 50);
        private readonly Rectangle UfoSource = new Rectangle(0, 500,
                                                              70, 32);

        private const int EasyFrameCount = 4;
        private const int MediumFrameCount = 6;
        private const int HardFrameCount = 6;
        private const int SpeederFrameCount = 6;
        private const int TankFrameCount = 6;
        private const int UfoFrameCount = 5;

        private const float InitShotChanceEasy = 0.15f;
        private const float InitShotChanceSpeeder = 0.3f;
        private const float InitShotChanceMedium = 0.6f;

        private Vector2 previousCenter = Vector2.Zero;

        private float hitPoints;

        private EnemyType type;

        private int killScore;

        private readonly float initialShotChance;
        private float shotChance;

        private float opacity = 1.0f;

        #endregion

        #region Constructors

        private Enemy(Texture2D texture, Vector2 location,
                     float speed, int killScore, float shotChance,
                     int collisionRadius, EnemyType type)
        {
            Rectangle initialFrame = new Rectangle();
            int frameCount = 0;

            switch (type)
            {
                case EnemyType.Easy:
                    initialFrame = EasySource;
                    frameCount = EasyFrameCount;
                    break;
                case EnemyType.Medium:
                    initialFrame = MediumSource;
                    frameCount = MediumFrameCount;
                    break;
                case EnemyType.Speeder:
                    initialFrame = SpeederSource;
                    frameCount = SpeederFrameCount;
                    break;
                case EnemyType.Ufo:
                    initialFrame = UfoSource;
                    frameCount = UfoFrameCount;
                    break;
            }

            enemySprite = new Sprite(location,
                                     texture,
                                     initialFrame,
                                     Vector2.Zero);

            for (int x = 1; x < frameCount; x++)
            {
                EnemySprite.AddFrame(new Rectangle(initialFrame.X + (x * initialFrame.Width),
                                                   initialFrame.Y,
                                                   initialFrame.Width,
                                                   initialFrame.Height));
                
            }
            
            previousCenter = enemySprite.Center;
            currentTargetPosition = enemySprite.Center;
            EnemySprite.CollisionRadius = collisionRadius;

            if (type != EnemyType.Ufo)
                this.enemySprite.RotateTo(Vector2.UnitY);

            this.speed = speed;

            this.killScore = killScore;

            this.initialShotChance = shotChance;
            this.shotChance = shotChance;

            this.type = type;

            this.hitPoints = INIT_HITPOINTS;
        }

        #endregion

        #region Methods

        public static Enemy CreateEasyEnemy(Texture2D texture, Vector2 location)
        {
            Enemy enemy = new Enemy(texture,
                                    location,
                                    DEFAULT_SPEED,
                                    10,
                                    InitShotChanceEasy,
                                    Enemy.EnemyRadiusEasy,
                                    EnemyType.Easy);

            return enemy;
        }

        public static Enemy CreateMediumEnemy(Texture2D texture, Vector2 location)
        {
            Enemy enemy = new Enemy(texture,
                                location,
                                DEFAULT_SPEED,
                                20,
                                InitShotChanceMedium,
                                Enemy.EnemyRadiusMedium,
                                EnemyType.Medium);

            return enemy;
        }

        public static Enemy CreateSpeederEnemy(Texture2D texture, Vector2 location)
        {
            Enemy enemy = new Enemy(texture,
                                location,
                                DEFAULT_SPEED,
                                30,
                                InitShotChanceSpeeder,
                                Enemy.EnemyRadiusSpeeder,
                                EnemyType.Speeder);

            return enemy;
        }

        public static Enemy CreateUfoEnemy(Texture2D texture, Vector2 location)
        {
            Enemy enemy = new Enemy(texture,
                                location,
                                DEFAULT_UFO_SPEED_MIN,
                                0,
                                0.3f,
                                0,
                                EnemyType.Ufo);

            enemy.enemySprite.BoundingXPadding = 10;

            return enemy;
        }

        public void SetCenter(Vector2 center)
        {
            this.enemySprite.Location = new Vector2(center.X - enemySprite.Source.Width / 2,
                                                    center.Y - enemySprite.Source.Height / 2);
            currentTargetPosition = center;
        }

        public void GoToTarget(Vector2 target)
        {
            this.currentTargetPosition = target;
        }

        public bool TargetReached()
        {
            if (Vector2.Distance(EnemySprite.Center, currentTargetPosition) <
                5.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Reset()
        {
            this.hitPoints = INIT_HITPOINTS;
            this.EnemySprite.TintColor = Color.White;
        }

        public bool IsActive()
        {
            return !IsDestroyed;
        }

        public void SetLevel(int lvl)
        {
            this.shotChance = initialShotChance + initialShotChance * (lvl - 1) * 0.035f;
        }

        public void Update(GameTime gameTime)
        {
            if (!IsDestroyed)
            {
                if (!TargetReached())
                {
                    Vector2 heading = currentTargetPosition - enemySprite.Center;

                    if (heading != Vector2.Zero)
                    {
                        heading.Normalize();
                    }

                    heading *= speed;
                    enemySprite.Velocity = heading;
                    previousCenter = enemySprite.Center;

                    if (enemySprite.Location.Y <= 40)
                        enemySprite.TintColor = Color.Red;
                    else if (enemySprite.Location.Y < 333)
                        enemySprite.TintColor = Color.White;
                    else
                        enemySprite.TintColor = Color.Lime;
                }
                else
                {
                    enemySprite.Velocity = Vector2.Zero;
                }

                if (opacity < 1.0f)
                    opacity += 0.05f;
                else if (opacity > 1.0f)
                    opacity = 1.0f;

                

                enemySprite.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive())
            {
                Color tint = enemySprite.TintColor;
                float renderOpacity = Math.Max(0.0f, opacity);
                enemySprite.TintColor *= renderOpacity;

                enemySprite.Draw(spriteBatch);

                enemySprite.TintColor = tint;
            }
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            // Enemy sprite
            enemySprite.Activated(reader);

            this.currentTargetPosition = new Vector2(Single.Parse(reader.ReadLine()),
                                               Single.Parse(reader.ReadLine()));

            this.speed = Single.Parse(reader.ReadLine());

            this.previousCenter = new Vector2(Single.Parse(reader.ReadLine()),
                                                Single.Parse(reader.ReadLine()));

            this.hitPoints = Single.Parse(reader.ReadLine());

            this.type = (EnemyType)Enum.Parse(type.GetType(), reader.ReadLine(), false);

            this.killScore = Int32.Parse(reader.ReadLine());

            this.shotChance = Single.Parse(reader.ReadLine());

            this.opacity = Single.Parse(reader.ReadLine());
        }

        public void Deactivated(StreamWriter writer)
        {
            // Enemy sprite
            enemySprite.Deactivated(writer);

            writer.WriteLine(currentTargetPosition.X);
            writer.WriteLine(currentTargetPosition.Y);

            writer.WriteLine(speed);

            writer.WriteLine(previousCenter.X);
            writer.WriteLine(previousCenter.Y);

            writer.WriteLine(hitPoints);

            writer.WriteLine(type);


            writer.WriteLine(killScore);

            writer.WriteLine(shotChance);

            writer.WriteLine(opacity);
        }

        #endregion

        #region Properties

        public float Speed
        {
            get
            {
                return speed;
            }
            set
            {
                this.speed = value;
            }
        }

        public Sprite EnemySprite
        {
            get
            {
                return this.enemySprite;
            }
        }

        public EnemyType Type
        {
            get
            {
                return this.type;
            }
        }

        public Vector2 GunOffset
        {
            get
            {
                return this.gunOffset;
            }
        }

        public int KillScore
        {
            get
            {
                return this.killScore;
            }
        }

        public float ShotChance
        {
            get
            {
                return this.shotChance;
            }
        }

        public float HitPoints
        {
            get
            {
                return this.hitPoints;
            }
            set
            {
                this.hitPoints = value;
            }
        }

        public bool IsDestroyed
        {
            get
            {
                return this.hitPoints <= 0.0f;
            }
        }

        public float Opacity
        {
            get
            {
                return opacity;
            }
            set
            {
                this.opacity = value;
            }
        }

        #endregion
    }
}
