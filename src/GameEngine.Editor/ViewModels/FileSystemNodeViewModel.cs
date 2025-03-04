using System;
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
        private readonly ObservableCollection<FileSystemNodeViewModel> _children = new();
        private bool _isLoaded;

        public string Name => Path.GetFileName(_fullPath) ?? _fullPath;
        public string FullPath => _fullPath;
        public bool IsDirectory => Directory.Exists(_fullPath);
        public ObservableCollection<FileSystemNodeViewModel> Children => _children;
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
                    if (_isExpanded && !_isLoaded)
                    {
                        LoadChildren();
                    }
                }
            }
        }

        public FileSystemNodeViewModel(string path)
        {
            _fullPath = path;
            if (IsDirectory && !_isLoaded)
            {
                // Add dummy node to show expand arrow
                _children.Add(new FileSystemNodeViewModel(Path.Combine(path, "dummy")));
            }
        }

        private void LoadChildren()
        {
            if (!IsDirectory || _isLoaded) return;

            _children.Clear();
            try
            {
                // Add directories first
                foreach (var dir in Directory.GetDirectories(_fullPath))
                {
                    _children.Add(new FileSystemNodeViewModel(dir));
                }

                // Then add files
                foreach (var file in Directory.GetFiles(_fullPath))
                {
                    _children.Add(new FileSystemNodeViewModel(file));
                }

                _isLoaded = true;
            }
            catch (Exception ex)
            {
                // Handle access denied or other IO exceptions
                System.Diagnostics.Debug.WriteLine($"Error loading directory {_fullPath}: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 