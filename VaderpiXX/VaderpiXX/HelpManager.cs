using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using VaderpiXX.Inputs;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Phone.Tasks;

namespace VaderpiXX
{
    class HelpManager
    {
        #region Members

        private Texture2D texture;
        private SpriteFont font;
        private readonly Rectangle HelpTitleSource = new Rectangle(0, 350,
                                                                   300, 50);
        private readonly Vector2 TitlePosition = new Vector2(250.0f, 100.0f);

        private static readonly string[] Content = {"IF YOU HAVE ANY FURTHER QUESTIONS,",
                                                    "IDEAS OR PROBLEMS WITH VADERPIXX,",
                                                    "PLEASE DO NOT HESITATE TO CONTACT US."};

        private const string Email = "apps@bsautermeister.de";
        private const string EmailSubject = "VaderpiXX - Support";
        private const string Blog = "bsautermeister.de";
        private const string MusicSponsor = "MUSIC SPONSOR:";
        private const string QBig = "soundcloud.com/qbig";

        private readonly Rectangle screenBounds;

        private float opacity = 0.0f;
        private const float OpacityMax = 1.0f;
        private const float OpacityMin = 0.0f;
        private const float OpacityChangeRate = 0.05f;

        private bool isActive = false;

        private WebBrowserTask browser;
        private const string BLOG_URL = "http://bsautermeister.de";
        private const string QBIG_URL = "http://soundcloud.com/qbig";

        private readonly Rectangle EmailDestination = new Rectangle(250, 300,
                                                                    300, 40);
        private readonly Rectangle BlogDestination = new Rectangle(250, 340,
                                                                    300, 40);
        private readonly Rectangle QBigDestination = new Rectangle(250, 410,
                                                                    300, 60);

        public static GameInput GameInput;
        private const string EmailAction = "Email";
        private const string BlogAction = "Blog";
        private const string QBigAction = "QBig";

        #endregion

        #region Constructors

        public HelpManager(Texture2D tex, SpriteFont font, Rectangle screenBounds)
        {
            this.browser = new WebBrowserTask();

            this.texture = tex;
            this.font = font;
            this.screenBounds = screenBounds;
        }

        #endregion

        #region Methods

        public void SetupInputs()
        {
            GameInput.AddTouchGestureInput(EmailAction,
                                           GestureType.Tap,
                                           EmailDestination);
            GameInput.AddTouchGestureInput(BlogAction,
                                           GestureType.Tap,
                                           BlogDestination);
            GameInput.AddTouchGestureInput(QBigAction,
                                           GestureType.Tap,
                                           QBigDestination);
        }

        private void handleTouchInputs()
        {
            // Email
            if (GameInput.IsPressed(EmailAction))
            {
                EmailComposeTask emailTask = new EmailComposeTask();
                emailTask.To = Email;
                emailTask.Subject = EmailSubject;
                emailTask.Show();
            }
            // Blog
            else if (GameInput.IsPressed(BlogAction))
            {
                this.browser.Uri = new Uri(BLOG_URL);
                browser.Show();
            }
            // QBig
            else if (GameInput.IsPressed(QBigAction))
            {
                this.browser.Uri = new Uri(QBIG_URL);
                browser.Show();
            }
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
                             HelpTitleSource,
                             Color.White * opacity);

            for (int i = 0; i < Content.Length; ++i)
            {
                spriteBatch.DrawString(font,
                       Content[i],
                       new Vector2((screenBounds.Width - font.MeasureString(Content[i]).X) / 2,
                                   180 + (i * 35)),
                       Color.Lime * opacity);
            }

            spriteBatch.DrawString(font,
                       Email,
                       new Vector2((screenBounds.Width - font.MeasureString(Email).X) / 2,
                                   300),
                       Color.Lime * opacity);

            spriteBatch.DrawString(font,
                       Blog,
                       new Vector2((screenBounds.Width - font.MeasureString(Blog).X) / 2,
                                   350),
                       Color.Lime * opacity);

            spriteBatch.DrawString(font,
                       MusicSponsor,
                       new Vector2((screenBounds.Width - font.MeasureString(MusicSponsor).X) / 2,
                                   395),
                       Color.Lime * opacity);

            spriteBatch.DrawString(font,
                       QBig,
                       new Vector2((screenBounds.Width - font.MeasureString(QBig).X) / 2,
                                   430),
                       Color.Lime * opacity);
        }

        #endregion

        #region Properties

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
