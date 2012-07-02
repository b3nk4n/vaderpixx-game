using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using Microsoft.Xna.Framework.Media;

namespace VaderpiXX
{
    public static class SoundManager
    {
        #region Members

        private static SettingsManager settings;

        private static List<SoundEffect> explosions = new List<SoundEffect>();
        private static int explosionsCount = 4;

        private static SoundEffect playerShot;
        private static SoundEffect enemyShot;

        private static SoundEffect ufoLowpitchSound;
        private static SoundEffectInstance ufoHighpitchSound;

        private static int currentMoveSound;
        private const int InvaderMoveSoundsCount = 4;
        private static SoundEffect[] invaderMoveSounds = new SoundEffect[InvaderMoveSoundsCount];

        private static SoundEffect playerExplosionSound;

        private static SoundEffect startGameSound;
        private static SoundEffect spawnEnemySound;

        private static List<SoundEffect> hitSounds = new List<SoundEffect>();
        private static int hitSoundsCount = 6;

        private static List<SoundEffect> asteroidHitSounds = new List<SoundEffect>();
        private static int asteroidHitSoundsCount = 5;

        private static Random rand = new Random();

        private static Song backgroundSound;
        //private static SoundEffectInstance backgroundSound;

        #endregion

        #region Methods

        public static void Initialize(ContentManager content)
        {
            try
            {
                settings = SettingsManager.GetInstance();

                playerShot = content.Load<SoundEffect>(@"Sounds\Shot1");
                enemyShot = content.Load<SoundEffect>(@"Sounds\Shot2");

                ufoLowpitchSound = content.Load<SoundEffect>(@"Sounds\si_ufo_lowpitch");
                ufoHighpitchSound = content.Load<SoundEffect>(@"Sounds\si_ufo_highpitch").CreateInstance();
                ufoHighpitchSound.IsLooped = true;

                playerExplosionSound = content.Load<SoundEffect>(@"Sounds\si_playerexplosion");

                startGameSound = content.Load<SoundEffect>(@"Sounds\si_startgame");
                spawnEnemySound = content.Load<SoundEffect>(@"Sounds\si_spawnenemy");

                for (int x = 1; x <= InvaderMoveSoundsCount; x++)
                {
                    invaderMoveSounds[x - 1] = content.Load<SoundEffect>(@"Sounds\si_fastinvader"
                                                             + x.ToString());
                }

                for (int x = 1; x <= explosionsCount; x++)
                {
                    explosions.Add(content.Load<SoundEffect>(@"Sounds\Explosion"
                                                             + x.ToString()));
                }

                for (int x = 1; x <= hitSoundsCount; x++)
                {
                    hitSounds.Add(content.Load<SoundEffect>(@"Sounds\Hit"
                                                             + x.ToString()));
                }

                for (int x = 1; x <= asteroidHitSoundsCount; x++)
                {
                    asteroidHitSounds.Add(content.Load<SoundEffect>(@"Sounds\AsteroidHit"
                                                             + x.ToString()));
                }

                backgroundSound = content.Load<Song>(@"Sounds\GameSound");
            }
            catch
            {
                Debug.WriteLine("SoundManager: Content not found.");
            }

            Reset();
        }

        public static void PlayExplosion()
        {
            try
            {
                SoundEffectInstance s = explosions[rand.Next(0, explosionsCount)].CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play explosion failed.");
            }
        }

        public static void PlayPlayerShot()
        {
            try
            {
                SoundEffectInstance s = playerShot.CreateInstance();
                s.Volume = 0.75f * settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play player shot failed.");
            }
        }

        public static void PlayEnemyShot()
        {
            try
            {
                SoundEffectInstance s = enemyShot.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play enemy shot failed.");
            }
        }

        public static void PlayHitSound()
        {
            try
            {
                SoundEffectInstance s = hitSounds[rand.Next(0, hitSoundsCount)].CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play explosion failed.");
            }
        }

        public static void PlayAsteroidHitSound()
        {
            try
            {
                SoundEffectInstance s = asteroidHitSounds[rand.Next(0, asteroidHitSoundsCount)].CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play explosion failed.");
            }
        }

        public static void PlayUfoLowpitchSound()
        {
            try
            {
                SoundEffectInstance s = ufoLowpitchSound.CreateInstance();
                s.Volume = settings.GetSfxValue() * 0.5f;
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play ufo sound failed.");
            }
        }

        public static void PlayUfoHighpitchSound()
        {
            try
            {
                ufoHighpitchSound.Volume = settings.GetSfxValue() * 0.4f;
                if (ufoHighpitchSound.State != SoundState.Playing)
                    ufoHighpitchSound.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play ufo sound failed.");
            }
        }

        public static void StopUfoHighpitchSound()
        {
            try
            {   
                if (ufoHighpitchSound.State == SoundState.Playing)
                    ufoHighpitchSound.Stop();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play ufo sound failed.");
            }
        }

        public static void PlayMoveSound()
        {
            try
            {
                SoundEffectInstance s = invaderMoveSounds[currentMoveSound].CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();

                currentMoveSound = (++currentMoveSound) % InvaderMoveSoundsCount;
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play ufo sound failed.");
            }
        }

        public static void PlayPlayerExplosionSound()
        {
            try
            {
                SoundEffectInstance s = playerExplosionSound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play player explosion sound failed.");
            }
        }

        public static void PlayStartGameSound()
        {
            try
            {
                SoundEffectInstance s = startGameSound.CreateInstance();
                s.Volume = settings.GetSfxValue() * 0.9f;
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play start game sound failed.");
            }
        }

        public static void PlaySpawnEnemySound()
        {
            try
            {
                SoundEffectInstance s = spawnEnemySound.CreateInstance();
                s.Volume = settings.GetSfxValue();
                s.Play();
            }
            catch
            {
                Debug.WriteLine("SoundManager: Play spawn enemy sound failed.");
            }
        }

        public static void PlayBackgroundSound()
        {
            try
            {
                if (MediaPlayer.GameHasControl)
                {
                    MediaPlayer.Play(backgroundSound);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = settings.GetMusicValue();
                }
            }
            catch (UnauthorizedAccessException)
            {
                // play no music...
            }
            catch (InvalidOperationException)
            {
                // play no music (because of Zune on PC)
            }
        }

        public static void RefreshMusicVolume()
        {
            float val = settings.GetMusicValue();
            MediaPlayer.Volume = val;
        }

        public static void Reset()
        {
            currentMoveSound = 0;
        }

        private static float musicFadeValue = 1.0f;
        private static bool musicOn = true;

        public static void Update()
        {
            if (musicOn)
            {
                if (musicFadeValue <= 1.0f)
                    musicFadeValue += 0.025f;
            }
            else
            {
                if (musicFadeValue >= 0.0f)
                    musicFadeValue -= 0.025f;
            }



            if (musicFadeValue >= 0.0f && musicFadeValue <= 1.0f)
            {
                float val = settings.GetMusicValue();
                MediaPlayer.Volume = val * musicFadeValue;
            }
        }

        public static void EnableMusic()
        {
            musicOn = true;
        }

        public static void DisableMusic()
        {
            musicOn = false;
        }

        #endregion
    }
}
