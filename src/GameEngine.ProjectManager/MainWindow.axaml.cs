using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using GameEngine.Core.Project;
using GameEngine.Editor.Views;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Diagnostics;

namespace GameEngine.ProjectManager
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly ProjectService _projectService;
        private ObservableCollection<ProjectMetadata> _recentProjects;

        public ObservableCollection<ProjectMetadata> RecentProjects
        {
            get => _recentProjects;
            set
            {
                if (_recentProjects != value)
                {
                    _recentProjects = value;
                    OnPropertyChanged();
                    Debug.WriteLine($"RecentProjects updated. Count: {_recentProjects?.Count ?? 0}");
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            _projectService = new ProjectService();
            _recentProjects = new ObservableCollection<ProjectMetadata>();
            DataContext = this;
            LoadRecentProjects();
            Debug.WriteLine($"MainWindow initialized. Projects count: {_recentProjects.Count}");
        }

        private void LoadRecentProjects()
        {
            try
            {
                string appDataPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "GameEngine");

                string recentProjectsPath = Path.Combine(appDataPath, "recentProjects.json");
                Debug.WriteLine($"Loading projects from: {recentProjectsPath}");

                _recentProjects.Clear();

                if (File.Exists(recentProjectsPath))
                {
                    string json = File.ReadAllText(recentProjectsPath);
                    Debug.WriteLine($"Loaded JSON: {json}");
                    var projects = JsonSerializer.Deserialize<ProjectMetadata[]>(json);
                    if (projects != null)
                    {
                        foreach (var project in projects)
                        {
                            Debug.WriteLine($"Checking project: {project.Name} at {project.ProjectPath}");
                            if (Directory.Exists(project.ProjectPath))
                            {
                                _recentProjects.Add(project);
                                Debug.WriteLine($"Added project: {project.Name}");
                            }
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("No recent projects file found");
                }

                Debug.WriteLine($"Loaded {_recentProjects.Count} projects");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading projects: {ex}");
                var messageBox = MessageBoxManager
                    .GetMessageBoxStandard("Error", $"Error loading recent projects: {ex.Message}", ButtonEnum.Ok);
                messageBox.ShowAsync();
            }
        }

        private void SaveRecentProjects()
        {
            try
            {
                string appDataPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "GameEngine");

                Directory.CreateDirectory(appDataPath);

                string recentProjectsPath = Path.Combine(appDataPath, "recentProjects.json");
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(_recentProjects.ToArray(), options);
                File.WriteAllText(recentProjectsPath, json);
                Debug.WriteLine($"Saved projects to: {recentProjectsPath}");
                Debug.WriteLine($"Saved JSON: {json}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving projects: {ex}");
                var messageBox = MessageBoxManager
                    .GetMessageBoxStandard("Error", $"Error saving recent projects: {ex.Message}", ButtonEnum.Ok);
                messageBox.ShowAsync();
            }
        }

        private async void OnNewProject(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Creating new project...");
            var wizard = new NewProjectWindow();
            var result = await wizard.ShowDialog<ProjectMetadata>(this);
            
            if (result != null)
            {
                Debug.WriteLine($"New project created: {result.Name} at {result.ProjectPath}");
                
                try
                {
                    // Ensure the project is properly loaded
                    var loadedProject = await _projectService.LoadProjectAsync(result.ProjectPath);
                    if (loadedProject != null)
                    {
                        Debug.WriteLine("Project loaded successfully after creation");
                        AddToRecentProjects(loadedProject);
                        
                        // Force UI refresh
                        var temp = _recentProjects.ToList();
                        _recentProjects.Clear();
                        foreach (var proj in temp)
                        {
                            _recentProjects.Add(proj);
                        }
                        
                        LaunchEditor(loadedProject);
                    }
                    else
                    {
                        Debug.WriteLine("Failed to load newly created project, using original result");
                        AddToRecentProjects(result);
                        
                        // Force UI refresh
                        var temp = _recentProjects.ToList();
                        _recentProjects.Clear();
                        foreach (var proj in temp)
                        {
                            _recentProjects.Add(proj);
                        }
                        
                        LaunchEditor(result);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error loading new project: {ex.Message}");
                    AddToRecentProjects(result);
                    
                    // Force UI refresh
                    var temp = _recentProjects.ToList();
                    _recentProjects.Clear();
                    foreach (var proj in temp)
                    {
                        _recentProjects.Add(proj);
                    }
                    
                    LaunchEditor(result);
                }

                // Save immediately after adding
                SaveRecentProjects();
                
                // Reload the list to ensure it's displayed
                LoadRecentProjects();
            }
            else
            {
                Debug.WriteLine("Project creation cancelled or failed");
            }
        }

        private async void OnOpenProject(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Opening project...");
            var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Open Project",
                AllowMultiple = false
            });

            var folder = folders.FirstOrDefault();
            if (folder != null)
            {
                Debug.WriteLine($"Selected folder: {folder.Path.LocalPath}");
                var project = await _projectService.LoadProjectAsync(folder.Path.LocalPath);
                if (project != null)
                {
                    Debug.WriteLine($"Project loaded: {project.Name}");
                    AddToRecentProjects(project);
                    LaunchEditor(project);
                }
                else
                {
                    Debug.WriteLine("Invalid project folder");
                    var messageBox = MessageBoxManager
                        .GetMessageBoxStandard("Error", "Selected folder is not a valid project.", ButtonEnum.Ok);
                    await messageBox.ShowAsync();
                }
            }
        }

        private void AddToRecentProjects(ProjectMetadata project)
        {
            Debug.WriteLine($"Adding project to recent list: {project.Name}");
            
            // Remove existing project with same path if exists
            var existing = _recentProjects.FirstOrDefault(p => p.ProjectPath == project.ProjectPath);
            if (existing != null)
            {
                Debug.WriteLine("Removing existing project with same path");
                _recentProjects.Remove(existing);
            }

            // Add to start of list
            _recentProjects.Insert(0, project);
            Debug.WriteLine($"Project added. New count: {_recentProjects.Count}");

            // Keep only the last 10 projects
            while (_recentProjects.Count > 10)
            {
                _recentProjects.RemoveAt(_recentProjects.Count - 1);
                Debug.WriteLine("Removed oldest project (limit 10)");
            }

            SaveRecentProjects();
        }

        private async void OnProjectSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems?.Count > 0 && e.AddedItems[0] is ProjectMetadata project)
            {
                Debug.WriteLine($"Project selected: {project.Name}");
                if (Directory.Exists(project.ProjectPath))
                {
                    var loadedProject = await _projectService.LoadProjectAsync(project.ProjectPath);
                    if (loadedProject != null)
                    {
                        Debug.WriteLine("Project loaded successfully");
                        AddToRecentProjects(loadedProject); // Move to top of list
                        LaunchEditor(loadedProject);
                    }
                }
                else
                {
                    Debug.WriteLine("Project directory not found");
                    var messageBox = MessageBoxManager
                        .GetMessageBoxStandard("Error", "Project no longer exists at the specified location.", ButtonEnum.Ok);
                    await messageBox.ShowAsync();
                    _recentProjects.Remove(project);
                    SaveRecentProjects();
                }
            }
        }

        private void LaunchEditor(ProjectMetadata project)
        {
            Debug.WriteLine($"Launching editor for project: {project.Name}");
            var editorWindow = new MainEditorWindow(project);
            editorWindow.Show();
            this.Hide();

            editorWindow.Closed += (s, e) =>
            {
                Debug.WriteLine("Editor closed, showing project manager");
                this.Show();
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 