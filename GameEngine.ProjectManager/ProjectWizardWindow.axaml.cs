using Avalonia.Controls;
using Avalonia.Interactivity;
using GameEngine.Core.Project;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GameEngine.ProjectManager
{
    public partial class ProjectWizardWindow : Window
    {
        private readonly ProjectService _projectService;
        private string _selectedPath = string.Empty;

        public ProjectWizardWindow(ProjectService projectService)
        {
            InitializeComponent();
            _projectService = projectService;
        }

        private async void OnBrowseLocation(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Select Project Location"
            };

            var result = await dialog.ShowAsync(this);
            if (!string.IsNullOrEmpty(result))
            {
                _selectedPath = result;
                LocationTextBox.Text = result;
            }
        }

        private async void OnCreate(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ProjectNameTextBox.Text))
            {
                await MessageBox.Show(this, "Please enter a project name.", "Validation Error");
                return;
            }

            if (string.IsNullOrWhiteSpace(_selectedPath))
            {
                await MessageBox.Show(this, "Please select a project location.", "Validation Error");
                return;
            }

            var projectPath = Path.Combine(_selectedPath, ProjectNameTextBox.Text);
            if (Directory.Exists(projectPath))
            {
                await MessageBox.Show(this, "A project with this name already exists at the selected location.", "Error");
                return;
            }

            try
            {
                var projectMetadata = new ProjectMetadata
                {
                    Name = ProjectNameTextBox.Text,
                    Description = DescriptionTextBox.Text,
                    ProjectPath = projectPath
                };

                await _projectService.CreateProjectAsync(projectMetadata);
                Close();
            }
            catch (Exception ex)
            {
                await MessageBox.Show(this, $"Failed to create project: {ex.Message}", "Error");
            }
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
} 