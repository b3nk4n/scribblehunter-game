using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ScribbleHunter
{
    public class DamageExplosion
    {
        /// <summary>
        /// The location center of the explosion.
        /// </summary>
        private Vector2 location;

        /// <summary>
        /// The starting radius.
        /// </summary>
        private float radiusStart;

        /// <summary>
        /// The final radius.
        /// </summary>
        private float radiusEnd;

        /// <summary>
        /// The current explosion radius.
        /// </summary>
        private float currentRadius;

        /// <summary>
        /// The duration of the explosion in seconds.
        /// </summary>
        private float duration;

        /// <summary>
        /// The current duration.
        /// </summary>
        private float currentDuration;

        /// <summary>
        /// The rotation.
        /// </summary>
        private float rotation;

        /// <summary>
        /// The color.
        /// </summary>
        private Color tintColor;

        /// <summary>
        /// The rotation per frame.
        /// </summary>
        private const float ROTATION_VALUE = 1.00f;

        private readonly Rectangle ExplosionSource = new Rectangle(1200, 0, 400, 400);

        /// <summary>
        /// Constructor just for activation/deactivation.
        /// </summary>
        public DamageExplosion()
        {

        }

        /// <summary>
        /// Creates a new damage explosion.
        /// </summary>
        /// <param name="location">The location center</param>
        /// <param name="radiusStart">The starting radius</param>
        /// <param name="radiusEnd">The final radius</param>
        /// <param name="duration">The duration of the explosion</param>
        public DamageExplosion(Vector2 location, float radiusStart, float radiusEnd, float duration, Color tint)
        {
            this.location = location;
            this.radiusStart = radiusStart;
            this.radiusEnd = radiusEnd;
            this.duration = duration;
            this.currentDuration = 0.0f;
            this.tintColor = tint;
        }

        /// <summary>
        /// Updates the damage explosion.
        /// </summary>
        /// <param name="gameTime">The current game time</param>
        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            currentDuration += elapsed;
            rotation += ROTATION_VALUE;

            this.currentRadius = radiusStart + ((radiusEnd - radiusStart) * currentDuration / duration);
        }

        /// <summary>
        /// Draws the explosion border for debugging
        /// </summary>
        /// <param name="batch">The sprite batch</param>
        public void Draw(SpriteBatch batch)
        {
            Rectangle destination = new Rectangle((int)(location.X),
                                                  (int)(location.Y),
                                                  (int)(currentRadius * 2),
                                                  (int)(currentRadius * 2));

            Color c;

            if (duration - currentRadius > 0.33f)
                c = tintColor;
            else
                c = tintColor * ((duration - currentDuration) / 0.33f);

            batch.Draw(
                DamageExplosionManager.Texture,
                destination,
                ExplosionSource,
                c,
                rotation,
                new Vector2(200, 200),
                SpriteEffects.None,
                0.0f);
        }

        #region Activate/Deactivate

        public void Activated(Queue<string> data)
        {
            this.location.X = Single.Parse(data.Dequeue());
            this.location.Y = Single.Parse(data.Dequeue());

            this.radiusStart = Single.Parse(data.Dequeue());
            this.radiusEnd = Single.Parse(data.Dequeue());
            this.currentRadius = Single.Parse(data.Dequeue());

            this.duration = Single.Parse(data.Dequeue());
            this.currentDuration = Single.Parse(data.Dequeue());

            this.rotation = Single.Parse(data.Dequeue());

            this.tintColor = new Color(Int32.Parse(data.Dequeue()),
                                       Int32.Parse(data.Dequeue()),
                                       Int32.Parse(data.Dequeue()),
                                       Int32.Parse(data.Dequeue()));
        }

        public void Deactivated(Queue<string> data)
        {
            data.Enqueue(this.location.X.ToString());
            data.Enqueue(this.location.Y.ToString());

            data.Enqueue(this.radiusStart.ToString());
            data.Enqueue(this.radiusEnd.ToString());
            data.Enqueue(this.currentRadius.ToString());

            data.Enqueue(this.duration.ToString());
            data.Enqueue(this.currentDuration.ToString());

            data.Enqueue(this.rotation.ToString());

            data.Enqueue(((int)tintColor.R).ToString());
            data.Enqueue(((int)tintColor.G).ToString());
            data.Enqueue(((int)tintColor.B).ToString());
            data.Enqueue(((int)tintColor.A).ToString());
        }

        #endregion

        /// <summary>
        /// Gets the center location of the explosion.
        /// </summary>
        public Vector2 Location
        {
            get
            {
                return this.location;
            }
        }

        /// <summary>
        /// Gets the current explosion radius of the explosion.
        /// </summary>
        public float ExplosionRadius
        {
            get
            {
                return this.currentRadius;
            }
        }

        /// <summary>
        /// Gets the value indication whether the exposion is elapsed or not.
        /// </summary>
        public bool IsElapsed
        {
            get
            {
                return currentDuration >= duration;
            }
        }
    }
}
