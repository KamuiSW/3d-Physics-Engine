using System;
using System.IO;
using System.IO.Abstractions;

namespace GameEngine.Core.IO
{
    public class FileSystemService
    {
        private readonly IFileSystem _fileSystem;
        private readonly FileSystemWatcher _watcher;
        
        public event Action<string>? FileChanged;

        public FileSystemService(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _watcher = new FileSystemWatcher
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
            };
            _watcher.Changed += OnFileSystemChange;
            _watcher.Created += OnFileSystemChange;
            _watcher.Deleted += OnFileSystemChange;
            _watcher.Renamed += OnFileSystemChange;
        }

        public void WatchDirectory(string path)
        {
            if (!_fileSystem.Directory.Exists(path))
                throw new DirectoryNotFoundException($"Path not found: {path}");

            _watcher.Path = path;
            _watcher.EnableRaisingEvents = true;
        }

        private void OnFileSystemChange(object sender, FileSystemEventArgs e)
        {
            // Debounce rapid fire events
            FileChanged?.Invoke(e.FullPath);
        }

        public string ReadAllText(string path)
        {
            if (!_fileSystem.File.Exists(path))
                throw new FileNotFoundException("File not found", path);

            return _fileSystem.File.ReadAllText(path);
        }

        public void CreateDirectory(string path)
        {
            if (_fileSystem.Directory.Exists(path))
                return;

            _fileSystem.Directory.CreateDirectory(path);
        }

        // Additional file system operations as needed...
    }
} 