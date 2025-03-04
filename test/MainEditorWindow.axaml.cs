using Avalonia.Controls;
using GameEngine.Core.Commands;
using GameEngine.Core.Project;
using GameEngine.Core.ViewModels;
using System;
using System.Windows.Input;

namespace GameEngine.Editor
{
    public partial class MainEditorWindow : Window
    {
        private readonly ProjectMetadata? _projectMetadata;
        private string _statusMessage = "Ready";

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                if (DataContext is MainEditorViewModel vm)
                {
                    vm.StatusMessage = value;
                }
            }
        }

        public MainEditorWindow()
        {
            InitializeComponent();
            DataContext = new MainEditorViewModel(this);
        }

        public MainEditorWindow(ProjectMetadata projectMetadata) : this()
        {
            _projectMetadata = projectMetadata;
            Title = $"Game Engine Editor - {_projectMetadata.Name}";
            StatusMessage = $"Project {_projectMetadata.Name} loaded successfully";
        }
    }

    public class MainEditorViewModel : ViewModelBase
    {
        private readonly MainEditorWindow _window;
        private string _statusMessage = "Ready";

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetField(ref _statusMessage, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand SaveAllCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }
        public ICommand ResetLayoutCommand { get; }

        public MainEditorViewModel(MainEditorWindow window)
        {
            _window = window;
            
            SaveCommand = new RelayCommand(OnSave);
            SaveAllCommand = new RelayCommand(OnSaveAll);
            ExitCommand = new RelayCommand(OnExit);
            UndoCommand = new RelayCommand(OnUndo);
            RedoCommand = new RelayCommand(OnRedo);
            ResetLayoutCommand = new RelayCommand(OnResetLayout);
        }

        private void OnSave()
        {
            StatusMessage = "Saving...";
            // TODO: Implement save
        }

        private void OnSaveAll()
        {
            StatusMessage = "Saving all...";
            // TODO: Implement save all
        }

        private void OnExit()
        {
            _window.Close();
        }

        private void OnUndo()
        {
            StatusMessage = "Undo";
            // TODO: Implement undo
        }

        private void OnRedo()
        {
            StatusMessage = "Redo";
            // TODO: Implement redo
        }

        private void OnResetLayout()
        {
            StatusMessage = "Resetting layout...";
            // TODO: Implement layout reset
        }
    }
} 