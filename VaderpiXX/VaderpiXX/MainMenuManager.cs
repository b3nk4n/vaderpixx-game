using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO;
using VaderpiXX.Inputs;
using Microsoft.Phone.Tasks;

namespace VaderpiXX
{
    class MainMenuManager
    {
        #region Members

        public enum MenuItems { None, Start, Highscores, Instructions, Help, Settings };

        private MenuItems lastPressedMenuItem = MenuItems.None;

        private Texture2D texture;

        private Rectangle spacepixxSource = new Rectangle(0, 0,
                                                          500, 100);
        private Rectangle spacepixxDestination = new Rectangle(150, 80,
                                                               500, 100);

        private Rectangle startSource = new Rectangle(0, 200,
                                                      300, 50);
        private Rectangle startDestination = new Rectangle(250, 200,
                                                           300, 50);

        private Rectangle instructionsSource = new Rectangle(0, 300,
                                                             300, 50);
        private Rectangle instructionsDestination = new Rectangle(250, 270,
                                                                  300, 50);
        
        private Rectangle highscoresSource = new Rectangle(0, 250,
                                                           300, 50);
        private Rectangle highscoresDestination = new Rectangle(250, 340,
                                                                300, 50);

        private Rectangle settingsSource = new Rectangle(0, 400,
                                                     300, 50);
        private Rectangle settingsDestination = new Rectangle(250, 410,
                                                          300, 50);

        private Rectangle moreGamesSource = new Rectangle(400, 900,
                                                       100, 100);
        private Rectangle moreGamesDestination = new Rectangle(10, 380,
                                                            100, 100);

        private Rectangle helpSource = new Rectangle(300, 800,
                                                     100, 100);
        private Rectangle helpDestination = new Rectangle(690, 380,
                                                          100, 100);

        private float opacity = 0.0f;
        private const float OpacityMax = 1.0f;
        private const float OpacityMin = 0.0f;
        private const float OpacityChangeRate = 0.05f;

        private bool isActive = false;

        private float time = 0.0f;

        private GameInput gameInput;
        private const string StartAction = "Start";
        private const string InstructionsAction = "Instructions";
        private const string HighscoresAction = "Highscores";
        private const string SettingsAction = "Settings";
        private const string HelpAction = "Help";
        private const string ReviewAction = "Review";

        private MarketplaceSearchTask searchTask = new MarketplaceSearchTask();

        private const string SEARCH_TERM = "Benjamin Sautermeister";

        #endregion

        #region Constructors

        public MainMenuManager(Texture2D spriteSheet, GameInput input)
        {
            this.texture = spriteSheet;
            this.gameInput = input;
        }

        #endregion

        #region Methods

        public void SetupInputs()
        {
            gameInput.AddTouchGestureInput(StartAction,
                                           GestureType.Tap, 
                                           startDestination);
            gameInput.AddTouchGestureInput(InstructionsAction,
                                           GestureType.Tap,
                                           instructionsDestination);
            gameInput.AddTouchGestureInput(HighscoresAction,
                                           GestureType.Tap,
                                           highscoresDestination);
            gameInput.AddTouchGestureInput(SettingsAction,
                                           GestureType.Tap,
                                           settingsDestination);
            gameInput.AddTouchGestureInput(HelpAction,
                                           GestureType.Tap,
                                           helpDestination);

            gameInput.AddTouchGestureInput(ReviewAction,
                                           GestureType.Tap,
                                           moreGamesDestination);
        }

        public void Update(GameTime gameTime)
        {
            if (isActive)
            {
                if (this.opacity < OpacityMax)
                    this.opacity += OpacityChangeRate;
            }

            time = (float)gameTime.TotalGameTime.TotalSeconds;

            this.handleTouchInputs();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture,
                             spacepixxDestination,
                             spacepixxSource,
                             Color.Lime * opacity);

            spriteBatch.Draw(texture,
                             startDestination,
                             startSource,
                             Color.Lime * opacity);

            spriteBatch.Draw(texture,
                             highscoresDestination,
                             highscoresSource,
                             Color.Lime * opacity);

            if (InstructionManager.HasDoneInstructions)
            {
                spriteBatch.Draw(texture,
                                 instructionsDestination,
                                 instructionsSource,
                                 Color.Lime * opacity);
            }
            else
            {
                spriteBatch.Draw(texture,
                                 instructionsDestination,
                                 instructionsSource,
                                 Color.Lime * opacity * (0.25f + (float)(Math.Pow(Math.Sin(time), 2.0f)) * 0.75f));
            }

            spriteBatch.Draw(texture,
                             helpDestination,
                             helpSource,
                             Color.Lime * opacity);

            spriteBatch.Draw(texture,
                             settingsDestination,
                             settingsSource,
                             Color.Lime * opacity);

            spriteBatch.Draw(texture,
                             moreGamesDestination,
                             moreGamesSource,
                             Color.Lime * opacity);
        }

        private void handleTouchInputs()
        {
            // Start
            if (gameInput.IsPressed(StartAction))
            {
                this.lastPressedMenuItem = MenuItems.Start;
            }
            // Highscores
            else if (gameInput.IsPressed(HighscoresAction))
            {
                this.lastPressedMenuItem = MenuItems.Highscores;
            }
            // Instructions
            else if (gameInput.IsPressed(InstructionsAction))
            {
                this.lastPressedMenuItem = MenuItems.Instructions;
            }
            // Help
            else if (gameInput.IsPressed(HelpAction))
            {
                this.lastPressedMenuItem = MenuItems.Help;
            }
            // Settings
            else if (gameInput.IsPressed(SettingsAction))
            {
                this.lastPressedMenuItem = MenuItems.Settings;
            }
            // Settings
            else if (gameInput.IsPressed(ReviewAction))
            {
                searchTask.SearchTerms = SEARCH_TERM;
                searchTask.Show();
            }
            else
            {
                this.lastPressedMenuItem = MenuItems.None;
            }
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            this.lastPressedMenuItem = (MenuItems)Enum.Parse(lastPressedMenuItem.GetType(), reader.ReadLine(), false);
            this.opacity = Single.Parse(reader.ReadLine());
            this.isActive = Boolean.Parse(reader.ReadLine());
            this.time = Single.Parse(reader.ReadLine());
        }

        public void Deactivated(StreamWriter writer)
        {
            writer.WriteLine(lastPressedMenuItem);
            writer.WriteLine(opacity);
            writer.WriteLine(isActive);
            writer.WriteLine(time);
        }

        #endregion

        #region Properties

        public MenuItems LastPressedMenuItem
        {
            get
            {
                return this.lastPressedMenuItem;
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
                }
            }
        }

        #endregion
    }
}
