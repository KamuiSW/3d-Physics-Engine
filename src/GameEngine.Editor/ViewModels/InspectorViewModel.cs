using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GameEngine.Core.Scene;

namespace GameEngine.Editor.ViewModels
{
    public class InspectorViewModel : INotifyPropertyChanged
    {
        private SceneObjectViewModel? _selectedObject;
        private TransformViewModel? _transformViewModel;
        private readonly ObservableCollection<ComponentViewModel> _components = new();

        public SceneObjectViewModel? SelectedObject
        {
            get => _selectedObject;
            set
            {
                if (_selectedObject != value)
                {
                    _selectedObject = value;
                    OnPropertyChanged();
                    UpdateComponents();
                }
            }
        }

        public TransformViewModel? TransformViewModel
        {
            get => _transformViewModel;
            private set
            {
                if (_transformViewModel != value)
                {
                    _transformViewModel = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<ComponentViewModel> Components => _components;

        private void UpdateComponents()
        {
            _components.Clear();
            TransformViewModel = null;

            if (_selectedObject != null)
            {
                // Create Transform view model
                var transform = _selectedObject.SceneObject.Transform;
                TransformViewModel = new TransformViewModel(transform);

                // Add other components
                foreach (var component in _selectedObject.SceneObject.Components)
                {
                    if (component is not Transform) // Skip Transform as it's handled separately
                    {
                        _components.Add(CreateComponentViewModel(component));
                    }
                }
            }
        }

        private ComponentViewModel CreateComponentViewModel(GameEngine.Core.Scene.Component component)
        {
            // For now, return a basic component view model
            // Later, we can add specific view models for different component types
            return new ComponentViewModel(component);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ComponentViewModel : INotifyPropertyChanged
    {
        protected readonly GameEngine.Core.Scene.Component _component;

        public ComponentViewModel(GameEngine.Core.Scene.Component component)
        {
            _component = component;
            _component.PropertyChanged += (s, e) => OnPropertyChanged(e.PropertyName);
        }

        public string Name => _component.GetType().Name;

        public bool Enabled
        {
            get => _component.Enabled;
            set => _component.Enabled = value;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 