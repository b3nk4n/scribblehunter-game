using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;

namespace ScribbleHunter
{
    class Particle : Sprite
    {
        #region Members

        private Vector2 acceleration;
        private float maxSpeed;
        private float initialDuration;
        private float remainingDuration;
        private Color initialColor;
        private Color finalColor;
        private float rotationSpeed;

        #endregion

        #region Constructors

        public Particle(Vector2 location, Texture2D texture, Rectangle initialFrame,
                        Vector2 velocity, Vector2 acceleration, float maxSpeed,
                        float duration, Color initialColor, Color finalColor, float rotationSpeed)
            : base(location, texture, initialFrame, velocity)
        {            
            this.acceleration = acceleration;
            this.maxSpeed = maxSpeed;
            this.initialDuration = duration;
            this.remainingDuration = duration;
            this.initialColor = initialColor;
            this.finalColor = finalColor;
            this.rotationSpeed = rotationSpeed;
        }

        #endregion

        #region Methods

        public void Reinitialize(Vector2 location, Texture2D texture,
                                 Vector2 velocity, Vector2 acceleration, float maxSpeed,
                                 float duration, Color initialColor, Color finalColor, float rotationSpeed)
        {
            this.Location = location;
            this.Velocity = velocity;
            this.Frame = 0;

            this.acceleration = acceleration;
            this.maxSpeed = maxSpeed;
            this.initialDuration = duration;
            this.remainingDuration = duration;
            this.initialColor = initialColor;
            this.finalColor = finalColor;

            this.rotationSpeed = rotationSpeed;
        }

        public override void Update(float elapsed)
        {
            if (IsActive)
            {
                velocity += acceleration;

                if (velocity.Length() > maxSpeed)
                {
                    velocity.Normalize();
                    velocity *= maxSpeed;
                }

                if (remainingDuration > 0.25f)
                {
                    TintColor = Color.Lerp(initialColor,
                                           finalColor,
                                           this.DurationProgress);
                }
                else
                {
                    TintColor = Color.Lerp(initialColor,
                                           finalColor,
                                           this.DurationProgress) * (remainingDuration / 0.25f);
                }
                remainingDuration -= elapsed;

                base.Rotation += rotationSpeed;

                base.Update(elapsed);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
            {
                base.Draw(spriteBatch);
            }
            
        }

        #endregion

        #region Activate/Deactivate

        public new void Activated(Queue<string> data)
        {
            // Sprite
            base.Activated(data);

            this.acceleration.X = Single.Parse(data.Dequeue());
            this.acceleration.Y = Single.Parse(data.Dequeue());

            this.maxSpeed = Single.Parse(data.Dequeue());

            this.initialDuration = Single.Parse(data.Dequeue());
            this.remainingDuration = Single.Parse(data.Dequeue());

            this.initialColor = new Color(Int32.Parse(data.Dequeue()),
                                          Int32.Parse(data.Dequeue()),
                                          Int32.Parse(data.Dequeue()),
                                          Int32.Parse(data.Dequeue()));

            this.finalColor = new Color(Int32.Parse(data.Dequeue()),
                                        Int32.Parse(data.Dequeue()),
                                        Int32.Parse(data.Dequeue()),
                                        Int32.Parse(data.Dequeue()));

            this.rotationSpeed = Single.Parse(data.Dequeue());
        }

        public new void Deactivated(Queue<string> data)
        {
            // Sprite
            base.Deactivated(data);

            data.Enqueue(acceleration.X.ToString());
            data.Enqueue(acceleration.Y.ToString());

            data.Enqueue(maxSpeed.ToString());

            data.Enqueue(initialDuration.ToString());
            data.Enqueue(remainingDuration.ToString());

            data.Enqueue(((int)initialColor.R).ToString());
            data.Enqueue(((int)initialColor.G).ToString());
            data.Enqueue(((int)initialColor.B).ToString());
            data.Enqueue(((int)initialColor.A).ToString());

            data.Enqueue(((int)finalColor.R).ToString());
            data.Enqueue(((int)finalColor.G).ToString());
            data.Enqueue(((int)finalColor.B).ToString());
            data.Enqueue(((int)finalColor.A).ToString());

            data.Enqueue(rotationSpeed.ToString());
        }

        #endregion

        #region Properties

        public float ElapsedDuration
        {
            get
            {
                return initialDuration - remainingDuration;
            }
        }

        public float DurationProgress
        {
            get
            {
                return (float)ElapsedDuration / (float)initialDuration;
            }
        }

        public bool IsActive
        {
            get
            {
                return remainingDuration > 0;
            }
        }

        #endregion
    }
}
