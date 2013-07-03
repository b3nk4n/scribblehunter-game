using System;
using Microsoft.Xna.Framework;
using System.IO;
using System.Collections.Generic;

namespace ScribbleHunter
{
    class ZoomText
    {
        #region Membsers

        public string text;
        public Color drawColor;
        public int displayCounter;
        private int maxDisplayCount;
        private float lastScaleAmount = 0.0f;
        private float scaleRate;
        private Vector2 location;

        #endregion

        #region Constructors

        public ZoomText()
        {
            this.displayCounter = 0;
        }

        public ZoomText(string text, Color fontColor,
                        int maxDisplayCount, float scaleRate,
                        Vector2 location)
        {
            this.text = text;
            this.drawColor = fontColor;
            this.displayCounter = 0;
            this.maxDisplayCount = maxDisplayCount;
            this.scaleRate = scaleRate;
            this.location = location;
        }

        #endregion

        #region Methods

        public void Update()
        {
            lastScaleAmount += scaleRate;
            displayCounter++;
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(Queue<string> data)
        {
            this.text = data.Dequeue();

            this.drawColor = new Color(Int32.Parse(data.Dequeue()),
                                       Int32.Parse(data.Dequeue()),
                                       Int32.Parse(data.Dequeue()),
                                       Int32.Parse(data.Dequeue()));

            this.displayCounter = Int32.Parse(data.Dequeue());
            this.maxDisplayCount = Int32.Parse(data.Dequeue());

            this.lastScaleAmount = Single.Parse(data.Dequeue());
            this.scaleRate = Single.Parse(data.Dequeue());

            this.location.X = Single.Parse(data.Dequeue());
            this.location.Y = Single.Parse(data.Dequeue());
        }

        public void Deactivated(Queue<string> data)
        {
            data.Enqueue(text);

            data.Enqueue(((int)drawColor.R).ToString());
            data.Enqueue(((int)drawColor.G).ToString());
            data.Enqueue(((int)drawColor.B).ToString());
            data.Enqueue(((int)drawColor.A).ToString());

            data.Enqueue(displayCounter.ToString());
            data.Enqueue(maxDisplayCount.ToString());

            data.Enqueue(lastScaleAmount.ToString());
            data.Enqueue(scaleRate.ToString());

            data.Enqueue(location.X.ToString());
            data.Enqueue(location.Y.ToString());
        }

        #endregion

        #region Properties

        public float Scale
        {
            get
            {
                return scaleRate * displayCounter;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return displayCounter > maxDisplayCount;
            }
        }

        public float Progress
        {
            get
            {
                return (float)displayCounter / maxDisplayCount;
            }
        }

        public Vector2 Location
        {
            get
            {
                return this.location;
            }
        }

        #endregion
    }
}
