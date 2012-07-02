using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using VaderpiXX.Extensions;

namespace VaderpiXX
{
    class Hud
    {
        #region Members

        private static Hud hud;

        private long score;
        private int remainingLives;
        private int level;

        private Rectangle screenBounds;
        private Texture2D texture;
        private SpriteFont font;

        private Vector2 scoreTextLocation = new Vector2(5, -5);
        private Vector2 scoreLocation = new Vector2(80, -5);
        private Vector2 livesTextLocation = new Vector2(650, -5);
        private Vector2 livesLocation = new Vector2(710, -1);
        private Vector2 levelTextLocation = new Vector2(360, -5);
        private Vector2 levelLocation = new Vector2(425, -5);

        Vector2 barOverlayStart = new Vector2(0, 350);

        private const string LEVEL_PRE_TEXT = "LEVEL";
        private const string SCORE_PRE_TEXT = "SCORE";
        private const string LIVES_PRE_TEXT = "LIVES";

        #endregion

        #region Constructors

        private Hud(Rectangle screen, Texture2D texture, SpriteFont font,
                    long score, int lives, int level)
        {
            this.screenBounds = screen;
            this.texture = texture;
            this.font = font;
            this.score = score;
            this.remainingLives = lives;
            this.level = level;
        }

        #endregion

        #region Methods

        public static Hud GetInstance(Rectangle screen, Texture2D texture, SpriteFont font, long score,
                                      int lives, float hitPoints, int level)
        {
            if (hud == null)
            {
                hud = new Hud(screen,
                              texture,
                              font,
                              score,
                              lives,
                              level);
            }

            return hud;
        }

        public void Update(long score, int lives, int level)
        {
            this.score = score;
            this.remainingLives = lives;
            this.level = level;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            drawScore(spriteBatch);

            if (remainingLives >= 0)
            {
                drawLevel(spriteBatch);
                drawRemainingLives(spriteBatch);
            }       
        }

        private void drawScore(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font,
                                   SCORE_PRE_TEXT,
                                   scoreTextLocation,
                                   Color.White * 0.8f);

            spriteBatch.DrawInt64WithZeros(font,
                                  score,
                                  scoreLocation,
                                  Color.Lime * 0.8f,
                                  6);
        }

        private void drawRemainingLives(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font,
                                   LIVES_PRE_TEXT,
                                   livesTextLocation,
                                   Color.White * 0.8f);

            for (int x = 0; x < PlayerManager.MAX_PLAYER_LIVES; x++)
            {
                Color c;

                if (x <= remainingLives)
                    c = Color.Lime * 0.8f;
                else
                    c = Color.Lime * 0.2f;

                spriteBatch.Draw(texture,
                                    new Rectangle((int)livesLocation.X + (x * 30),
                                                (int)livesLocation.Y,
                                                25,
                                                25),
                                    new Rectangle(0, 400, 25, 25),
                                    c);              
            }
        }

        private void drawLevel(SpriteBatch spriteBatch)
        {
            if (level > 0)
            {
                spriteBatch.DrawString(font,
                                       LEVEL_PRE_TEXT,
                                       levelTextLocation,
                                       Color.White * 0.8f);

                spriteBatch.DrawInt64WithZeros(font,
                                      level,
                                      levelLocation,
                                      Color.Lime * 0.8f,
                                      2);
            }
        }

        #endregion
    }
}
