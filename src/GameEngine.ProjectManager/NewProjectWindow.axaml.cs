using Avalonia.Controls;
using Avalonia.Platform.Storage;
using GameEngine.Core.Project;
using GameEngine.Core.ViewModels;
using GameEngine.Core.Commands;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Generic;

namespace GameEngine.ProjectManager
{
    public partial class NewProjectWindow : Window
    {
        public NewProjectWindow()
        {
            InitializeComponent();
            var vm = new NewProjectViewModel(this);
            DataContext = vm;
            vm.RequestClose += (s, e) => Close();
        }
    }

    public class ProjectTemplate
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Func<ProjectMetadata, Task> SetupAction { get; set; } = _ => Task.CompletedTask;
    }

    public class NewProjectViewModel : ViewModelBase
    {
        private readonly ProjectService _projectService;
        private readonly Window _window;
        private string _projectName = string.Empty;
        private string _description = string.Empty;
        private string _location = string.Empty;
        private ProjectTemplate? _selectedTemplate;
        private string _projectNameError = string.Empty;
        private string _locationError = string.Empty;

        public event EventHandler? RequestClose;

        public string ProjectName
        {
            get => _projectName;
            set
            {
                if (SetField(ref _projectName, value))
                {
                    ValidateProjectName();
                    (CreateCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public string Description
        {
            get => _description;
            set => SetField(ref _description, value);
        }

        public string Location
        {
            get => _location;
            set
            {
                if (SetField(ref _location, value))
                {
                    ValidateLocation();
                    (CreateCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public ProjectTemplate? SelectedTemplate
        {
            get => _selectedTemplate;
            set
            {
                if (SetField(ref _selectedTemplate, value))
                {
                    (CreateCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public string ProjectNameError
        {
            get => _projectNameError;
            set
            {
                if (SetField(ref _projectNameError, value))
                {
                    OnPropertyChanged(nameof(HasProjectNameError));
                    (CreateCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public string LocationError
        {
            get => _locationError;
            set
            {
                if (SetField(ref _locationError, value))
                {
                    OnPropertyChanged(nameof(HasLocationError));
                    (CreateCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public bool HasProjectNameError => !string.IsNullOrEmpty(ProjectNameError);
        public bool HasLocationError => !string.IsNullOrEmpty(LocationError);

        public ObservableCollection<ProjectTemplate> Templates { get; } = new();

        public ICommand BrowseCommand { get; }
        public ICommand CreateCommand { get; }
        public ICommand CancelCommand { get; }

        public NewProjectViewModel(Window window)
        {
            _window = window;
            _projectService = new ProjectService();
            BrowseCommand = new RelayCommand(Browse);
            CreateCommand = new RelayCommand(Create, CanCreate);
            CancelCommand = new RelayCommand(() => RequestClose?.Invoke(this, EventArgs.Empty));

            InitializeTemplates();
            SelectedTemplate = Templates.FirstOrDefault();
        }

        private void InitializeTemplates()
        {
            Templates.Add(new ProjectTemplate
            {
                Name = "Empty Project",
                Description = "Create a new empty project with basic folder structure.",
                SetupAction = CreateEmptyProject
            });

            Templates.Add(new ProjectTemplate
            {
                Name = "3D Game Template",
                Description = "Create a new 3D game project with basic scene setup, player controller, and example assets.",
                SetupAction = Create3DGameProject
            });

            Templates.Add(new ProjectTemplate
            {
                Name = "Physics Demo",
                Description = "Create a new project with physics examples including rigid bodies, collisions, and joints.",
                SetupAction = CreatePhysicsDemo
            });
        }

        private async void Browse()
        {
            var topLevel = TopLevel.GetTopLevel(_window);
            if (topLevel == null) return;

            var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Select Project Location",
                AllowMultiple = false
            });

            var folder = folders.FirstOrDefault();
            if (folder != null)
            {
                Location = folder.Path.LocalPath;
            }
        }

        private async void Create()
        {
            if (!ValidateAll() || SelectedTemplate == null) return;

            try
            {
                var projectPath = Path.Combine(Location, ProjectName);
                var metadata = new ProjectMetadata
                {
                    Name = ProjectName,
                    Description = Description,
                    ProjectPath = projectPath
                };

                await _projectService.CreateProjectAsync(metadata);
                await SelectedTemplate.SetupAction(metadata);

                RequestClose?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                // Show error in location error field
                LocationError = ex.Message;
            }
        }

        private bool CanCreate()
        {
            if (string.IsNullOrWhiteSpace(ProjectName)) return false;
            if (string.IsNullOrWhiteSpace(Location)) return false;
            if (SelectedTemplate == null) return false;
            if (HasProjectNameError) return false;
            if (HasLocationError) return false;

            var projectPath = Path.Combine(Location, ProjectName);
            if (Directory.Exists(projectPath)) return false;

            return true;
        }

        private bool ValidateAll()
        {
            ValidateProjectName();
            ValidateLocation();
            return !HasProjectNameError && !HasLocationError;
        }

        private void ValidateProjectName()
        {
            if (string.IsNullOrWhiteSpace(ProjectName))
            {
                ProjectNameError = "Project name is required";
            }
            else if (ProjectName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                ProjectNameError = "Project name contains invalid characters";
            }
            else
            {
                ProjectNameError = string.Empty;
                ValidateLocation(); // Revalidate location since it depends on project name
            }
        }

        private void ValidateLocation()
        {
            if (string.IsNullOrWhiteSpace(Location))
            {
                LocationError = "Location is required";
            }
            else if (!Directory.Exists(Location))
            {
                LocationError = "Selected location does not exist";
            }
            else if (!string.IsNullOrWhiteSpace(ProjectName))
            {
                var projectPath = Path.Combine(Location, ProjectName);
                if (Directory.Exists(projectPath))
                {
                    LocationError = "A project with this name already exists at this location";
                }
                else
                {
                    LocationError = string.Empty;
                }
            }
            else
            {
                LocationError = string.Empty;
            }
        }

        private async Task CreateEmptyProject(ProjectMetadata metadata)
        {
            // Create a README file
            await File.WriteAllTextAsync(
                Path.Combine(metadata.ProjectPath, "README.md"),
                $"# {metadata.Name}\n\n{metadata.Description}\n\nCreated with Game Engine"
            );

            // Create an example script
            var scriptsPath = Path.Combine(metadata.ProjectPath, "Assets", "Scripts");
            await File.WriteAllTextAsync(
                Path.Combine(scriptsPath, "ExampleScript.cs"),
                @"using GameEngine.Core;

public class ExampleScript : Component
{
    public float Speed { get; set; } = 10.0f;

    public override void OnStart()
    {
        // Called when the script starts
    }

    public override void OnUpdate()
    {
        // Called every frame
    }
}"
            );
        }

        private async Task Create3DGameProject(ProjectMetadata metadata)
        {
            // Create example scene
            var scenesPath = Path.Combine(metadata.ProjectPath, "Scenes");
            var sceneContent = new Dictionary<string, object>
            {
                { "Name", "Example Scene" },
                { "Objects", new List<Dictionary<string, object>>
                    {
                        new Dictionary<string, object>
                        {
                            { "Name", "Main Camera" },
                            { "Position", new float[] { 0, 5, -10 } },
                            { "Rotation", new float[] { 30, 0, 0 } },
                            { "Components", new string[] { "Camera", "AudioListener" } }
                        },
                        new Dictionary<string, object>
                        {
                            { "Name", "Player" },
                            { "Position", new float[] { 0, 0, 0 } },
                            { "Components", new string[] { "PlayerController", "MeshRenderer", "Rigidbody" } }
                        },
                        new Dictionary<string, object>
                        {
                            { "Name", "Ground" },
                            { "Position", new float[] { 0, -1, 0 } },
                            { "Scale", new float[] { 100, 1, 100 } },
                            { "Components", new string[] { "MeshRenderer", "BoxCollider" } }
                        },
                        new Dictionary<string, object>
                        {
                            { "Name", "Directional Light" },
                            { "Position", new float[] { 0, 10, 0 } },
                            { "Rotation", new float[] { 50, -30, 0 } },
                            { "Components", new string[] { "Light" } }
                        }
                    }
                }
            };

            await File.WriteAllTextAsync(
                Path.Combine(scenesPath, "ExampleScene.scene"),
                JsonConvert.SerializeObject(sceneContent, Formatting.Indented)
            );

            // Create player controller script
            var scriptsPath = Path.Combine(metadata.ProjectPath, "Assets", "Scripts");
            await File.WriteAllTextAsync(
                Path.Combine(scriptsPath, "PlayerController.cs"),
                @"using GameEngine.Core;
using OpenTK.Mathematics;

public class PlayerController : Component
{
    public float MoveSpeed { get; set; } = 5.0f;
    public float JumpForce { get; set; } = 5.0f;
    private Rigidbody _rigidbody;

    public override void OnStart()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnUpdate()
    {
        var input = new Vector3(
            Input.GetAxis(""Horizontal""),
            0,
            Input.GetAxis(""Vertical"")
        );

        _rigidbody.AddForce(input * MoveSpeed);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _rigidbody.AddForce(Vector3.UnitY * JumpForce, ForceMode.Impulse);
        }
    }
}"
            );
        }

        private async Task CreatePhysicsDemo(ProjectMetadata metadata)
        {
            // Create physics demo scene
            var scenesPath = Path.Combine(metadata.ProjectPath, "Scenes");
            var sceneContent = new Dictionary<string, object>
            {
                { "Name", "Physics Demo" },
                { "Objects", new List<Dictionary<string, object>>
                    {
                        new Dictionary<string, object>
                        {
                            { "Name", "Main Camera" },
                            { "Position", new float[] { 0, 10, -15 } },
                            { "Rotation", new float[] { 30, 0, 0 } },
                            { "Components", new string[] { "Camera", "AudioListener" } }
                        },
                        new Dictionary<string, object>
                        {
                            { "Name", "Ground" },
                            { "Position", new float[] { 0, -1, 0 } },
                            { "Scale", new float[] { 100, 1, 100 } },
                            { "Components", new string[] { "MeshRenderer", "BoxCollider" } }
                        },
                        new Dictionary<string, object>
                        {
                            { "Name", "Domino Stack" },
                            { "Position", new float[] { 0, 0, 0 } },
                            { "Components", new string[] { "DominoStackSpawner" } }
                        },
                        new Dictionary<string, object>
                        {
                            { "Name", "Ball Spawner" },
                            { "Position", new float[] { 0, 10, 0 } },
                            { "Components", new string[] { "BallSpawner" } }
                        }
                    }
                }
            };

            await File.WriteAllTextAsync(
                Path.Combine(scenesPath, "PhysicsDemo.scene"),
                JsonConvert.SerializeObject(sceneContent, Formatting.Indented)
            );

            // Create spawner scripts
            var scriptsPath = Path.Combine(metadata.ProjectPath, "Assets", "Scripts");
            await File.WriteAllTextAsync(
                Path.Combine(scriptsPath, "DominoStackSpawner.cs"),
                @"using GameEngine.Core;
using OpenTK.Mathematics;

public class DominoStackSpawner : Component
{
    public int DominoCount { get; set; } = 10;
    public float Spacing { get; set; } = 1.5f;

    public override void OnStart()
    {
        for (int i = 0; i < DominoCount; i++)
        {
            var domino = GameObject.CreatePrimitive(PrimitiveType.Cube);
            domino.Transform.Position = new Vector3(i * Spacing, 1, 0);
            domino.Transform.Scale = new Vector3(0.2f, 2f, 1f);
            
            var rb = domino.AddComponent<Rigidbody>();
            rb.Mass = 1;
            
            var collider = domino.AddComponent<BoxCollider>();
            collider.Size = domino.Transform.Scale;
        }
    }
}"
            );

            await File.WriteAllTextAsync(
                Path.Combine(scriptsPath, "BallSpawner.cs"),
                @"using GameEngine.Core;
using OpenTK.Mathematics;

public class BallSpawner : Component
{
    public float SpawnInterval { get; set; } = 2.0f;
    private float _timer;

    public override void OnUpdate()
    {
        _timer += Time.DeltaTime;
        if (_timer >= SpawnInterval)
        {
            SpawnBall();
            _timer = 0;
        }
    }

    private void SpawnBall()
    {
        var ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.Transform.Position = Transform.Position;
        ball.Transform.Scale = Vector3.One * 0.5f;

        var rb = ball.AddComponent<Rigidbody>();
        rb.Mass = 5;
        rb.AddForce(Vector3.UnitX * Random.Range(-5f, 5f), ForceMode.Impulse);

        var collider = ball.AddComponent<SphereCollider>();
        collider.Radius = 0.5f;

        // Destroy after 10 seconds
        GameObject.Destroy(ball, 10f);
    }
}"
            );
        }
    }
} 