using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Avalonia.Threading;

namespace GameEngine.Editor.ViewModels
{
    public class SceneViewModel : INotifyPropertyChanged
    {
        private string _statusMessage = "Ready";
        private bool _isGridVisible = true;

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                if (_statusMessage != value)
                {
                    _statusMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsGridVisible
        {
            get => _isGridVisible;
            set
            {
                if (_isGridVisible != value)
                {
                    _isGridVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        // Commands
        public ICommand ToggleGridCommand { get; }
        public ICommand ResetViewCommand { get; }

        public SceneViewModel()
        {
            // Initialize commands
            ToggleGridCommand = new RelayCommand(_ => 
            {
                IsGridVisible = !IsGridVisible;
                StatusMessage = IsGridVisible ? "Grid visible" : "Grid hidden";
            });

            ResetViewCommand = new RelayCommand(_ => 
            {
                StatusMessage = "View reset";
                // Reset camera position logic would go here
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Simple ICommand implementation
    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute(parameter);

        public void Execute(object? parameter) => _execute(parameter);

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
} 