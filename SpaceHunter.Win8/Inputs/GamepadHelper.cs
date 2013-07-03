using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ScribbleHunter.Windows.Inputs
{
    class GamepadHelper
    {
        private const float ThumbstickLimit = 0.5f;

        public static bool IsLeftThumbstickLeft()
        {
            return GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X < -ThumbstickLimit;
        }

        public static bool IsLeftThumbstickRight()
        {
            return GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X > ThumbstickLimit;
        }

        public static bool IsLeftThumbstickUp()
        {
            return GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y > ThumbstickLimit;
        }

        public static bool IsLeftThumbstickDown()
        {
            return GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y < -ThumbstickLimit;
        }

        public static bool IsRightThumbstickLeft()
        {
            return GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X < -ThumbstickLimit;
        }

        public static bool IsRightThumbstickRight()
        {
            return GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X > ThumbstickLimit;
        }

        public static bool IsRightThumbstickUp()
        {
            return GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y > ThumbstickLimit;
        }

        public static bool IsRightThumbstickDown()
        {
            return GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y < -ThumbstickLimit;
        }

        public static Vector2 LeftThumbstickDirection
        {
            get
            {
                Vector2 thumbLeft = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left;

                if (thumbLeft.Length() < ThumbstickLimit)
                    return Vector2.Zero;
                else
                    return thumbLeft;
            }
        }

        public static Vector2 RightThumbstickDirection
        {
            get
            {
                Vector2 thumbLeft = GamePad.GetState(PlayerIndex.One).ThumbSticks.Right;

                if (thumbLeft.Length() < ThumbstickLimit)
                    return Vector2.Zero;
                else
                    return thumbLeft;
            }
        }
    }
}
