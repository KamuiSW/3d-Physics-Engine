using Avalonia.Controls;
using Avalonia.Interactivity;
using GameEngine.Core.Project;
using GameEngine.Editor;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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
            var wizard = new ProjectWizardWindow(_projectService);
            await wizard.ShowDialog(this);
            LoadRecentProjects();
        }

        private async void OnOpenProject(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Open Project"
            };

            var result = await dialog.ShowAsync(this);
            if (!string.IsNullOrEmpty(result))
            {
                var project = await _projectService.LoadProjectAsync(result);
                if (project != null)
                {
                    LoadRecentProjects();
                    LaunchEditor(project);
                }
                else
                {
                    await MessageBox.Show(this, "Selected folder is not a valid project.", "Error");
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
                    await MessageBox.Show(this, "Project no longer exists at the specified location.", "Error");
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