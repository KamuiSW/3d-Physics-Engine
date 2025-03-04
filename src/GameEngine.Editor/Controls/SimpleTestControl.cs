using Avalonia.Controls;
using Avalonia.Media;

namespace GameEngine.Editor.Controls
{
    public class SimpleTestControl : Panel
    {
        public SimpleTestControl()
        {
            Background = Brushes.LightGray;
            Children.Add(new TextBlock
            {
                Text = "Test Control",
                Foreground = Brushes.Black
            });
        }
    }
} 