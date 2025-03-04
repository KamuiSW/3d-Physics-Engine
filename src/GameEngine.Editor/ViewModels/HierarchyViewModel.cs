using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GameEngine.Core.Scene;

namespace GameEngine.Editor.ViewModels
{
    public class HierarchyViewModel : INotifyPropertyChanged
    {
        private SceneObject? _selectedObject;
        private Scene? _currentScene;
        private ObservableCollection<SceneObjectViewModel> _sceneObjects = new();

        public ObservableCollection<SceneObjectViewModel> SceneObjects => _sceneObjects;

        public SceneObject? SelectedObject
        {
            get => _selectedObject;
            set
            {
                if (_selectedObject != value)
                {
                    _selectedObject = value;
                    OnPropertyChanged();
                }
            }
        }

        public Scene? CurrentScene
        {
            get => _currentScene;
            set
            {
                if (_currentScene != value)
                {
                    _currentScene = value;
                    OnPropertyChanged();
                    RefreshSceneObjects();
                }
            }
        }

        private void RefreshSceneObjects()
        {
            _sceneObjects.Clear();
            if (_currentScene != null)
            {
                foreach (var obj in _currentScene.RootObjects)
                {
                    _sceneObjects.Add(CreateViewModel(obj));
                }
            }
        }

        private SceneObjectViewModel CreateViewModel(SceneObject sceneObject)
        {
            var vm = new SceneObjectViewModel(sceneObject);
            foreach (var child in sceneObject.Children)
            {
                vm.Children.Add(CreateViewModel(child));
            }
            return vm;
        }

        public HierarchyViewModel()
        {
            // Create actual SceneObjects for testing
            var scene = new Scene();
            
            var root = new SceneObject("Root");
            var child1 = new SceneObject("Child 1");
            var child2 = new SceneObject("Child 2");
            var grandchild = new SceneObject("Grandchild");
            
            child1.AddChild(grandchild);
            root.AddChild(child1);
            root.AddChild(child2);
            
            scene.AddRootObject(root);
            CurrentScene = scene;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class SceneObjectViewModel : INotifyPropertyChanged
    {
        private readonly SceneObject _sceneObject;
        private readonly ObservableCollection<SceneObjectViewModel> _children = new();

        public SceneObjectViewModel(SceneObject sceneObject)
        {
            _sceneObject = sceneObject;
            _sceneObject.PropertyChanged += (s, e) => OnPropertyChanged(e.PropertyName);
        }

        public SceneObject SceneObject => _sceneObject;

        public string Name
        {
            get => _sceneObject.Name;
            set => _sceneObject.Name = value;
        }

        public bool IsActive
        {
            get => _sceneObject.IsActive;
            set => _sceneObject.IsActive = value;
        }

        public ObservableCollection<SceneObjectViewModel> Children => _children;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 