using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using GameEngine.Core.Project;
using GameEngine.Editor.Viewport;
using System;
using System.Windows.Input;

namespace GameEngine.Editor
{
    public partial class MainEditorWindow : Window
    {
        private readonly ProjectMetadata _projectMetadata;
        private readonly CameraController _cameraController;
        private string _statusMessage = "Ready";

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                // TODO: Implement property change notification
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand SaveAllCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }
        public ICommand ResetLayoutCommand { get; }
        public ICommand PlayCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand StopCommand { get; }

        public MainEditorWindow(ProjectMetadata projectMetadata)
        {
            _projectMetadata = projectMetadata;
            _cameraController = new CameraController();
            
            InitializeComponent();
            DataContext = this;

            Title = $"Game Engine Editor - {_projectMetadata.Name}";

            // Initialize commands
            SaveCommand = new RelayCommand(OnSave);
            SaveAllCommand = new RelayCommand(OnSaveAll);
            ExitCommand = new RelayCommand(OnExit);
            UndoCommand = new RelayCommand(OnUndo);
            RedoCommand = new RelayCommand(OnRedo);
            ResetLayoutCommand = new RelayCommand(OnResetLayout);
            PlayCommand = new RelayCommand(OnPlay);
            PauseCommand = new RelayCommand(OnPause);
            StopCommand = new RelayCommand(OnStop);

            InitializeLayout();
        }

        private void InitializeLayout()
        {
            // TODO: Load layout from project settings
            StatusMessage = $"Project {_projectMetadata.Name} loaded successfully";
        }

        private void OnSave()
        {
            // TODO: Implement save current scene/asset
            StatusMessage = "Saving...";
        }

        private void OnSaveAll()
        {
            // TODO: Implement save all changes
            StatusMessage = "Saving all changes...";
        }

        private void OnExit()
        {
            // TODO: Check for unsaved changes
            Close();
        }

        private void OnUndo()
        {
            // TODO: Implement undo
            StatusMessage = "Undo";
        }

        private void OnRedo()
        {
            // TODO: Implement redo
            StatusMessage = "Redo";
        }

        private void OnResetLayout()
        {
            // TODO: Reset dock layout to default
            StatusMessage = "Layout reset to default";
        }

        private void OnPlay()
        {
            // TODO: Implement play mode
            StatusMessage = "Playing...";
        }

        private void OnPause()
        {
            // TODO: Implement pause
            StatusMessage = "Paused";
        }

        private void OnStop()
        {
            // TODO: Implement stop
            StatusMessage = "Stopped";
        }

        private void OnViewportMouseWheel(object sender, PointerWheelEventArgs e)
        {
            _cameraController.OnMouseWheel(e);
        }

        private void OnViewportMouseDown(object sender, PointerPressedEventArgs e)
        {
            _cameraController.OnMouseDown(e);
        }

        private void OnViewportMouseUp(object sender, PointerReleasedEventArgs e)
        {
            _cameraController.OnMouseUp(e);
        }

        private void OnViewportMouseMove(object sender, PointerEventArgs e)
        {
            _cameraController.OnMouseMove(e);
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object? parameter) => _execute();

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
} 