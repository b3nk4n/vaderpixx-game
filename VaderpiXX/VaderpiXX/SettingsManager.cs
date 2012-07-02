using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO.IsolatedStorage;
using System.IO;
using VaderpiXX.Inputs;

namespace VaderpiXX
{
    class SettingsManager
    {
        #region Members

        private static SettingsManager settingsManager;

        private static Texture2D texture;
        private static SpriteFont font;
        private readonly Rectangle SettingsTitleSource = new Rectangle(0, 400,
                                                                       300, 50);
        private readonly Vector2 TitlePosition = new Vector2(250.0f, 100.0f);

        public enum SoundValues {Off, VeryLow, Low, Med, High, VeryHigh};
        public enum VibrationValues { On, Off };
        public enum ControlTypeValues { Touch, Tilt, GameController };

        private const string MUSIC_TITLE = "MUSIC: ";
        private SoundValues musicValue = SoundValues.Med;
        private readonly int musicPositionY = 180;
        private readonly Rectangle musicDestination = new Rectangle(250, 175,
                                                                    300, 50);

        private const string SFX_TITLE = "SFX: ";
        private SoundValues sfxValue = SoundValues.Med;
        private readonly int sfxPositionY = 250;
        private readonly Rectangle sfxDestination = new Rectangle(250, 245,
                                                                  300, 50);

        private const string VIBRATION_TITLE = "VIBRATION: ";
        private VibrationValues vibrationValue = VibrationValues.On;
        private readonly int vibrationPositionY = 320;
        private readonly Rectangle vibrationDestination = new Rectangle(250, 315,
                                                                        300, 50);

        private const string CONTROL_TYPE_TITLE = "CONTROL TYPE: ";
        private ControlTypeValues controlTypeValue = ControlTypeValues.GameController;
        private readonly int controlTypeY = 390;
        private readonly Rectangle controlTypeDestination = new Rectangle(250, 385,
                                                                              300, 50);

        private static Rectangle screenBounds;

        private float opacity = 0.0f;
        private const float OpacityMax = 1.0f;
        private const float OpacityMin = 0.0f;
        private const float OpacityChangeRate = 0.05f;

        private bool isActive = false;

        public static GameInput GameInput;
        private const string MusicAction = "Music";
        private const string SfxAction = "SFX";
        private const string VibrationAction = "Vibration";
        private const string ControlPositionAction = "ControlPos";

        private const string ON = "ON";
        private const string OFF = "OFF";
        private const string VERY_LOW = "VERY LOW";
        private const string LOW = "LOW";
        private const string MEDIUM = "MEDIUM";
        private const string HIGH = "HIGH";
        private const string VERY_HIGH = "VERY HIGH";
        
        private const string TOUCH = "TOUCH 2";
        private const string TILT = "TILT";
        private const string GAMECONTROLLER = "TOUCH 1";

        private readonly Rectangle touch1Source = new Rectangle(400, 1000,
                                                                100, 100);
        private readonly Rectangle touch2Source = new Rectangle(300, 1000,
                                                                100, 100);
        private readonly Rectangle tiltSource = new Rectangle(300, 900,
                                                                100, 100);
        private readonly Vector2 controllPosition = new Vector2(630, 354);

        private readonly Rectangle arrowSource = new Rectangle(400, 550,
                                                                20, 20);
        private readonly Vector2 arrowPosition = new Vector2(585, 397);

        #endregion

        #region Constructors

        private SettingsManager()
        {
            this.Load();
        }

        #endregion

        #region Methods

        public void SetupInputs()
        {
            GameInput.AddTouchGestureInput(MusicAction,
                                           GestureType.Tap,
                                           musicDestination);
            GameInput.AddTouchGestureInput(SfxAction,
                                           GestureType.Tap,
                                           sfxDestination);
            GameInput.AddTouchGestureInput(VibrationAction,
                                           GestureType.Tap,
                                           vibrationDestination);
            GameInput.AddTouchGestureInput(ControlPositionAction,
                                           GestureType.Tap,
                                           controlTypeDestination);
        }

        public void Initialize(Texture2D tex, SpriteFont f, Rectangle screen)
        {
            texture = tex;
            font = f;
            screenBounds = screen;
        }

        public static SettingsManager GetInstance()
        {
            if (settingsManager == null)
            {
                settingsManager = new SettingsManager();
            }

            return settingsManager;
        }

        public void Update(GameTime gameTime)
        {
            if (isActive)
            {
                if (this.opacity < OpacityMax)
                    this.opacity += OpacityChangeRate;
            }

            handleTouchInputs();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture,
                             TitlePosition,
                             SettingsTitleSource,
                             Color.White * opacity);

            drawMusic(spriteBatch);
            drawSfx(spriteBatch);
            drawVibration(spriteBatch);
            drawControlType(spriteBatch);
        }

        private void handleTouchInputs()
        {
            // Music
            if (GameInput.IsPressed(MusicAction))
            {
                toggleMusic();
            }
            // Sfx
            if (GameInput.IsPressed(SfxAction))
            {
                toggleSfx();
            }
            // Vibration
            if (GameInput.IsPressed(VibrationAction))
            {
                toggleVibration();
            }
            // ControlPosition
            if (GameInput.IsPressed(ControlPositionAction))
            {
                ToggleControlType();
            }
        }

        private void toggleMusic()
        {
            switch (musicValue)
            {
                case SoundValues.Off:
                    musicValue = SoundValues.VeryLow;
                    break;
                case SoundValues.VeryLow:
                    musicValue = SoundValues.Low;
                    break;
                case SoundValues.Low:
                    musicValue = SoundValues.Med;
                    break;
                case SoundValues.Med:
                    musicValue = SoundValues.High;
                    break;
                case SoundValues.High:
                    musicValue = SoundValues.VeryHigh;
                    break;
                case SoundValues.VeryHigh:
                    musicValue = SoundValues.Off;
                    break;
            }

            SoundManager.RefreshMusicVolume();
        }

        private void toggleSfx()
        {
            switch (sfxValue)
            {
                case SoundValues.Off:
                    sfxValue = SoundValues.VeryLow;
                    break;
                case SoundValues.VeryLow:
                    sfxValue = SoundValues.Low;
                    break;
                case SoundValues.Low:
                    sfxValue = SoundValues.Med;
                    break;
                case SoundValues.Med:
                    sfxValue = SoundValues.High;
                    break;
                case SoundValues.High:
                    sfxValue = SoundValues.VeryHigh;
                    break;
                case SoundValues.VeryHigh:
                    sfxValue = SoundValues.Off;
                    break;
            }

            if (sfxValue != SoundValues.Off)
                SoundManager.PlayPlayerShot();
        }

        private void toggleVibration()
        {
            switch (vibrationValue)
            {
                case VibrationValues.Off:
                    vibrationValue = VibrationValues.On;
                    break;
                case VibrationValues.On:
                    vibrationValue = VibrationValues.Off;
                    break;
            }

            if (vibrationValue == VibrationValues.On)
                VibrationManager.Vibrate(0.2f);
        }

        public void ToggleControlType()
        {
            switch (controlTypeValue)
            {
                case ControlTypeValues.GameController:
                    controlTypeValue = ControlTypeValues.Touch;
                    break;
                case ControlTypeValues.Touch:
                    controlTypeValue = ControlTypeValues.Tilt;
                    break;
                case ControlTypeValues.Tilt:
                    controlTypeValue = ControlTypeValues.GameController;
                    break;
                default:
                    break;
            }
        }

        private void drawMusic(SpriteBatch spriteBatch)
        {
            string text;

            switch (musicValue)
            {
                case SoundValues.VeryLow:
                    text = VERY_LOW;
                    break;
                case SoundValues.Low:
                    text = LOW;
                    break;
                case SoundValues.Med:
                    text = MEDIUM;
                    break;
                case SoundValues.High:
                    text = HIGH;
                    break;
                case SoundValues.VeryHigh:
                    text = VERY_HIGH;
                    break;
                default:
                    text = OFF;
                    break;
            }

            spriteBatch.DrawString(font,
                                   MUSIC_TITLE,
                                   new Vector2(250,
                                               musicPositionY),
                                   Color.Lime * opacity);

            spriteBatch.DrawString(font,
                                   text,
                                   new Vector2((550 - font.MeasureString(text).X),
                                               musicPositionY),
                                   Color.Lime * opacity);
        }

        private void drawSfx(SpriteBatch spriteBatch)
        {
            string text;

            switch (sfxValue)
            {
                case SoundValues.VeryLow:
                    text = VERY_LOW;
                    break;
                case SoundValues.Low:
                    text = LOW;
                    break;
                case SoundValues.Med:
                    text = MEDIUM;
                    break;
                case SoundValues.High:
                    text = HIGH;
                    break;
                case SoundValues.VeryHigh:
                    text = VERY_HIGH;
                    break;
                default:
                    text = OFF;
                    break;
            }

            spriteBatch.DrawString(font,
                                   SFX_TITLE,
                                   new Vector2(250,
                                               sfxPositionY),
                                   Color.Lime * opacity);

            spriteBatch.DrawString(font,
                                   text,
                                   new Vector2((550 - font.MeasureString(text).X),
                                               sfxPositionY),
                                   Color.Lime * opacity);
        }

        private void drawVibration(SpriteBatch spriteBatch)
        {
            string text;

            switch (vibrationValue)
            {
                case VibrationValues.On:
                    text = ON;
                    break;
                default:
                    text = OFF;
                    break;
            }

            spriteBatch.DrawString(font,
                                   VIBRATION_TITLE,
                                   new Vector2(250,
                                               vibrationPositionY),
                                   Color.Lime * opacity);

            spriteBatch.DrawString(font,
                                   text,
                                   new Vector2((550 - font.MeasureString(text).X),
                                               vibrationPositionY),
                                   Color.Lime * opacity);
        }

        private void drawControlType(SpriteBatch spriteBatch)
        {
            string text;
            Rectangle src;

            switch (controlTypeValue)
            {
                case ControlTypeValues.Touch:
                    text = TOUCH;
                    src = touch2Source;
                    break;
                case ControlTypeValues.GameController:
                    text = GAMECONTROLLER;
                    src = touch1Source;
                    break;
                default:
                    text = TILT;
                    src = tiltSource;
                    break;
            }

            spriteBatch.DrawString(font,
                                   CONTROL_TYPE_TITLE,
                                   new Vector2(250,
                                               controlTypeY),
                                   Color.Lime * opacity);

            spriteBatch.DrawString(font,
                                   text,
                                   new Vector2((550 - font.MeasureString(text).X),
                                               controlTypeY),
                                   Color.Lime * opacity);

            spriteBatch.Draw(texture,
                             controllPosition,
                             src,
                             Color.Lime * 0.66f);

            spriteBatch.Draw(texture,
                             arrowPosition,
                             arrowSource,
                             Color.Lime * 0.66f);
        }

        #endregion

        #region Load/Save

        public void Save()
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream("settings.txt", FileMode.Create, isf))
                {
                    using (StreamWriter sw = new StreamWriter(isfs))
                    {
                        sw.WriteLine(this.musicValue);
                        sw.WriteLine(this.sfxValue);
                        sw.WriteLine(this.vibrationValue);
                        sw.WriteLine(this.controlTypeValue);

                        sw.Flush();
                        sw.Close();
                    }
                }
            }
        }

        public void Load()
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                bool hasExisted = isf.FileExists(@"settings.txt");

                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(@"settings.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, isf))
                {
                    if (hasExisted)
                    {
                        using (StreamReader sr = new StreamReader(isfs))
                        {
                            this.musicValue = (SoundValues)Enum.Parse(musicValue.GetType(), sr.ReadLine(), true);
                            this.sfxValue = (SoundValues)Enum.Parse(sfxValue.GetType(), sr.ReadLine(), true);
                            this.vibrationValue = (VibrationValues)Enum.Parse(vibrationValue.GetType(), sr.ReadLine(), true);
                            this.controlTypeValue = (ControlTypeValues)Enum.Parse(controlTypeValue.GetType(), sr.ReadLine(), true);
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = new StreamWriter(isfs))
                        {
                            sw.WriteLine(this.musicValue);
                            sw.WriteLine(this.sfxValue);
                            sw.WriteLine(this.vibrationValue);
                            sw.WriteLine(this.controlTypeValue);

                            // ... ? 
                        }
                    }
                }
            }
        }

        public float GetMusicValue()
        {
            switch (settingsManager.musicValue)
            {
                case SoundValues.Off:
                    return 0.0f;

                case SoundValues.VeryLow:
                    return 0.1f;

                case SoundValues.Low:
                    return 0.2f;

                case SoundValues.Med:
                    return 0.3f;

                case SoundValues.High:
                    return 0.4f;

                case SoundValues.VeryHigh:
                    return 0.5f;

                default:
                    return 0.3f;
            }
        }

        public float GetSfxValue()
        {
            switch (settingsManager.sfxValue)
            {
                case SoundValues.Off:
                    return 0.0f;

                case SoundValues.VeryLow:
                    return 0.2f;

                case SoundValues.Low:
                    return 0.4f;

                case SoundValues.Med:
                    return 0.6f;

                case SoundValues.High:
                    return 0.8f;

                case SoundValues.VeryHigh:
                    return 1.0f;

                default:
                    return 0.6f;
            }
        }

        public bool GetVabrationValue()
        {
            switch (settingsManager.vibrationValue)
            {
                case VibrationValues.On:
                    return true;

                case VibrationValues.Off:
                    return false;

                default:
                    return true;
            }
        }

        #endregion

        #region Properties

        public ControlTypeValues ControlType
        {
            get
            {
                return controlTypeValue;
            }
            set
            {
                controlTypeValue = value;
            }
        }

        public bool IsActive
        {
            get
            {
                return this.isActive;
            }
            set
            {
                this.isActive = value;

                if (isActive == false)
                {
                    this.opacity = OpacityMin;
                    Save();
                }
            }
        }

        #endregion
    }
}
