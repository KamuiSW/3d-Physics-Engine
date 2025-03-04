using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace GameEngine.Editor.ViewModels
{
    public class ProjectFilesViewModel : INotifyPropertyChanged
    {
        private string _rootPath = string.Empty;
        private readonly ObservableCollection<FileSystemNodeViewModel> _rootNodes = new();

        public ObservableCollection<FileSystemNodeViewModel> RootNodes => _rootNodes;

        public string RootPath
        {
            get => _rootPath;
            set
            {
                if (_rootPath != value)
                {
                    _rootPath = value;
                    OnPropertyChanged();
                    Refresh();
                }
            }
        }

        public void Refresh()
        {
            _rootNodes.Clear();
            if (Directory.Exists(RootPath))
            {
                _rootNodes.Add(new FileSystemNodeViewModel(RootPath));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 