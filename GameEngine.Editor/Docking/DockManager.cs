using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using System.Collections.ObjectModel;

namespace GameEngine.Editor.Docking
{
    public class DockManager : Grid
    {
        public static readonly StyledProperty<ObservableCollection<DockPanel>> LeftDockProperty =
            AvaloniaProperty.Register<DockManager, ObservableCollection<DockPanel>>(
                nameof(LeftDock), new ObservableCollection<DockPanel>());

        public static readonly StyledProperty<ObservableCollection<DockPanel>> RightDockProperty =
            AvaloniaProperty.Register<DockManager, ObservableCollection<DockPanel>>(
                nameof(RightDock), new ObservableCollection<DockPanel>());

        public static readonly StyledProperty<ObservableCollection<DockPanel>> BottomDockProperty =
            AvaloniaProperty.Register<DockManager, ObservableCollection<DockPanel>>(
                nameof(BottomDock), new ObservableCollection<DockPanel>());

        public ObservableCollection<DockPanel> LeftDock
        {
            get => GetValue(LeftDockProperty);
            set => SetValue(LeftDockProperty, value);
        }

        public ObservableCollection<DockPanel> RightDock
        {
            get => GetValue(RightDockProperty);
            set => SetValue(RightDockProperty, value);
        }

        public ObservableCollection<DockPanel> BottomDock
        {
            get => GetValue(BottomDockProperty);
            set => SetValue(BottomDockProperty, value);
        }

        private Grid _mainGrid;
        private TabControl _centerContent;
        private Grid _leftDock;
        private Grid _rightDock;
        private Grid _bottomDock;

        public DockManager()
        {
            InitializeLayout();
            SetupCollectionHandlers();
        }

        private void InitializeLayout()
        {
            _mainGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Auto }, // Left dock
                    new ColumnDefinition { Width = GridLength.Star }, // Center
                    new ColumnDefinition { Width = GridLength.Auto }  // Right dock
                },
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Star },
                    new RowDefinition { Height = GridLength.Auto }    // Bottom dock
                }
            };

            _leftDock = new Grid();
            _rightDock = new Grid();
            _bottomDock = new Grid();
            _centerContent = new TabControl();

            Grid.SetColumn(_leftDock, 0);
            Grid.SetColumn(_centerContent, 1);
            Grid.SetColumn(_rightDock, 2);
            Grid.SetRow(_bottomDock, 1);
            Grid.SetColumnSpan(_bottomDock, 3);

            _mainGrid.Children.Add(_leftDock);
            _mainGrid.Children.Add(_centerContent);
            _mainGrid.Children.Add(_rightDock);
            _mainGrid.Children.Add(_bottomDock);

            Children.Add(_mainGrid);
        }

        private void SetupCollectionHandlers()
        {
            LeftDock.CollectionChanged += (s, e) => UpdateDockPanels(_leftDock, LeftDock);
            RightDock.CollectionChanged += (s, e) => UpdateDockPanels(_rightDock, RightDock);
            BottomDock.CollectionChanged += (s, e) => UpdateDockPanels(_bottomDock, BottomDock);
        }

        private void UpdateDockPanels(Grid dockGrid, ObservableCollection<DockPanel> panels)
        {
            dockGrid.Children.Clear();
            foreach (var panel in panels)
            {
                dockGrid.Children.Add(panel);
            }
        }

        public void AddCenterPanel(DockPanel panel)
        {
            var tab = new TabItem
            {
                Header = panel.Title,
                Content = panel
            };
            _centerContent.Items.Add(tab);
        }
    }
} 