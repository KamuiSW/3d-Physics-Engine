using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO;

namespace GameEngine.Editor.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly ProjectFilesViewModel _projectFiles;
        private readonly HierarchyViewModel _hierarchy;
        private readonly InspectorViewModel _inspector;

        public MainWindowViewModel()
        {
            _projectFiles = new ProjectFilesViewModel();
            _hierarchy = new HierarchyViewModel();
            _inspector = new InspectorViewModel();

            // Set the project root path to the current solution directory
            string currentDir = Directory.GetCurrentDirectory();
            // Navigate up until we find the solution directory
            while (!string.IsNullOrEmpty(currentDir) && !File.Exists(Path.Combine(currentDir, "3d-Physics-Engine.sln")))
            {
                currentDir = Path.GetDirectoryName(currentDir);
            }
            
            _projectFiles.RootPath = currentDir ?? Directory.GetCurrentDirectory();
            _projectFiles.Refresh(); // Explicitly refresh to load files
        }

        public ProjectFilesViewModel ProjectFiles => _projectFiles;
        public HierarchyViewModel Hierarchy => _hierarchy;
        public InspectorViewModel Inspector => _inspector;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 