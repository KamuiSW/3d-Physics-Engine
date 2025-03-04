using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using GameEngine.Editor.ViewModels;

namespace GameEngine.Editor.Controls
{
    public partial class SceneView : UserControl
    {
        public SceneView()
        {
            InitializeComponent();
            DataContext = new SceneViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
} 