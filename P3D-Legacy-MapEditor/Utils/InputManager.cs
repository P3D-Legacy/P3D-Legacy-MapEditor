using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace P3D.Legacy.MapEditor.Utils
{
    public enum AnalogStickDirection { None, Up, UpRight, Right, DownRight, Down, DownLeft, Left, UpLeft }

    public static class InputManager
    {
        #region Fields

        public static MouseState CurrentMouseState;
        private static KeyboardState CurrentKeyboardState;
        public static List<Keys> CurrentOtherKeys = new List<Keys>();

        private static MouseState LastMouseState;
        private static KeyboardState LastKeyboardState;
        public static List<Keys> LastOtherKeys = new List<Keys>();

        #endregion Fields

        #region Properties

        public static Point MousePosition => CurrentMouseState.Position;

        public static Keys[] CurrentKeys { get { var list = new List<Keys>(CurrentOtherKeys); list.AddRange(CurrentKeyboardState.GetPressedKeys()); return list.ToArray(); } }
        public static Keys[] LastKeys { get { var list = new List<Keys>(LastOtherKeys); list.AddRange(LastKeyboardState.GetPressedKeys()); return list.ToArray(); } }

        #endregion Properties

        public static void Update(GameTime time)
        {
            LastOtherKeys = new List<Keys>(CurrentOtherKeys);
            CurrentOtherKeys.Clear();

            LastMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();

            LastKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();
        }

        public static bool IsCurrentKeyPressed(Keys key) { return CurrentKeys.Any(pressedKey => pressedKey == key); }
        public static bool IsLastKeyPressed(Keys key) { return LastKeys.Any(pressedKey => pressedKey == key); }
        public static bool IsOncePressed(Keys key) { return IsCurrentKeyPressed(key) && !IsLastKeyPressed(key); }
    }
}
