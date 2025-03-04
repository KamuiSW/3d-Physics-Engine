using Avalonia.Controls;
using GameEngine.Core.Scene;
using System;
using System.Windows.Input;

namespace GameEngine.Editor.Panels
{
    public partial class HierarchyPanel : UserControl
    {
        private Scene? _scene;
        private GameObject? _selectedGameObject;

        public event EventHandler<GameObject?>? SelectionChanged;

        public Scene? Scene
        {
            get => _scene;
            set
            {
                _scene = value;
                if (DataContext is HierarchyPanelViewModel vm)
                {
                    vm.Scene = value;
                }
            }
        }

        public GameObject? SelectedGameObject
        {
            get => _selectedGameObject;
            private set
            {
                _selectedGameObject = value;
                SelectionChanged?.Invoke(this, value);
            }
        }

        public HierarchyPanel()
        {
            InitializeComponent();
            DataContext = new HierarchyPanelViewModel(this);
        }
    }

    public class HierarchyPanelViewModel
    {
        private readonly HierarchyPanel _panel;

        public Scene? Scene { get; set; }
        public GameObject? SelectedGameObject
        {
            get => _panel.SelectedGameObject;
            set => _panel.SelectionChanged?.Invoke(_panel, value);
        }

        public ICommand CreateGameObjectCommand { get; }
        public ICommand DeleteSelectedCommand { get; }

        public HierarchyPanelViewModel(HierarchyPanel panel)
        {
            _panel = panel;
            CreateGameObjectCommand = new RelayCommand(CreateGameObject, () => Scene != null);
            DeleteSelectedCommand = new RelayCommand(DeleteSelected, () => SelectedGameObject != null);
        }

        private void CreateGameObject()
        {
            if (Scene == null) return;

            var parent = SelectedGameObject;
            var newObject = Scene.CreateGameObject("New GameObject", parent);
            SelectedGameObject = newObject;
        }

        private void DeleteSelected()
        {
            if (Scene == null || SelectedGameObject == null) return;

            Scene.DestroyGameObject(SelectedGameObject);
            SelectedGameObject = null;
        }
    }
} 