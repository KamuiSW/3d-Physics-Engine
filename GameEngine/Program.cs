global using OpenTK.Mathematics;
global using OpenTK.Graphics.OpenGL4;
global using OpenTK.Windowing.Common;
global using OpenTK.Windowing.Desktop;
global using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using GameEngine.Examples;

namespace GameEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var game = new TestScene())
            {
                game.Start();
            }
        }
    }
} 