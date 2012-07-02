using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.IO.IsolatedStorage;

namespace VaderpiXX
{
    class InstructionManager
    {
        #region Members

        private float progressTimer = 0.0f;

        public enum InstructionStates { 
            Welcome,
            ChangeControlls,         
            MovementTouch1,
            MovementTouch2,
            MovementTilt,    
            Lives,
            Score,
            KillEnemies, 
            ReachBottom, 
            KillUfo,
            GoodLuck, 
            ReturnWithBackButton};

        private InstructionStates state = InstructionStates.Welcome;

        private const float WelcomeLimit = 2.0f;
        private const float ChangeControllsLimit = 6.0f;
        private const float MovementTouch1Limit = 14.0f;
        private const float MovementTouch2Limit = 22.0f;
        private const float MovementTiltLimit = 30.0f;
        private const float LivesLimit = 33.0f;
        private const float ScoreLimit = 36.0f;
        private const float KillEnemiesLimit = 39.0f;
        private const float ReachBottomLimit = 42.0f;
        private const float KillUfoLimit = 45.0f;

        private SpriteFont font;

        private Texture2D texture;

        private Rectangle screenBounds;

        private Rectangle areaSource = new Rectangle(660, 60, 100, 100);
        private Rectangle crosshairSource = new Rectangle(760, 60, 100, 100);
        private Rectangle controlArrowSource = new Rectangle(860, 60, 100, 100);
        private Rectangle arrowRightSource = new Rectangle(100, 460, 40, 20);

        private Rectangle screenDestination = new Rectangle(10, 75, 780, 395);
        private Rectangle leftSideDestination = new Rectangle(10, 320, 380, 150);
        private Rectangle rightSideDestination = new Rectangle(410, 320, 380, 150);
        private Rectangle leftQuaterDestination = new Rectangle(10, 320, 120, 150);
        private Rectangle rightQuaterDestination = new Rectangle(150, 320, 120, 150);
        private Rectangle topDestination = new Rectangle(10, 75, 780, 230);
        private Rectangle livesDestination = new Rectangle(585, 0, 40, 20);
        private Rectangle scoreDestination = new Rectangle(165, 0, 40, 20);

        private Rectangle crosshairTouch1Destination = new Rectangle(600, 345, 100, 100);
        private Rectangle crosshairTopDestination = new Rectangle(350, 150, 100, 100);
        private Rectangle controlArrowLeftDestination = new Rectangle(125, 345, 100, 100);
        private Rectangle controlArrowRightDestination = new Rectangle(575, 345, 100, 100);
        private Rectangle controlArrowLeftQuaterDestination = new Rectangle(20, 345, 100, 100);
        private Rectangle controlArrowRightQuaterDestination = new Rectangle(160, 345, 100, 100);

        private Color areaTint = Color.Lime * 0.5f;
        private Color arrowTint = Color.Lime * 0.8f;

        private AsteroidManager asteroidManager;

        private EnemyManager enemyManager;

        private PlayerManager playerManager;

        private SettingsManager settingsManager;

        CollisionManager collisionManager;

        private readonly string WelcomeText = "WELCOME TO VADERPIXX!";
        private readonly string ChangeControllsText = "CHOOSE BETWEEN 3 CONTROL TYPES";
        private readonly string MovementTouch1Text = "TOUCH1 - CLASSIC GAMEPAD STYLE";
        private readonly string MovementTouch2Text = "TOUCH2 - ALTERNATIVE";
        private readonly string MovementTiltText = "TILT - ACCELEROMETER CONTROLS";
        private readonly string LivesText = "THE  H U D  DISPLAYS YOUR REMAINING LIVES...";
        private readonly string ScoreText = "...AND YOUR CURRENT SCORE";
        private readonly string KillEnemiesText = "KILL ALL ENEMIES TO CLEAR THE STAGE!";
        private readonly string ReachBottomText = "DO NOT LET THEM INVADE THE EARTH!!!";
        private readonly string KillUfoText = "KILL THE MOTHERSHIP FOR BONUS SCORE!";
        private readonly string GoodLuckText = "GOOD LUCK COMMANDER!";
        private readonly string ReturnWithBackButtonText = "PRESS  B A C K  TO RETURN...";

        private static bool hasDoneInstructions = false;

        #endregion

        #region Constructors

        public InstructionManager(Texture2D texture, SpriteFont font, Rectangle screenBounds,
                                  AsteroidManager asteroidManager, PlayerManager playerManager,
                                  EnemyManager enemyManager, CollisionManager collisionManager)
        {
            this.texture = texture;
            this.font = font;
            this.screenBounds = screenBounds;

            this.asteroidManager = asteroidManager;
            this.asteroidManager.Reset(true);

            this.enemyManager = enemyManager;
            this.enemyManager.Reset();

            this.playerManager = playerManager;
            this.playerManager.Reset();

            this.settingsManager = SettingsManager.GetInstance();

            this.collisionManager = collisionManager;

            loadHasDoneInstructions();
        }

        #endregion

        #region Methods

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            progressTimer += elapsed;

            if (playerManager.IsDestroyed)
            {
                this.state = InstructionStates.ReturnWithBackButton;

                asteroidManager.Update(gameTime);
                if (!enemyManager.ReachedBottom())
                    enemyManager.Update(gameTime);
                collisionManager.Update();
                return;
            }
            else if (enemyManager.ReachedBottom())
            {
                playerManager.Kill();

                EffectManager.AddLargeExplosion(playerManager.playerSprite.Center,
                                                playerManager.playerSprite.Velocity / 10,
                                                Color.Lime,
                                                false);
                SoundManager.PlayPlayerExplosionSound();

                VibrationManager.Vibrate(0.5f);
            }
                
            else if (progressTimer < WelcomeLimit)
            {
                playerManager.CanFire = false;
                this.state = InstructionStates.Welcome;
            }
            else if (progressTimer < ChangeControllsLimit)
            {
                this.state = InstructionStates.ChangeControlls;
            }
            else if (progressTimer < MovementTouch1Limit)
            {
                playerManager.CanFire = true;
                this.state = InstructionStates.MovementTouch1;
                settingsManager.ControlType = SettingsManager.ControlTypeValues.GameController;
            }
            else if (progressTimer < MovementTouch2Limit)
            {
                this.state = InstructionStates.MovementTouch2;
                settingsManager.ControlType = SettingsManager.ControlTypeValues.Touch;
            }
            else if (progressTimer < MovementTiltLimit)
            {
                this.state = InstructionStates.MovementTilt;
                settingsManager.ControlType = SettingsManager.ControlTypeValues.Tilt;
            }
            else if (progressTimer < LivesLimit)
            {
                this.state = InstructionStates.Lives;
            }
            else if (progressTimer < ScoreLimit)
            {
                this.state = InstructionStates.Score;
            }
            else if (progressTimer < KillEnemiesLimit)
            {
                this.state = InstructionStates.KillEnemies;

                collisionManager.Update();
                asteroidManager.Update(gameTime);
                enemyManager.Update(gameTime);
            }
            else if (progressTimer < ReachBottomLimit)
            {
                this.state = InstructionStates.ReachBottom;
                collisionManager.Update();
                asteroidManager.Update(gameTime);
                enemyManager.Update(gameTime);
            }
            else if (progressTimer < KillUfoLimit)
            {
                this.state = InstructionStates.KillUfo;
                collisionManager.Update();
                asteroidManager.Update(gameTime);
                enemyManager.Update(gameTime);
            }
            else
            {
                this.state = InstructionStates.GoodLuck;

                asteroidManager.Update(gameTime);
                enemyManager.Update(gameTime);
                collisionManager.Update();
            }

            playerManager.Update(gameTime);

            if (state == InstructionStates.Welcome ||
                state == InstructionStates.ChangeControlls)
                playerManager.playerSprite.Location = new Vector2(375, playerManager.playerSprite.Location.Y);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            switch(this.state)
            {
                case InstructionStates.Welcome:
                    playerManager.Draw(spriteBatch);

                    drawLimeCenteredText(spriteBatch, WelcomeText);
                    break;

                case InstructionStates.ChangeControlls:
                    playerManager.Draw(spriteBatch);
                    
                    drawLimeCenteredText(spriteBatch, ChangeControllsText);
                    break;

                case InstructionStates.MovementTouch1:
                    playerManager.Draw(spriteBatch);

                    spriteBatch.Draw(texture,
                                     leftQuaterDestination,
                                     areaSource,
                                     arrowTint);

                    spriteBatch.Draw(texture,
                                     rightQuaterDestination,
                                     areaSource,
                                     arrowTint);

                    spriteBatch.Draw(texture,
                                     rightSideDestination,
                                     areaSource,
                                     arrowTint);

                    spriteBatch.Draw(texture,
                                     controlArrowLeftQuaterDestination,
                                     controlArrowSource,
                                     arrowTint);

                    spriteBatch.Draw(texture,
                                     controlArrowRightQuaterDestination,
                                     controlArrowSource,
                                     arrowTint,
                                     0.0f,
                                     Vector2.Zero,
                                     SpriteEffects.FlipHorizontally,
                                     1.0f);

                    spriteBatch.Draw(texture,
                                     crosshairTouch1Destination,
                                     crosshairSource,
                                     arrowTint);

                    drawLimeCenteredText(spriteBatch, MovementTouch1Text);
                    break;

                case InstructionStates.MovementTouch2:
                    playerManager.Draw(spriteBatch);

                    spriteBatch.Draw(texture,
                                     leftSideDestination,
                                     areaSource,
                                     arrowTint);
                    spriteBatch.Draw(texture,
                                     rightSideDestination,
                                     areaSource,
                                     arrowTint);
                    spriteBatch.Draw(texture,
                                     topDestination,
                                     areaSource,
                                     arrowTint);

                    spriteBatch.Draw(texture,
                                     controlArrowLeftDestination,
                                     controlArrowSource,
                                     arrowTint);

                    spriteBatch.Draw(texture,
                                     controlArrowRightDestination,
                                     controlArrowSource,
                                     arrowTint,
                                     0.0f,
                                     Vector2.Zero,
                                     SpriteEffects.FlipHorizontally,
                                     1.0f);

                    spriteBatch.Draw(texture,
                                     crosshairTopDestination,
                                     crosshairSource,
                                     arrowTint);

                    drawLimeCenteredText(spriteBatch, MovementTouch2Text);
                    break;

                case InstructionStates.MovementTilt:
                    playerManager.Draw(spriteBatch);

                    spriteBatch.Draw(texture,
                                     screenDestination,
                                     areaSource,
                                     arrowTint);

                    spriteBatch.Draw(texture,
                                     crosshairTopDestination,
                                     crosshairSource,
                                     arrowTint);

                    drawLimeCenteredText(spriteBatch, MovementTiltText);
                    break;

                case InstructionStates.Lives:
                    playerManager.Draw(spriteBatch);

                    spriteBatch.Draw(texture,
                                     livesDestination,
                                     arrowRightSource,
                                     arrowTint);
                    drawLimeCenteredText(spriteBatch, LivesText);
                    break;

                case InstructionStates.Score:
                    playerManager.Draw(spriteBatch);

                    spriteBatch.Draw(texture,
                                     scoreDestination,
                                     arrowRightSource,
                                     arrowTint,
                                     0.0f,
                                     Vector2.Zero,
                                     SpriteEffects.FlipHorizontally,
                                     1.0f);
                    drawLimeCenteredText(spriteBatch, ScoreText);
                    break;

                case InstructionStates.KillEnemies:
                    asteroidManager.Draw(spriteBatch);
                    enemyManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    drawLimeCenteredText(spriteBatch, KillEnemiesText);
                    break;

                case InstructionStates.ReachBottom:
                    asteroidManager.Draw(spriteBatch);
                    enemyManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    drawLimeCenteredText(spriteBatch, ReachBottomText);
                    break;

                case InstructionStates.KillUfo:
                    asteroidManager.Draw(spriteBatch);
                    enemyManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    drawLimeCenteredText(spriteBatch, KillUfoText);
                    break;

                case InstructionStates.GoodLuck:
                    asteroidManager.Draw(spriteBatch);
                    enemyManager.Draw(spriteBatch);
                    playerManager.Draw(spriteBatch);

                    drawLimeCenteredText(spriteBatch, GoodLuckText);
                    break;

                case InstructionStates.ReturnWithBackButton:
                    asteroidManager.Draw(spriteBatch);
                    enemyManager.Draw(spriteBatch);

                    drawLimeCenteredText(spriteBatch, ReturnWithBackButtonText);
                    break;
            }
        }

        private void drawLimeCenteredText(SpriteBatch spriteBatch, string text)
        {
            spriteBatch.DrawString(font,
                                   text,
                                   new Vector2(screenBounds.Width / 2 - font.MeasureString(text).X / 2,
                                               25),
                                   Color.Lime);
        }

        public void Reset()
        {
            this.progressTimer = 0.0f;
            this.state = InstructionStates.Welcome;
        }

        public void SaveHasDoneInstructions()
        {

            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream("instructions2.txt", FileMode.Create, isf))
                {
                    using (StreamWriter sw = new StreamWriter(isfs))
                    {
                        sw.WriteLine(hasDoneInstructions);

                        sw.Flush();
                        sw.Close();
                    }
                }
            }
        }

        private void loadHasDoneInstructions()
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                bool hasExisted = isf.FileExists(@"instructions2.txt");

                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(@"instructions2.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, isf))
                {
                    if (hasExisted)
                    {
                        using (StreamReader sr = new StreamReader(isfs))
                        {
                            hasDoneInstructions = Boolean.Parse(sr.ReadLine());
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = new StreamWriter(isfs))
                        {
                            sw.WriteLine(hasDoneInstructions);

                            // ... ? 
                        }
                    }
                }

                // Delete the old file
                if (isf.FileExists(@"instructions.txt"))
                    isf.DeleteFile(@"instructions.txt");
            }
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            this.progressTimer = Single.Parse(reader.ReadLine());
            hasDoneInstructions = Boolean.Parse(reader.ReadLine());
        }

        public void Deactivated(StreamWriter writer)
        {
            writer.WriteLine(progressTimer);
            writer.WriteLine(hasDoneInstructions);
        }

        #endregion

        #region Properties

        public static bool HasDoneInstructions
        {
            get
            {
                return hasDoneInstructions;
            }
            set
            {
                hasDoneInstructions = value;
            }
        }

        #endregion
    }
}
