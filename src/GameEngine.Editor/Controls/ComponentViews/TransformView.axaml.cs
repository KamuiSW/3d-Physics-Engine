using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GameEngine.Editor.Controls.ComponentViews
{
    public partial class TransformView : UserControl
    {
        public TransformView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
} 