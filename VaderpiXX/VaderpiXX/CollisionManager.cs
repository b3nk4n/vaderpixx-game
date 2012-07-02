using System;
using Microsoft.Xna.Framework;

namespace VaderpiXX
{
    class CollisionManager
    {
        #region Members

        private AsteroidManager asteroidManager;
        private PlayerManager playerManager;
        private EnemyManager enemyManager;
        public static readonly Vector2 OFF_SCREEN = new Vector2(-500, -500);
        private Vector2 shotToAsteroidImpact = new Vector2(0, -20);

        private const string INFO_HEALTH25 = "+25% Repair-Kit";
        private const string INFO_HEALTH50 = "+50% Repair-Kit";
        private const string INFO_HEALTH100 = "+100% Repair-Kit";
        private const string INFO_COOLINGWATER = "Cooling Water";
        private const string INFO_EXTRASHIP = "Extra Spaceship!";
        private const string INFO_SUPERLASER = "+1 Super-Laser";
        private const string INFO_ROCKETS = "+2 C.A.R.L.I-Rockets";
        private const string INFO_NUKE = "!!! N U K E !!!";
        private const string INFO_BONUSSCORE_LOW = "+1000";
        private const string INFO_BONUSSCORE_MEDIUM = "+2500";
        private const string INFO_BONUSSCORE_HIGH = "+5000";
        private const string INFO_SCOREMULTI_LOW = "+50% Score-Multiplier";
        private const string INFO_SCOREMULTI_MEDIUM = "+75% Score-Multiplier";
        private const string INFO_SCOREMULTI_HIGH = "+100% Score-Multiplier";
        private const string INFO_SCOREMULTI_LOST = "Score-Multiplier Lost!";
        private const string INFO_OUT_OF_CONTROL = "We get out of control!";
        private const string INFO_SLOW = "Hyper Eninge damaged!";
        private const string INFO_OVERHEAT = "Overheating Problem!";
        private const string INFO_SHIELDS = "Activated Shields";
        private const string INFO_OVERDRIVE = "Overdrive!";
        private const string INFO_UNDERDRIVE = "Underdrive!";

        Random rand = new Random();

        #endregion

        #region Constructors

        public CollisionManager(AsteroidManager asteroidManager, PlayerManager playerManager,
                                EnemyManager enemyManager)
        {
            this.asteroidManager = asteroidManager;
            this.playerManager = playerManager;
            this.enemyManager = enemyManager;
        }

        #endregion

        #region Methods

        private void checkShotToEnemyCollisions()
        {
            foreach (var enemy in enemyManager.Enemies)
            {
                Vector2 location = Vector2.Zero;
                Vector2 velocity = Vector2.Zero;

                foreach (var shot in playerManager.PlayerShotManager.Shots)
                {
                    if (shot.IsCircleColliding(enemy.EnemySprite.Center,
                                               enemy.EnemySprite.CollisionRadius) &&
                        !enemy.IsDestroyed)
                    {
                        enemy.HitPoints -= playerManager.ShotPower;

                        location = shot.Location;
                        velocity = shot.Velocity;

                        shot.Location = OFF_SCREEN;
                        
                        break; // Skip next comparisons, because they are probably neccessary or will be checked in the next try
                    }
                }

                if (location != Vector2.Zero)
                {
                    playerManager.IncreasePlayerScore(enemy.KillScore);

                    EffectManager.AddLargeExplosion(enemy.EnemySprite.Center,
                                                    enemy.EnemySprite.Velocity / 10,
                                                    enemy.EnemySprite.TintColor,
                                                    true);
                }
            }
        }

        private void checkShotToUfoCollisions()
        {
            Vector2 location = Vector2.Zero;
            Vector2 velocity = Vector2.Zero;

            foreach (var shot in playerManager.PlayerShotManager.Shots)
            {
                Rectangle shotBox = new Rectangle((int)shot.Center.X - shot.CollisionRadius,
                                                  (int)shot.Center.Y - shot.CollisionRadius,
                                                  shot.CollisionRadius * 2,
                                                  shot.CollisionRadius * 2);

                if (enemyManager.Ufo.EnemySprite.isBoxColliding(shotBox) &&
                    !enemyManager.Ufo.IsDestroyed)
                {
                    enemyManager.Ufo.HitPoints -= playerManager.ShotPower;

                    location = shot.Location;
                    velocity = shot.Velocity;

                    shot.Location = OFF_SCREEN;

                    

                    break; // Skip next comparisons, because they are probably neccessary or will be checked in the next try
                }
            }

            if (location != Vector2.Zero)
            {
                long ufoScore = enemyManager.GetUfoScore();

                playerManager.IncreasePlayerScore(ufoScore);
                
                ZoomTextManager.ShowInfo(ufoScore.ToString(), enemyManager.Ufo.EnemySprite.Center.X);

                EffectManager.AddLargeExplosion(enemyManager.Ufo.EnemySprite.Center,
                                                enemyManager.Ufo.EnemySprite.Velocity / 10,
                                                enemyManager.Ufo.EnemySprite.TintColor,
                                                true);
                SoundManager.PlayUfoLowpitchSound();
            }
        }

        private void checkPlayerShotToAsteroidCollisions()
        {
            foreach (var shot in playerManager.PlayerShotManager.Shots)
            {
                foreach (var asteroid in asteroidManager.Asteroids)
                {
                    if (!asteroid.IsDestroyed && shot.IsCircleColliding(asteroid.Center,
                                                                        asteroid.CollisionRadius))
                    {
                        EffectManager.AddSparksEffect(shot.Center,
                                                      shot.Velocity,
                                                      asteroid.Velocity,
                                                      Color.Lime,
                                                      true);

                        asteroid.DecreaseRemainingSustainingHits();

                        if (asteroid.IsDestroyed)
                        {
                            EffectManager.AddLargeSparksEffect(asteroid.Center,
                                                              asteroid.Velocity,
                                                              asteroid.Velocity,
                                                              Color.Lime,
                                                              true);
                        }

                        shot.Location = OFF_SCREEN;

                        break; // Skip next comparisons, because they are probably neccessary or will be checked in the next try
                    }
                }
            }
        }

        private void checkEnemyShotToAsteroidCollisions()
        {
            foreach (var shot in enemyManager.EnemyShotManager.Shots)
            {
                foreach (var asteroid in asteroidManager.Asteroids)
                {
                    if (!asteroid.IsDestroyed && shot.IsCircleColliding(asteroid.Center,
                                                                        asteroid.CollisionRadius))
                    {
                        EffectManager.AddSparksEffect(shot.Location,
                                                      shot.Velocity,
                                                      asteroid.Velocity,
                                                      Color.Lime,
                                                      true);

                        asteroid.DecreaseRemainingSustainingHits();

                        if (asteroid.IsDestroyed)
                        {
                            EffectManager.AddLargeSparksEffect(asteroid.Center,
                                                              asteroid.Velocity,
                                                              asteroid.Velocity,
                                                              Color.Lime,
                                                              true);
                        }

                        shot.Location = OFF_SCREEN;

                        break; // Skip next comparisons, because they are probably neccessary or will be checked in the next try
                    }
                }
            }
        }

        private void checkEnemyShotToPlayerCollisions()
        {
            foreach (var shot in enemyManager.EnemyShotManager.Shots)
            {
                if (shot.IsCircleColliding(playerManager.playerSprite.Center,
                                           playerManager.playerSprite.CollisionRadius))
                {
                    playerManager.Kill();

                    EffectManager.AddLargeExplosion(playerManager.playerSprite.Center,
                                                    playerManager.playerSprite.Velocity / 10,
                                                    Color.Lime,
                                                    false);
                    SoundManager.PlayPlayerExplosionSound();

                    VibrationManager.Vibrate(0.5f);
                    
                    shot.Location = OFF_SCREEN;
                }
            }
        }

        private void checkAsteroidToEnemiesCollisions()
        {
            foreach (var asteroid in asteroidManager.Asteroids)
            {
                foreach (var enemy in enemyManager.Enemies)
                {
                    if (!asteroid.IsDestroyed && asteroid.IsCircleColliding(enemy.EnemySprite.Center,
                                                                            enemy.EnemySprite.CollisionRadius))
                    {
                        EffectManager.AddLargeSparksEffect(asteroid.Center,
                                                           asteroid.Velocity,
                                                           asteroid.Velocity,
                                                           Color.Lime,
                                                           true);

                        asteroid.Destroy();

                        break; // Skip next comparisons, because they are probably neccessary or will be checked in the next try
                    }
                }
            }
        }

        private void checkPlayerShotToEnemyShotCollisions()
        {
            foreach (var shotPlayer in playerManager.PlayerShotManager.Shots)
            {
                foreach (var shotEnemy in enemyManager.EnemyShotManager.Shots)
                {
                    if (shotPlayer.IsCircleColliding(shotEnemy.Center,
                                                     shotEnemy.CollisionRadius + 2))
                    {
                        EffectManager.AddSparksEffect(shotPlayer.Center,
                                                      shotPlayer.Velocity,
                                                      shotEnemy.Velocity,
                                                      shotPlayer.TintColor,
                                                      true);

                        shotPlayer.Location = OFF_SCREEN;
                        shotEnemy.Location = OFF_SCREEN;

                        break; // Skip next comparisons, because they are probably neccessary or will be checked in the next try
                    }
                }
            }
        }

        public void Update()
        {
            checkPlayerShotToAsteroidCollisions();
            checkEnemyShotToAsteroidCollisions();
            checkShotToEnemyCollisions();
            checkShotToUfoCollisions();
            checkAsteroidToEnemiesCollisions();
            checkPlayerShotToEnemyShotCollisions();

            if (!playerManager.IsDestroyed)
            {
                checkEnemyShotToPlayerCollisions();
            }
        }

        #endregion
    }
}
