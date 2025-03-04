using Avalonia.Controls;
using Avalonia.Media.Imaging;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GameEngine.Editor.Panels
{
    public partial class AssetBrowserPanel : UserControl
    {
        private string? _projectPath;

        public string? ProjectPath
        {
            get => _projectPath;
            set
            {
                _projectPath = value;
                if (DataContext is AssetBrowserViewModel vm)
                {
                    vm.ProjectPath = value;
                    vm.RefreshCommand.Execute(null);
                }
            }
        }

        public AssetBrowserPanel()
        {
            InitializeComponent();
            DataContext = new AssetBrowserViewModel();
        }
    }

    public class AssetBrowserViewModel : ViewModelBase
    {
        private string? _projectPath;
        private AssetFolder? _selectedFolder;
        private AssetItem? _selectedAsset;
        private readonly ObservableCollection<AssetFolder> _folderStructure = new();
        private readonly ObservableCollection<AssetItem> _assets = new();

        public string? ProjectPath
        {
            get => _projectPath;
            set
            {
                _projectPath = value;
                OnPropertyChanged();
            }
        }

        public AssetFolder? SelectedFolder
        {
            get => _selectedFolder;
            set
            {
                _selectedFolder = value;
                LoadAssetsForFolder(value);
                OnPropertyChanged();
            }
        }

        public AssetItem? SelectedAsset
        {
            get => _selectedAsset;
            set
            {
                _selectedAsset = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<AssetFolder> FolderStructure => _folderStructure;
        public ObservableCollection<AssetItem> Assets => _assets;

        public ICommand CreateFolderCommand { get; }
        public ICommand ImportAssetsCommand { get; }
        public ICommand RefreshCommand { get; }

        public AssetBrowserViewModel()
        {
            CreateFolderCommand = new RelayCommand(CreateFolder, () => SelectedFolder != null);
            ImportAssetsCommand = new RelayCommand(ImportAssets, () => SelectedFolder != null);
            RefreshCommand = new RelayCommand(Refresh);
        }

        private async void Refresh()
        {
            if (string.IsNullOrEmpty(ProjectPath)) return;

            _folderStructure.Clear();
            await LoadFolderStructure(ProjectPath);
        }

        private async Task LoadFolderStructure(string path)
        {
            var assetsPath = Path.Combine(path, "Assets");
            if (!Directory.Exists(assetsPath))
            {
                Directory.CreateDirectory(assetsPath);
            }

            var rootFolder = new AssetFolder { Name = "Assets", Path = assetsPath };
            await LoadSubFolders(rootFolder);
            _folderStructure.Add(rootFolder);
        }

        private async Task LoadSubFolders(AssetFolder folder)
        {
            try
            {
                var directories = Directory.GetDirectories(folder.Path);
                foreach (var dir in directories)
                {
                    var subFolder = new AssetFolder
                    {
                        Name = Path.GetFileName(dir),
                        Path = dir,
                        Parent = folder
                    };
                    folder.Children.Add(subFolder);
                    await LoadSubFolders(subFolder);
                }
            }
            catch (Exception ex)
            {
                // TODO: Log error
            }
        }

        private async void LoadAssetsForFolder(AssetFolder? folder)
        {
            _assets.Clear();
            if (folder == null) return;

            try
            {
                var files = Directory.GetFiles(folder.Path);
                foreach (var file in files)
                {
                    var asset = await CreateAssetItem(file);
                    if (asset != null)
                    {
                        _assets.Add(asset);
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO: Log error
            }
        }

        private async Task<AssetItem?> CreateAssetItem(string path)
        {
            var extension = Path.GetExtension(path).ToLower();
            var name = Path.GetFileName(path);

            // TODO: Create proper asset type detection and preview generation
            return new AssetItem
            {
                Name = name,
                Path = path,
                Type = extension,
                Preview = null // TODO: Generate preview
            };
        }

        private async void CreateFolder()
        {
            if (SelectedFolder == null) return;

            var dialog = new TextBox
            {
                Watermark = "Folder Name",
                Text = "New Folder"
            };

            // TODO: Show dialog and create folder
            var folderName = dialog.Text;
            var path = Path.Combine(SelectedFolder.Path, folderName);
            
            try
            {
                Directory.CreateDirectory(path);
                var newFolder = new AssetFolder
                {
                    Name = folderName,
                    Path = path,
                    Parent = SelectedFolder
                };
                SelectedFolder.Children.Add(newFolder);
            }
            catch (Exception ex)
            {
                // TODO: Show error dialog
            }
        }

        private async void ImportAssets()
        {
            if (SelectedFolder == null) return;

            var dialog = new OpenFileDialog
            {
                AllowMultiple = true,
                Title = "Import Assets"
            };

            try
            {
                var result = await dialog.ShowAsync(null);
                if (result != null)
                {
                    foreach (var file in result)
                    {
                        var destPath = Path.Combine(SelectedFolder.Path, Path.GetFileName(file));
                        File.Copy(file, destPath, true);
                    }
                    LoadAssetsForFolder(SelectedFolder);
                }
            }
            catch (Exception ex)
            {
                // TODO: Show error dialog
            }
        }
    }

    public class AssetFolder : ViewModelBase
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public AssetFolder? Parent { get; set; }
        public ObservableCollection<AssetFolder> Children { get; } = new();
    }

    public class AssetItem : ViewModelBase
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public Bitmap? Preview { get; set; }
    }
} 