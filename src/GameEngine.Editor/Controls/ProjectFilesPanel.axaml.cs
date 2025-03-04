using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GameEngine.Editor.Controls
{
    public partial class ProjectFilesPanel : UserControl
    {
        public ProjectFilesPanel()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
} 