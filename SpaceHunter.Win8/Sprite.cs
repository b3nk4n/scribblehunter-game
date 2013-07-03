using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace ScribbleHunter
{
    class Sprite
    {
        #region Members

        public Texture2D Texture;

        protected List<Rectangle> frames = new List<Rectangle>(16);
        private int frameWidth;
        private int frameHeight;
        private int currentFrame;
        private float frameTime = 0.1f;
        private float timeForCurrentFrame = 0.0f;

        private Color tintColor = Color.White;
        private float rotation = 0.0f;

        public int CollisionRadius = 0;
        public int BoundingXPadding = 0;
        public int BoundingYPadding = 0;

        protected Vector2 location = Vector2.Zero;
        protected Vector2 velocity = Vector2.Zero;

        #endregion

        #region Constructors

        public Sprite(Vector2 location, Texture2D texture,
                      Rectangle initialFrame, Vector2 velocity)
        {
            this.location = location;
            this.Texture = texture;
            this.velocity = velocity;

            this.frames.Add(initialFrame);
            this.frameWidth = initialFrame.Width;
            this.frameHeight = initialFrame.Height;
        }

        #endregion

        #region Methods

        public bool isBoxColliding(Rectangle otherBox)
        {
            return this.BoundingBoxRect.Intersects(otherBox);
        }

        public bool IsCircleColliding(Vector2 otherCenter, float otherRadius)
        {
            if (Vector2.Distance(this.Center, otherCenter) < this.CollisionRadius + otherRadius)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddFrame(Rectangle frameRect)
        {
            this.frames.Add(frameRect);
        }

        public void ReplaceFrames(Rectangle newFrame)
        {
            this.frames.Clear();
            this.frames.Add(newFrame);
        }

        public virtual void Update(float elapsed)
        {
            this.timeForCurrentFrame += elapsed;

            if (this.timeForCurrentFrame >= this.FrameTime)
            {
                this.currentFrame = (++this.currentFrame) % this.frames.Count;
                this.timeForCurrentFrame = this.timeForCurrentFrame - this.FrameTime;
            }

            location += (this.Velocity * elapsed);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture,
                             Center,
                             Source,
                             TintColor,
                             Rotation,
                             new Vector2(frameWidth / 2, frameHeight / 2),
                             1.0f,
                             SpriteEffects.None,
                             0.0f);
        }

        public void RotateTo(Vector2 direction)
        {
            this.Rotation = (float)Math.Atan2(direction.Y, direction.X);
        }

        #endregion

        #region Activate/Deactivate

        public void Activated(Queue<string> data)
        {
            this.currentFrame = Int32.Parse(data.Dequeue());
            this.timeForCurrentFrame = Single.Parse(data.Dequeue());

            this.TintColor = new Color(Int32.Parse(data.Dequeue()),
                                       Int32.Parse(data.Dequeue()),
                                       Int32.Parse(data.Dequeue()),
                                       Int32.Parse(data.Dequeue()));

            this.rotation = Single.Parse(data.Dequeue());

            this.CollisionRadius = Int32.Parse(data.Dequeue());
            this.BoundingXPadding = Int32.Parse(data.Dequeue());
            this.BoundingYPadding = Int32.Parse(data.Dequeue());

            this.location = new Vector2(Single.Parse(data.Dequeue()),
                                        Single.Parse(data.Dequeue()));

            this.velocity = new Vector2(Single.Parse(data.Dequeue()),
                                        Single.Parse(data.Dequeue()));
        }

        public void Deactivated(Queue<string> data)
        {
            data.Enqueue(currentFrame.ToString());
            data.Enqueue(timeForCurrentFrame.ToString());

            data.Enqueue(((int)tintColor.R).ToString());
            data.Enqueue(((int)tintColor.G).ToString());
            data.Enqueue(((int)tintColor.B).ToString());
            data.Enqueue(((int)tintColor.A).ToString());

            data.Enqueue(rotation.ToString());

            data.Enqueue(CollisionRadius.ToString());
            data.Enqueue(BoundingXPadding.ToString());
            data.Enqueue(BoundingYPadding.ToString());

            data.Enqueue(location.X.ToString());
            data.Enqueue(location.Y.ToString());

            data.Enqueue(velocity.X.ToString());
            data.Enqueue(velocity.Y.ToString());
        }

        #endregion

        #region Properties

        public Vector2 Location
        {
            get
            {
                return this.location;
            }
            set
            {
                this.location = value;
            }
        }

        public Vector2 Velocity
        {
            get
            {
                return this.velocity;
            }
            set
            {
                this.velocity = value;
            }
        }

        public Color TintColor
        {
            get
            {
                return this.tintColor;
            }
            set
            {
                this.tintColor = value;
            }
        }

        public float Rotation
        {
            get
            {
                return this.rotation;
            }
            set
            {
                this.rotation = value;
            }
        }

        public int Frame
        {
            get
            {
                return this.currentFrame;
            }
            set
            {
                this.currentFrame = (int)MathHelper.Clamp(value, 0, frames.Count - 1);
            }
        }

        public float FrameTime
        {
            get
            {
                return this.frameTime;
            }
            set
            {
                this.frameTime = MathHelper.Max(0, value);
            }
        }

        public float FrameHeight
        {
            get { return frameHeight; }
        }

        public float FrameWidth
        {
            get { return frameWidth; }
        }

        public Rectangle Source
        {
            get
            {
                return this.frames[currentFrame];
            }
        }

        public Rectangle Destination
        {
            get
            {
                return new Rectangle((int)location.X,
                                     (int)location.Y,
                                     frameWidth,
                                     frameHeight);
            }
        }

        public Vector2 Center
        {
            get
            {
                return location + new Vector2(frameWidth / 2,
                                              frameHeight / 2);
            }
        }

        public Rectangle BoundingBoxRect
        {
            get
            {
                return new Rectangle((int)location.X + BoundingXPadding,
                                     (int)location.Y + BoundingYPadding,
                                     frameWidth - (2 * BoundingXPadding),
                                     frameHeight - (2 * BoundingYPadding));
            }
        }

        #endregion
    }
}
