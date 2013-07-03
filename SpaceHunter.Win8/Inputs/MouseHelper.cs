

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using ScribbleHunter.Inputs;

namespace ScribbleHunter.Windows.Inputs
{
    static class MouseHelper
    {
        private static Game game;
        private static Texture2D texture;
        private static Rectangle source;
        private readonly static Rectangle CrosshaoirSource = new Rectangle(480, 1425, 50, 50);
        private static int windowHeight;
        private static int windowWidth;

        private static float timer;
        private const float TimerLimit = 0.25f;

        private const float MouseSpeed = 7;
        public const int MouseTopLimit = 10;

        private static float mouseOpacity;

        private static float lastMouseX;
        private static float lastMouseY;

        private static bool isCrosshair = false;

        public static void Initialize(Game g, Texture2D tex, Rectangle src, int winWidth, int winHeight)
        {
            game = g;
            texture = tex;
            source = src;
            windowHeight = winHeight;
            windowWidth = winWidth;
            Position = new Vector2(400, 240);

            lastMouseX = (float)(Mouse.GetState().X / WidthFactor);
            lastMouseY = (float)(Mouse.GetState().Y / HeightFactor);
        }

        public static bool IsMouseInRegion(Rectangle region)
        {
            Vector2 pos = Position;

            return region.Contains((int)pos.X, (int)pos.Y);
        }

        public static bool IsLeftMouseDown()
        {
            bool val = Mouse.GetState().LeftButton == ButtonState.Pressed;

            if (val)
                WakuUpMouse();

            return val;
        }

        public static bool IsLeftMouseDown(Rectangle region)
        {
            if (IsMouseInRegion(region))
            {
                bool val = Mouse.GetState().LeftButton == ButtonState.Pressed;

                if (val)
                    WakuUpMouse();

                return val;
            }
            else
                return false;
        }

        public static bool IsLeftMouseDownDelay()
        {
            if (timer < TimerLimit)
                return false;

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                timer = 0.0f;
                return true;
            }

            return false;
        }

        public static bool IsLeftMouseDownDelay(Rectangle region)
        {
            if (timer < TimerLimit)
                return false;

            if (IsMouseInRegion(region))
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    timer = 0.0f;
                    return true;
                }
            }

            return false;
        }

        public static bool IsActionPressedInRegion(GameInput input, string action, Rectangle region)
        {
            return input.IsPressed(action) && IsMouseInRegion(region);
        }

        public static void Move(Vector2 direction)
        {
            direction.Normalize();
            Position = Position + direction * MouseSpeed;
        }

        public static Vector2 Position
        {
            get
            {
                float newX = (float)(Mouse.GetState().X / WidthFactor);
                float newY = (float)(Mouse.GetState().Y / HeightFactor);

                if (newX != lastMouseX || newY != lastMouseY)
                {
                    mouseOpacity = 1.0f;
                }

                lastMouseX = newX;
                lastMouseY = newY;

                return new Vector2(newX, newY);
            }
            set
            {
                Mouse.SetPosition((int)(value.X * WidthFactor), (int)(value.Y * HeightFactor));
            }
        }


        public static void Update(float elapsed)
        {
            timer += elapsed;
            mouseOpacity -= 0.0025f;
            MathHelper.Clamp(mouseOpacity, 0, 1);
        }

        public static void Draw(SpriteBatch batch)
        {
            Vector2 pos = Position;

            if (pos.Y > MouseTopLimit)
            {
                if (isCrosshair)
                {
                    batch.Draw(
                        texture,
                        new Rectangle(
                            (int)pos.X - CrosshaoirSource.Width / 2, (int)pos.Y - CrosshaoirSource.Height / 2,
                            CrosshaoirSource.Width, CrosshaoirSource.Height),
                        CrosshaoirSource,
                        Color.White * mouseOpacity);
                }
                else
                {
                    batch.Draw(
                        texture,
                        new Rectangle(
                            (int)pos.X, (int)pos.Y,
                            source.Width, source.Height),
                        source,
                        Color.White * mouseOpacity);
                }
            }
        }

        private static float HeightFactor
        {
            get
            {
                return (float)game.Window.ClientBounds.Height / windowHeight;
            }
        }

        private static float WidthFactor
        {
            get
            {
                return (float)game.Window.ClientBounds.Width / windowWidth;
            }
        }

        public static void WakuUpMouse()
        {
            mouseOpacity = 1.0f;
        }

        public static bool IsCrosshair
        {
            get { return MouseHelper.isCrosshair; }
            set { MouseHelper.isCrosshair = value; }
        }
    }
}
