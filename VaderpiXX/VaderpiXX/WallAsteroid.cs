using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace VaderpiXX
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    class WallAsteroid : Sprite
    {
        #region Members

        private int remainingSustainingHits;
        private readonly int initialRemainingSustainingHits;

        public const int HITS_TO_DESTROY_ELEMENT = 1;

        private float opacity = 1.0f;

        #endregion

        #region Constructors

        public WallAsteroid(Vector2 location, Texture2D texture,
                            Rectangle initialFrame, int remainingHits)
            : base(location, texture, initialFrame, Vector2.Zero)
        {
            this.remainingSustainingHits = 0;
            this.initialRemainingSustainingHits = remainingHits;
        }

        #endregion

        #region Methods

        public void DecreaseRemainingSustainingHits()
        {
            this.remainingSustainingHits--;
        }

        public void Destroy()
        {
            this.remainingSustainingHits = 0;
        }

        public void Reset()
        {
            this.remainingSustainingHits = HITS_TO_DESTROY_ELEMENT;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsDestroyed)
            {
                float renderOpacity = Math.Max(0.0f, opacity);
                this.TintColor = Color.Lime * (0.5f + 0.5f * remainingSustainingHits / initialRemainingSustainingHits) * opacity;
                base.Draw(spriteBatch);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (opacity < 1.0f)
                opacity += 0.02f;
            else if (opacity > 1.0f)
                opacity = 1.0f;

            base.Update(gameTime);
        }

        #endregion

        #region Activated/Deactivated

        public void WallActivated(StreamReader reader)
        {
            remainingSustainingHits = Int32.Parse(reader.ReadLine());
            opacity = (float)Single.Parse(reader.ReadLine());
        }

        public void WallDeactivated(StreamWriter writer)
        {
            writer.WriteLine(remainingSustainingHits);
            writer.WriteLine(opacity);
        }

        #endregion

        #region Properties

        public bool IsDestroyed
        {
            get
            {
                return this.remainingSustainingHits < 1;
            }
        }

        public int RemainingSustainingHits
        {
            get 
            { 
                return remainingSustainingHits; 
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
