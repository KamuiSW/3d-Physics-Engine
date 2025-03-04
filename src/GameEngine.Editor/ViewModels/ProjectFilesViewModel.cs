using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Avalonia.Threading;
using GameEngine.Editor.Commands;

namespace GameEngine.Editor.ViewModels
{
    public class ProjectFilesViewModel : INotifyPropertyChanged
    {
        private string _rootPath = string.Empty;
        private readonly ObservableCollection<FileSystemNodeViewModel> _rootNodes = new();
        private FileSystemWatcher? _watcher;

        public ObservableCollection<FileSystemNodeViewModel> RootNodes => _rootNodes;

        public ICommand RefreshCommand { get; }
        public ICommand CollapseAllCommand { get; }

        public ProjectFilesViewModel()
        {
            RefreshCommand = new RelayCommand(_ => Refresh());
            CollapseAllCommand = new RelayCommand(_ => CollapseAll());
        }

        public string RootPath
        {
            get => _rootPath;
            set
            {
                if (_rootPath != value)
                {
                    _rootPath = value;
                    OnPropertyChanged();
                    InitializeFileWatcher();
                    Refresh();
                }
            }
        }

        private void InitializeFileWatcher()
        {
            _watcher?.Dispose();
            if (Directory.Exists(_rootPath))
            {
                _watcher = new FileSystemWatcher(_rootPath)
                {
                    IncludeSubdirectories = true,
                    EnableRaisingEvents = true,
                    NotifyFilter = NotifyFilters.DirectoryName 
                                | NotifyFilters.FileName 
                                | NotifyFilters.LastWrite
                };

                _watcher.Created += OnFileSystemChanged;
                _watcher.Deleted += OnFileSystemChanged;
                _watcher.Renamed += OnFileSystemChanged;
                _watcher.Changed += OnFileSystemChanged;
            }
        }

        private void OnFileSystemChanged(object sender, FileSystemEventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync(Refresh);
        }

        public void Refresh()
        {
            _rootNodes.Clear();
            if (Directory.Exists(_rootPath))
            {
                var rootNode = new FileSystemNodeViewModel(_rootPath);
                rootNode.IsExpanded = true;
                _rootNodes.Add(rootNode);
            }
        }

        private void CollapseAll()
        {
            foreach (var node in _rootNodes)
            {
                CollapseNode(node);
            }
        }

        private void CollapseNode(FileSystemNodeViewModel node)
        {
            node.IsExpanded = false;
            foreach (var child in node.Children)
            {
                CollapseNode(child);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 