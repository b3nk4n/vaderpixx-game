using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;
using System.IO;
using VaderpiXX.Inputs;
using Microsoft.Phone.Applications.Common;

namespace VaderpiXX
{
    class PlayerManager
    {
        #region Members

        public Sprite playerSprite;
        public const float PLAYER_SPEED = 225.0f;
        private Rectangle playerAreaLimit;

        private Vector2 startLocation;

        private long playerScore = 0;

        private const int PLAYER_STARTING_LIVES = 2;
        public const int MAX_PLAYER_LIVES = 3;
        private int livesRemaining = PLAYER_STARTING_LIVES;

        private float shotPower = 50.0f;

        private float hitPoints = 1.0f;
        public const float MaxHitPoints = 1.0f;

        private Vector2 gunOffset = new Vector2(19, 5);
        private float shotTimer = 0.0f;
        private float minShotTimer = 0.4f;
        
        private const int PlayerRadius = 20;
        public ShotManager PlayerShotManager;

        Vector3 currentAccValue = Vector3.Zero;

        Rectangle leftSideScreen;
        Rectangle rightSideScreen;
        Rectangle upperScreen;
        Rectangle screen;

        Rectangle gameControllerLeftScreen;
        Rectangle gameControllerRightScreen;
        Rectangle gameControllerFireScreen;

        private Sprite shieldSprite;
        private readonly Rectangle initialShieldFrame = new Rectangle(250, 400, 70, 70);

        SettingsManager settings = SettingsManager.GetInstance();

        GameInput gameInput;
        private const string ActionLeft = "Left";
        private const string ActionRight = "Right";
        private const string ActionTop = "Top";
        private const string ActionScreen = "Screen";

        private const string ActionControllerLeft = "CtrlLeft";
        private const string ActionControllerRight = "CtrlRight";
        private const string ActionControllerFire = "CtrlFore";

        private int LOCATION_Y = 430;
        private const int MOVE_PADDING_X = 30;

        private bool canFire = false;

        #endregion

        #region Constructors

        public PlayerManager(Texture2D texture, Rectangle initialFrame,
                             int frameCount, Rectangle screenBounds,
                             Vector2 startLocation, GameInput input)
        {
            this.playerSprite = new Sprite(new Vector2(500, 500),
                                           texture,
                                           initialFrame,
                                           Vector2.Zero);

            this.PlayerShotManager = new ShotManager(texture,
                                                     new Rectangle(100, 430, 18, 6),
                                                     4,
                                                     1,
                                                     375.0f,
                                                     screenBounds);

            this.playerAreaLimit = new Rectangle(0,
                                                 screenBounds.Height / 5,
                                                 screenBounds.Width,
                                                 4 * screenBounds.Height / 5);

            for (int x = 0; x < frameCount; x++)
            {
                this.playerSprite.AddFrame(new Rectangle(initialFrame.X + (x * initialFrame.Width),
                                                         initialFrame.Y,
                                                         initialFrame.Width,
                                                         initialFrame.Height));
            }

            playerSprite.CollisionRadius = PlayerRadius;

            AccelerometerHelper.Instance.ReadingChanged += new EventHandler<AccelerometerHelperReadingEventArgs>(OnAccelerometerHelperReadingChanged);
            AccelerometerHelper.Instance.Active = true;

            screen = screenBounds;

            leftSideScreen = new Rectangle(0,
                                           3 * screenBounds.Height / 4,
                                           screenBounds.Width / 2,
                                           screenBounds.Height / 4);

            rightSideScreen = new Rectangle(screenBounds.Width / 2,
                                           3 * screenBounds.Height / 4,
                                           screenBounds.Width / 2,
                                           screenBounds.Height / 4);

            upperScreen = new Rectangle(0, 0,
                                        screenBounds.Width,
                                        screenBounds.Height * 3 / 4);

            gameControllerLeftScreen = new Rectangle(0,
                                           screenBounds.Height / 2,
                                           screenBounds.Width / 6,
                                           screenBounds.Height / 2);

            gameControllerRightScreen = new Rectangle(screenBounds.Width / 6 + 20,
                                           screenBounds.Height / 2,
                                           screenBounds.Width / 6,
                                           screenBounds.Height / 2);

            gameControllerFireScreen = new Rectangle(screenBounds.Width / 2,
                                           screenBounds.Height / 2,
                                           screenBounds.Width / 2,
                                           screenBounds.Height / 2);

            this.startLocation = startLocation;

            this.shieldSprite = new Sprite(Vector2.Zero,
                                           texture,
                                           initialShieldFrame,
                                           Vector2.Zero);

            gameInput = input;
        }

        #endregion

        #region Methods

        public void SetupInputs()
        {
            gameInput.AddTouchTapInput(ActionLeft,
                                       leftSideScreen,
                                       false);
            gameInput.AddTouchTapInput(ActionRight,
                                       rightSideScreen,
                                       false);
            gameInput.AddTouchTapInput(ActionScreen,
                                       screen,
                                       false);
            gameInput.AddTouchTapInput(ActionTop,
                                       upperScreen,
                                       false);

            gameInput.AddTouchTapInput(ActionControllerLeft,
                                       gameControllerLeftScreen,
                                       false);
            gameInput.AddTouchTapInput(ActionControllerRight,
                                       gameControllerRightScreen,
                                       false);
            gameInput.AddTouchTapInput(ActionControllerFire,
                                       gameControllerFireScreen,
                                       false);
        }

        public void Reset()
        {
            this.PlayerShotManager.Shots.Clear();

            this.playerSprite.Location = startLocation;

            this.hitPoints = MaxHitPoints;
        }

        public void ResetRemainingLives()
        {
            this.LivesRemaining = PLAYER_STARTING_LIVES;
        }

        private void fireShot()
        {
            if (shotTimer <= 0.0f && PlayerShotManager.Shots.Count == 0)
            {
                this.PlayerShotManager.FireShot(this.playerSprite.Location + gunOffset,
                                                new Vector2(0, -1),
                                                true,
                                                Color.Lime,
                                                true);
                shotTimer = minShotTimer;
            }
        }

        private void HandleTouchInput(TouchCollection touches, float elapsed)
        {
            bool left = false;
            bool right = false;
            bool fire = false;

            if (touches.Count == 1)
            {
                if (settings.ControlType == SettingsManager.ControlTypeValues.Tilt)
                {
                    if (gameInput.IsPressed(ActionScreen))
                    {
                        fire = true;
                    }
                }
                else
                {
                    if (settings.ControlType == SettingsManager.ControlTypeValues.GameController)
                    {
                        if (gameInput.IsPressed(ActionControllerLeft))
                        {
                            left = true;
                        }
                        if (gameInput.IsPressed(ActionControllerRight))
                        {
                            right = true;
                        }
                        if (gameInput.IsPressed(ActionControllerFire))
                        {
                            fire = true;
                        }
                    }
                    else
                    {
                        if (gameInput.IsPressed(ActionLeft))
                        {
                            left = true;
                        }
                        if (gameInput.IsPressed(ActionRight))
                        {
                            right = true;
                        }
                        if (gameInput.IsPressed(ActionTop))
                        {
                            fire = true;
                        }
                    }
                    
                }         
            }
            else if (touches.Count == 2)
            {
                if (settings.ControlType == SettingsManager.ControlTypeValues.GameController)
                {
                    if ((gameControllerLeftScreen.Contains(new Point((int)touches[0].Position.X, (int)touches[0].Position.Y)) &&
                            gameControllerFireScreen.Contains(new Point((int)touches[1].Position.X, (int)touches[1].Position.Y))) ||
                        (gameControllerLeftScreen.Contains(new Point((int)touches[1].Position.X, (int)touches[1].Position.Y)) &&
                            gameControllerFireScreen.Contains(new Point((int)touches[0].Position.X, (int)touches[0].Position.Y))))
                    {
                        fire = true;
                        left = true;
                    }
                    else if ((gameControllerRightScreen.Contains(new Point((int)touches[0].Position.X, (int)touches[0].Position.Y)) &&
                                gameControllerFireScreen.Contains(new Point((int)touches[1].Position.X, (int)touches[1].Position.Y))) ||
                            (gameControllerRightScreen.Contains(new Point((int)touches[1].Position.X, (int)touches[1].Position.Y)) &&
                                gameControllerFireScreen.Contains(new Point((int)touches[0].Position.X, (int)touches[0].Position.Y))))
                    {
                        fire = true;
                        right = true;
                    }
                }
                else if (settings.ControlType == SettingsManager.ControlTypeValues.Touch)
                {
                    if ((leftSideScreen.Contains(new Point((int)touches[0].Position.X, (int)touches[0].Position.Y)) &&
                            upperScreen.Contains(new Point((int)touches[1].Position.X, (int)touches[1].Position.Y))) ||
                        (leftSideScreen.Contains(new Point((int)touches[1].Position.X, (int)touches[1].Position.Y)) &&
                            upperScreen.Contains(new Point((int)touches[0].Position.X, (int)touches[0].Position.Y))))
                    {
                        fire = true;
                        left = true;
                    }
                    else if ((rightSideScreen.Contains(new Point((int)touches[0].Position.X, (int)touches[0].Position.Y)) &&
                                upperScreen.Contains(new Point((int)touches[1].Position.X, (int)touches[1].Position.Y))) ||
                            (rightSideScreen.Contains(new Point((int)touches[1].Position.X, (int)touches[1].Position.Y)) &&
                                upperScreen.Contains(new Point((int)touches[0].Position.X, (int)touches[0].Position.Y))))
                    {
                        fire = true;
                        right = true;
                    }
                }
                else if (settings.ControlType == SettingsManager.ControlTypeValues.Tilt)
                {
                    fire = true;
                }
            }

            if (fire && canFire && playerSprite.Location.Y == LOCATION_Y)
            {
                fireShot();
            }


            if (left && !right)
            {
                playerSprite.Velocity = new Vector2(-PLAYER_SPEED / 3 * elapsed,
                                        0);
            }
            else if (!left && right)
            {
                playerSprite.Velocity = new Vector2(PLAYER_SPEED / 3 * elapsed,
                                        0);
            }
            else
            {
                playerSprite.Velocity = Vector2.Zero;
            }

            if (settings.ControlType == SettingsManager.ControlTypeValues.Tilt)
            {
                Vector3 current = currentAccValue;
                
                current.Y = MathHelper.Clamp(current.Y, -0.4f, 0.4f);
                current.X = MathHelper.Clamp(current.X, -0.4f, 0.4f);


                playerSprite.Velocity = new Vector2(-current.Y * 4,
                                                    0);

                if (playerSprite.Velocity.Length() < 0.1f)
                {
                    playerSprite.Velocity = Vector2.Zero;
                }
            }
        }

        private void HandleKeyboardInput(KeyboardState state)
        {
            #if DEBUG

            /*if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D1))
            {
                fireShot();
            }
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D2))
            {
                fireDoubleShot();
            }
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D3))
            {
                fireTripleShot();
            }
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Q))
            {
                fireSideLeftShot();
            }
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.W))
            {
                fireSideRightShot();
            }
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D4))
            {
                fireSpecialShot();
            }
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D5))
            {
                fireCarliRocket();
            }*/

            Vector2 velo = Vector2.Zero;

            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
            {
                velo += new Vector2(-1.0f, 0.0f);
            }
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                velo += new Vector2(1.0f, 0.0f);
            }
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                velo += new Vector2(0.0f, -1.0f);
            }
            if (state.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Down))
            {
                velo += new Vector2(0.0f, 1.0f);
            }

            if (velo != Vector2.Zero)
                velo.Normalize();

            playerSprite.Velocity = velo;

            #endif
        }

        private void adaptMovementLimits()
        {
            Vector2 location = playerSprite.Location;

            if (location.X < playerAreaLimit.X + MOVE_PADDING_X)
            {
                location.X = playerAreaLimit.X + MOVE_PADDING_X;
            }

            if (location.X > (playerAreaLimit.Right - (playerSprite.Source.Width + MOVE_PADDING_X)))
            {
                location.X = (playerAreaLimit.Right - (playerSprite.Source.Width + MOVE_PADDING_X));
            }

            if (location.Y < playerAreaLimit.Y)
            {
                location.Y = playerAreaLimit.Y;
            }

            playerSprite.Location = location;
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            PlayerShotManager.Update(gameTime);

            if (!IsDestroyed)
            {
                shotTimer -= elapsed;

                HandleTouchInput(TouchPanel.GetState(), elapsed);
                HandleKeyboardInput(Keyboard.GetState());

                if (playerSprite.Velocity.Length() != 0.0f)
                {
                    playerSprite.Velocity.Normalize();
                }

                playerSprite.Velocity *= PLAYER_SPEED;

                playerSprite.Update(gameTime);
                adaptMovementLimits();

                if (playerSprite.Location.Y > LOCATION_Y)
                {
                    playerSprite.Location = new Vector2(playerSprite.Location.X, playerSprite.Location.Y - 1);
                }

                float factor = (float)Math.Max((this.hitPoints / 100.0f), 0.75f);
                this.playerSprite.TintColor = new Color(factor, factor, factor);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            PlayerShotManager.Draw(spriteBatch);

            if (!IsDestroyed)
            {
                playerSprite.TintColor = Color.Lime;
                playerSprite.Draw(spriteBatch);
            }
        }

        public void IncreasePlayerScore(long score)
        {
            this.playerScore += score;
        }

        public void SetHitPoints(float hp)
        {
            this.hitPoints = MathHelper.Clamp(hp, 0.0f, MaxHitPoints);
        }

        public void IncreaseHitPoints(float hp)
        {
            if (hp < 0)
                throw new ArgumentException("Negative values are not allowed.");

            this.hitPoints += hp;
            this.hitPoints = MathHelper.Clamp(hitPoints, 0.0f, MaxHitPoints);
        }

        public void Kill()
        {
            this.hitPoints = 0.0f;
        }

        public void ResetPlayerScore()
        {
            this.playerScore = 0;
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            //Player sprite
            playerSprite.Activated(reader);

            this.playerScore = Int64.Parse(reader.ReadLine());

            this.livesRemaining = Int32.Parse(reader.ReadLine());

            this.shotPower = Single.Parse(reader.ReadLine());
            this.hitPoints = Single.Parse(reader.ReadLine());

            this.shotTimer = Single.Parse(reader.ReadLine());

            PlayerShotManager.Activated(reader);

            //Shield sprite
            this.shieldSprite.Location = new Vector2(Single.Parse(reader.ReadLine()),
                                                     Single.Parse(reader.ReadLine()));
            this.shieldSprite.Rotation = Single.Parse(reader.ReadLine());
            this.shieldSprite.Velocity = new Vector2(Single.Parse(reader.ReadLine()),
                                                     Single.Parse(reader.ReadLine()));

            this.canFire = (bool)Boolean.Parse(reader.ReadLine());
        }

        public void Deactivated(StreamWriter writer)
        {
            // Player sprite
            playerSprite.Deactivated(writer);

            writer.WriteLine(this.playerScore);

            writer.WriteLine(this.livesRemaining);

            writer.WriteLine(this.shotPower);
            writer.WriteLine(this.hitPoints);

            writer.WriteLine(this.shotTimer);

            PlayerShotManager.Deactivated(writer);

            // Shield sprite
            writer.WriteLine(shieldSprite.Location.X);
            writer.WriteLine(shieldSprite.Location.Y);
            writer.WriteLine(shieldSprite.Rotation);
            writer.WriteLine(shieldSprite.Velocity.X);
            writer.WriteLine(shieldSprite.Velocity.Y);

            writer.WriteLine(canFire);
        }

        #endregion

        #region Properties

        public int LivesRemaining
        {
            get
            {
                return this.livesRemaining;
            }
            set
            {
                this.livesRemaining = (int)MathHelper.Clamp(value, -1, MAX_PLAYER_LIVES);
            }
        }

        public float HitPoints
        {
            get
            {
                return this.hitPoints;
            }
        }

        public bool IsDestroyed
        {
            get
            {
                return this.hitPoints <= 0.0f;
            }
        }

        public float ShotPower
        {
            get
            {
                return this.shotPower;
            }
        }

        public long PlayerScore
        {
            get
            {
                return this.playerScore;
            }
        }

        public bool CanFire
        {
            get
            {
                return canFire;
            }
            set
            {
                canFire = value;
            }
        }

        #endregion

        #region Events

        private void OnAccelerometerHelperReadingChanged(object sender, AccelerometerHelperReadingEventArgs e)
        {
            currentAccValue = new Vector3((float)e.OptimalyFilteredAcceleration.X,
                                          (float)e.OptimalyFilteredAcceleration.Y,
                                          (float)e.OptimalyFilteredAcceleration.Z);
        }

        #endregion
    }
}
