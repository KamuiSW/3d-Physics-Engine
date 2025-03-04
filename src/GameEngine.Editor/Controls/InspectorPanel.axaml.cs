using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GameEngine.Editor.Controls
{
    public partial class InspectorPanel : UserControl
    {
        public InspectorPanel()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
} 