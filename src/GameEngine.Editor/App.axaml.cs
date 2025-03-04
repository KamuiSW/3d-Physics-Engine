using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using GameEngine.Core.Project;

namespace GameEngine.Editor
{
    public partial class App : Application
    {
        private ProjectMetadata? _projectMetadata;

        public App()
        {
        }

        public App(ProjectMetadata projectMetadata)
        {
            _projectMetadata = projectMetadata;
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = _projectMetadata != null 
                    ? new MainEditorWindow(_projectMetadata)
                    : new MainEditorWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
} 