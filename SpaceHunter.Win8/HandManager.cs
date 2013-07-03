using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace ScribbleHunter
{
    class HandManager
    {
        #region Members

        private const float STAGGER_X = 6.0f;
        private const float STAGGER_Y = 8.0f;

        private Texture2D texture;

        private readonly Rectangle leftHandSource = new Rectangle(0, 0, 150, 740);

        private readonly Vector2 leftHandFrom = new Vector2(-160, 20);

        private readonly Vector2 leftHandTo = new Vector2(-70, 0);

        private Vector2 leftHandPosition;

        private Vector2 leftHandStagger;

        private readonly Rectangle rightHandSource = new Rectangle(150, 0, 550, 800);

        private readonly Vector2 rightHandFrom = new Vector2(810, -40);

        private readonly Vector2 rightHandTo = new Vector2(670, -25);

        private Vector2 rightHandPosition;

        private Vector2 rightHandStagger;

        private bool handShown;

        private readonly Vector2 ScribblePosition = new Vector2(378, -120);
        private bool scribbled = true;

        #endregion

        #region Constructors

        public HandManager(Texture2D texture)
        {
            this.texture = texture;

            leftHandPosition = leftHandFrom;
            rightHandPosition = rightHandFrom;
        }

        #endregion

        #region Methods

        public void Reset()
        {
            leftHandPosition = leftHandFrom;
            rightHandPosition = rightHandFrom;
            scribbled = true;
        }

        public void ShowHands()
        {
            handShown = true;
            scribbled = true;
        }

        public void HideHands()
        {
            handShown = false;
            scribbled = true;
        }

        public void HideHandsAndScribble()
        {
            handShown = false;
            scribbled = false;
            SoundManager.PlayWritingSound();
        }

        public void Update(GameTime gameTime)
        {
            Vector2 velocityLeft;
            Vector2 velocityRight;

            if (handShown)
            {
                velocityLeft = (leftHandPosition - leftHandTo) * 0.04f;
                velocityRight = (rightHandPosition - rightHandTo) * 0.04f;
            }
            else
            {
                velocityLeft = -(leftHandTo - (leftHandPosition - leftHandFrom)) * 0.015f;
                velocityRight = -(rightHandTo - (rightHandPosition - rightHandFrom)) * 0.01f;

                if (!scribbled)
                {
                    velocityRight = (rightHandPosition - ScribblePosition) * 0.166f;

                    if ((rightHandPosition - ScribblePosition).Length() < 2)
                    {
                        scribbled = true;
                    }
                }

                if (scribbled)
                {
                    velocityRight = -(rightHandTo - (rightHandPosition - rightHandFrom)) * 0.01f;
                }
                    
            }

            

            leftHandPosition -= velocityLeft;
            rightHandPosition -= velocityRight;
            
            // Stagger
            leftHandStagger = new Vector2(
                (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds) * STAGGER_X,
                (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds) * STAGGER_Y);
            rightHandStagger = new Vector2(
                (float)Math.Cos(gameTime.TotalGameTime.TotalSeconds) * STAGGER_X,
                (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds / 2) * STAGGER_Y);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Left hand
            if (leftHandPosition.X > -155)
            {
                spriteBatch.Draw(
                    texture,
                    leftHandPosition + leftHandStagger,
                    leftHandSource,
                    Color.White);
            }

            // Right hand
            if (rightHandPosition.X < 805)
            {
                spriteBatch.Draw(
                    texture,
                    rightHandPosition + rightHandStagger,
                    rightHandSource,
                    Color.White);
            }
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(Queue<string> data)
        {
            // Left hand
            this.leftHandPosition = new Vector2(Single.Parse(data.Dequeue()),
                                               Single.Parse(data.Dequeue()));

            this.leftHandStagger = new Vector2(Single.Parse(data.Dequeue()),
                                               Single.Parse(data.Dequeue()));

            // Right hand
            this.rightHandPosition = new Vector2(Single.Parse(data.Dequeue()),
                                               Single.Parse(data.Dequeue()));

            this.rightHandStagger = new Vector2(Single.Parse(data.Dequeue()),
                                               Single.Parse(data.Dequeue()));

            this.scribbled = Boolean.Parse(data.Dequeue());
        }


        public void Deactivated(Queue<string> data)
        {
            // Left hand
            data.Enqueue(leftHandPosition.X.ToString());
            data.Enqueue(leftHandPosition.Y.ToString());

            data.Enqueue(leftHandStagger.X.ToString());
            data.Enqueue(leftHandStagger.Y.ToString());

            // Right hand
            data.Enqueue(rightHandPosition.X.ToString());
            data.Enqueue(rightHandPosition.Y.ToString());

            data.Enqueue(rightHandStagger.X.ToString());
            data.Enqueue(rightHandStagger.Y.ToString());

            data.Enqueue(scribbled.ToString());
        }

        #endregion
    }
}
