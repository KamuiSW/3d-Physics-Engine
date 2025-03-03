using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;

namespace GameEngine
{
    public class InputManager
    {
        private static KeyboardState keyboard;
        private static MouseState mouse;

        public static void Update(KeyboardState keyboardState, MouseState mouseState)
        {
            keyboard = keyboardState;
            mouse = mouseState;
        }

        public static bool IsKeyDown(Keys key)
        {
            return keyboard.IsKeyDown(key);
        }

        public static bool IsKeyPressed(Keys key)
        {
            return keyboard.IsKeyPressed(key);
        }

        public static Vector2 GetMousePosition()
        {
            return new Vector2(mouse.X, mouse.Y);
        }

        public static bool IsMouseButtonDown(MouseButton button)
        {
            return mouse.IsButtonDown(button);
        }
    }
} 