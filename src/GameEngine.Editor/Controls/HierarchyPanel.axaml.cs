using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using GameEngine.Editor.ViewModels;

namespace GameEngine.Editor.Controls
{
    public partial class HierarchyPanel : UserControl
    {
        public HierarchyPanel()
        {
            InitializeComponent();
            DataContext = new HierarchyViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
} 