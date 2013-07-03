using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScribbleHunter.Windows.Inputs
{
    static class KeyboardHelper
    {
        private static float timer;
        private const float TimerLimit = 0.2f;

        private static Keys[] textKeys = { Keys.Enter, Keys.Escape, Keys.Back,
                                           Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H,
                                           Keys.I, Keys.J, Keys.K, Keys.L, Keys.M, Keys.N, Keys.O, Keys.P,
                                           Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z,
                                           Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9};

        public static bool IsKeyDown(Keys key)
        {
            return Keyboard.GetState(PlayerIndex.One).IsKeyDown(key);
        }

        public static bool IsKeyDownDelay(Keys key)
        {
            if (timer < TimerLimit)
                return false;

            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(key))
            {
                timer = 0.0f;
                return true;
            }

            return false;
        }

        public static Keys IsAnyTextOrAcceptKeyDownDelay()
        {
            if (timer < TimerLimit)
                return Keys.None;

            Keys[] pressedKeys = Keyboard.GetState(PlayerIndex.One).GetPressedKeys();

            if (pressedKeys.Length > 0 && textKeys.Contains(pressedKeys[0]))
            {
                timer = 0.0f;
                return pressedKeys[0];
            }

            return Keys.None;
        }

        public static bool IsDeleteKey(Keys key)
        {
            return (key == Keys.Back || key == Keys.Delete);
        }

        public static bool IsAcceptKey(Keys key)
        {
            return (key == Keys.Escape || key == Keys.Enter);
        }

        public static void Update(float elapsed)
        {
            timer += elapsed;
        }
    }
}
