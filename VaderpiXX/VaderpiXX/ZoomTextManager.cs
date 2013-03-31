using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace VaderpiXX
{
    class ZoomTextManager
    {
        #region Members

        private Queue<ZoomText> zoomTexts = new Queue<ZoomText>(8);
        private Vector2 location;
        private SpriteFont font;

        private static Queue<ZoomText> infoTexts = new Queue<ZoomText>(8);

        #endregion

        #region Constructors

        public ZoomTextManager(Vector2 location, SpriteFont font)
        {
            this.location = location;
            this.font = font;
        }

        #endregion

        #region Methods

        public void ShowText(string text)
        {
            zoomTexts.Enqueue(new ZoomText(text,
                                           Color.Lime,
                                           100,
                                           0.05f,
                                           400));
        }

        public static void ShowInfo(string text, float centerY)
        {
            infoTexts.Enqueue(new ZoomText(text,
                                       Color.Red,
                                       140,
                                       0.025f,
                                       centerY));
        }

        public void Update()
        {
            if (zoomTexts.Count > 0)
            {
                zoomTexts.First().Update();

                if (zoomTexts.First().IsCompleted)
                {
                    zoomTexts.Dequeue();
                }
            }

            if (infoTexts.Count > 0)
            {
                infoTexts.First().Update();

                if (infoTexts.First().IsCompleted)
                {
                    infoTexts.Dequeue();
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var zoom in zoomTexts)
            {
                spriteBatch.DrawString(font,
                                       zoom.text,
                                       location,
                                       zoom.drawColor * (float)(1.0f - Math.Pow(zoom.Progress, 4.0f)),
                                       0.0f,
                                       new Vector2(font.MeasureString(zoom.text).X / 2,
                                                   font.MeasureString(zoom.text).Y / 2),
                                       zoom.Scale,
                                       SpriteEffects.None,
                                       0.0f);
            }

            foreach (var info in infoTexts)
            {
                spriteBatch.DrawString(font,
                                       info.text,
                                       new Vector2(info.CenterX, 40),
                                       info.drawColor * (float)(1.0f - Math.Pow(info.Progress, 4.0f)),
                                       0.0f,
                                       new Vector2(font.MeasureString(info.text).X / 2,
                                                   font.MeasureString(info.text).Y / 2),
                                       info.Scale,
                                       SpriteEffects.None,
                                       0.0f);
            }
        }

        public void Reset()
        {
            this.zoomTexts.Clear();
            infoTexts.Clear();
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(StreamReader reader)
        {
            // Texts
            int textsCount = Int32.Parse(reader.ReadLine());
            zoomTexts.Clear();
            for (int i = 0; i < textsCount; ++i)
            {
                ZoomText text = new ZoomText();
                text.Activated(reader);
                zoomTexts.Enqueue(text);
            }

            // Infos
            int infosCount = Int32.Parse(reader.ReadLine());
            infoTexts.Clear();
            for (int i = 0; i < infosCount; ++i)
            {
                ZoomText info = new ZoomText();
                info.Activated(reader);
                infoTexts.Enqueue(info);
            }
        }

        public void Deactivated(StreamWriter writer)
        {
            // Texts
            int textsCount = zoomTexts.Count;
            writer.WriteLine(textsCount);

            for (int i = 0; i < textsCount; ++i)
            {
                ZoomText text = zoomTexts.Dequeue();
                text.Deactivated(writer);

            }

            // Infos
            int infosCount = infoTexts.Count;
            writer.WriteLine(infosCount);

            for (int i = 0; i < infosCount; ++i)
            {
                ZoomText info = infoTexts.Dequeue();
                info.Deactivated(writer);

            }
        }

        #endregion
    }
}
