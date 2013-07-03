using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace ScribbleHunter
{
    class ZoomTextManager
    {
        #region Members

        private Queue<ZoomText> zoomTexts = new Queue<ZoomText>(8);
        private Vector2 location;
        private SpriteFont font;
        private SpriteFont bigFont;

        private static Queue<ZoomText> infoTexts = new Queue<ZoomText>(8);
        private static Vector2 InfoLocation = new Vector2(400, 200);
        #endregion

        #region Constructors

        public ZoomTextManager(Vector2 location, SpriteFont font, SpriteFont bigFont)
        {
            this.location = location;
            this.font = font;
            this.bigFont = bigFont;
        }

        #endregion

        #region Methods

        public void ShowText(string text)
        {
            zoomTexts.Enqueue(new ZoomText(text,
                                           Color.Black,
                                           110,
                                           0.0166f,
                                           location));
        }

        public static void ShowInfo(string text)
        {
            infoTexts.Enqueue(new ZoomText(text,
                                       Color.Black,
                                       100,
                                       0.0175f,
                                       InfoLocation));
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
                spriteBatch.DrawString(bigFont,
                                       zoom.text,
                                       zoom.Location,
                                       zoom.drawColor * (float)(1.0f - Math.Pow(zoom.Progress, 4.0f)),
                                       0.0f,
                                       new Vector2(bigFont.MeasureString(zoom.text).X / 2,
                                                   bigFont.MeasureString(zoom.text).Y / 2),
                                       zoom.Scale,
                                       SpriteEffects.None,
                                       0.0f);
            }

            foreach (var info in infoTexts)
            {
                spriteBatch.DrawString(bigFont,
                                       info.text,
                                       info.Location,
                                       info.drawColor * (float)(1.0f - Math.Pow(info.Progress, 4.0f)),
                                       0.0f,
                                       new Vector2(bigFont.MeasureString(info.text).X / 2,
                                                   bigFont.MeasureString(info.text).Y / 2),
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

        public void Activated(Queue<string> data)
        {
            // Texts
            int textsCount = Int32.Parse(data.Dequeue());
            zoomTexts.Clear();

            for (int i = 0; i < textsCount; ++i)
            {
                ZoomText text = new ZoomText();
                text.Activated(data);
                zoomTexts.Enqueue(text);
            }

            // Infos
            int infosCount = Int32.Parse(data.Dequeue());
            infoTexts.Clear();

            for (int i = 0; i < infosCount; ++i)
            {
                ZoomText info = new ZoomText();
                info.Activated(data);
                infoTexts.Enqueue(info);
            }
        }

        public void Deactivated(Queue<string> data)
        {
            // Texts
            int textsCount = zoomTexts.Count;
            data.Enqueue(textsCount.ToString());

            for (int i = 0; i < textsCount; ++i)
            {
                ZoomText text = zoomTexts.Dequeue();
                text.Deactivated(data);
            }

            // Infos
            int infosCount = infoTexts.Count;
            data.Enqueue(infosCount.ToString());

            for (int i = 0; i < infosCount; ++i)
            {
                ZoomText info = infoTexts.Dequeue();
                info.Deactivated(data);
            }
        }

        #endregion
    }
}
