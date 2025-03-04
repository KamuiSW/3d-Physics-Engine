using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using GameEngine.Core.Project;
using GameEngine.Editor;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System.Collections.ObjectModel;
using System.Linq;

namespace GameEngine.ProjectManager
{
    public partial class MainWindow : Window
    {
        private readonly ProjectService _projectService;
        public ObservableCollection<ProjectMetadata> RecentProjects { get; } = new();

        public MainWindow()
        {
            InitializeComponent();
            _projectService = new ProjectService();
            DataContext = this;
            LoadRecentProjects();
        }

        private async void LoadRecentProjects()
        {
            var projects = await _projectService.LoadRecentProjectsAsync();
            RecentProjects.Clear();
            foreach (var project in projects)
            {
                RecentProjects.Add(project);
            }
        }

        private async void OnNewProject(object sender, RoutedEventArgs e)
        {
            var wizard = new NewProjectWindow();
            await wizard.ShowDialog(this);
            LoadRecentProjects();
        }

        private async void OnOpenProject(object sender, RoutedEventArgs e)
        {
            var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Open Project",
                AllowMultiple = false
            });

            var folder = folders.FirstOrDefault();
            if (folder != null)
            {
                var project = await _projectService.LoadProjectAsync(folder.Path.LocalPath);
                if (project != null)
                {
                    LoadRecentProjects();
                    LaunchEditor(project);
                }
                else
                {
                    var messageBox = MessageBoxManager
                        .GetMessageBoxStandard("Error", "Selected folder is not a valid project.", ButtonEnum.Ok);
                    await messageBox.ShowAsync();
                }
            }
        }

        private async void OnProjectSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems?.Count > 0 && e.AddedItems[0] is ProjectMetadata project)
            {
                var loadedProject = await _projectService.LoadProjectAsync(project.ProjectPath);
                if (loadedProject != null)
                {
                    LaunchEditor(loadedProject);
                }
                else
                {
                    var messageBox = MessageBoxManager
                        .GetMessageBoxStandard("Error", "Project no longer exists at the specified location.", ButtonEnum.Ok);
                    await messageBox.ShowAsync();
                    LoadRecentProjects();
                }
            }
        }

        private void LaunchEditor(ProjectMetadata project)
        {
            var editorWindow = new MainEditorWindow(project);
            editorWindow.Show();
            this.Hide();

            editorWindow.Closed += (s, e) =>
            {
                this.Show();
            };
        }
    }
} 