using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace GameEngine.Editor.ViewModels
{
    public class FileSystemNodeViewModel : INotifyPropertyChanged
    {
        private readonly string _fullPath;
        private bool _isExpanded;

        public string Name => Path.GetFileName(_fullPath);
        public string FullPath => _fullPath;
        public bool IsDirectory => Directory.Exists(_fullPath);
        public ObservableCollection<FileSystemNodeViewModel> Children { get; } = new();
        public string Icon => IsDirectory ? "resm:GameEngine.Editor.Assets.folder.png" 
                                       : "resm:GameEngine.Editor.Assets.file.png";

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged();
                    if (IsExpanded && IsDirectory)
                        LoadChildren();
                }
            }
        }

        public FileSystemNodeViewModel(string path)
        {
            _fullPath = path;
            if (IsDirectory)
                Children.Add(new FileSystemNodeViewModel("dummy")); // Placeholder
        }

        private void LoadChildren()
        {
            Children.Clear();
            try
            {
                foreach (var dir in Directory.GetDirectories(_fullPath))
                {
                    Children.Add(new FileSystemNodeViewModel(dir));
                }
                foreach (var file in Directory.GetFiles(_fullPath))
                {
                    Children.Add(new FileSystemNodeViewModel(file));
                }
            }
            catch { /* Handle access errors */ }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 