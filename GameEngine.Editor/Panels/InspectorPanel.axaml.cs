using Avalonia.Controls;
using GameEngine.Core.Scene;
using GameEngine.Core.Scene.Components;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GameEngine.Editor.Panels
{
    public partial class InspectorPanel : UserControl
    {
        private GameObject? _selectedObject;

        public GameObject? SelectedObject
        {
            get => _selectedObject;
            set
            {
                _selectedObject = value;
                if (DataContext is InspectorPanelViewModel vm)
                {
                    vm.SelectedObject = value;
                }
            }
        }

        public InspectorPanel()
        {
            InitializeComponent();
            DataContext = new InspectorPanelViewModel();
        }
    }

    public class InspectorPanelViewModel : ViewModelBase
    {
        private GameObject? _selectedObject;
        private readonly ObservableCollection<ComponentEditor> _components = new();

        public GameObject? SelectedObject
        {
            get => _selectedObject;
            set
            {
                _selectedObject = value;
                UpdateComponentEditors();
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasSelectedObject));
            }
        }

        public bool HasSelectedObject => SelectedObject != null;
        public ObservableCollection<ComponentEditor> Components => _components;
        public ICommand AddComponentCommand { get; }

        public InspectorPanelViewModel()
        {
            AddComponentCommand = new RelayCommand(ShowAddComponentDialog);
        }

        private void UpdateComponentEditors()
        {
            _components.Clear();
            if (SelectedObject == null) return;

            foreach (var component in SelectedObject.Components)
            {
                var editor = CreateEditorForComponent(component);
                if (editor != null)
                {
                    _components.Add(editor);
                }
            }
        }

        private ComponentEditor? CreateEditorForComponent(Component component)
        {
            return component switch
            {
                Transform transform => new TransformEditor(transform),
                _ => new GenericComponentEditor(component)
            };
        }

        private async void ShowAddComponentDialog()
        {
            if (SelectedObject == null) return;

            // TODO: Show component selection dialog
            // For now, just add a test component
            var transform = SelectedObject.AddComponent<Transform>();
            var editor = CreateEditorForComponent(transform);
            if (editor != null)
            {
                _components.Add(editor);
            }
        }
    }

    public abstract class ComponentEditor
    {
        public string Header { get; }
        public Control Editor { get; }

        protected ComponentEditor(string header, Control editor)
        {
            Header = header;
            Editor = editor;
        }
    }

    public class TransformEditor : ComponentEditor
    {
        public TransformEditor(Transform transform) : base("Transform", CreateEditor(transform))
        {
        }

        private static Control CreateEditor(Transform transform)
        {
            return new Grid
            {
                RowDefinitions = new RowDefinitions("Auto,Auto,Auto"),
                ColumnDefinitions = new ColumnDefinitions("Auto,*"),
                Children =
                {
                    new TextBlock { Text = "Position", VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center },
                    new Vector3Editor { DataContext = transform.Position, Margin = new Avalonia.Thickness(5,0,0,0) },
                    new TextBlock { Text = "Rotation", VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center },
                    new Vector3Editor { DataContext = transform.Rotation, Margin = new Avalonia.Thickness(5,0,0,0) },
                    new TextBlock { Text = "Scale", VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center },
                    new Vector3Editor { DataContext = transform.Scale, Margin = new Avalonia.Thickness(5,0,0,0) }
                }
            };
        }
    }

    public class GenericComponentEditor : ComponentEditor
    {
        public GenericComponentEditor(Component component) 
            : base(component.GetType().Name, new TextBlock { Text = "No custom editor available" })
        {
        }
    }

    public class Vector3Editor : Grid
    {
        public Vector3Editor()
        {
            ColumnDefinitions = new ColumnDefinitions("*,*,*");
            Children.Add(new NumericUpDown { Increment = 0.1 });
            Children.Add(new NumericUpDown { Increment = 0.1 });
            Children.Add(new NumericUpDown { Increment = 0.1 });
        }
    }
} 