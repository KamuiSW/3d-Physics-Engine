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