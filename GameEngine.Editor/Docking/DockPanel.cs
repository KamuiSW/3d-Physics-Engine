using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace GameEngine.Editor.Docking
{
    public class DockPanel : UserControl
    {
        public static readonly StyledProperty<string> TitleProperty =
            AvaloniaProperty.Register<DockPanel, string>(nameof(Title));

        public static readonly StyledProperty<bool> IsFloatingProperty =
            AvaloniaProperty.Register<DockPanel, bool>(nameof(IsFloating));

        public string Title
        {
            get => GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public bool IsFloating
        {
            get => GetValue(IsFloatingProperty);
            set => SetValue(IsFloatingProperty, value);
        }

        private Grid _mainGrid;
        private Border _headerBorder;
        private TextBlock _titleBlock;
        private Button _floatButton;
        private ContentControl _contentContainer;

        public DockPanel()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _mainGrid = new Grid
            {
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Star }
                }
            };

            // Header
            _headerBorder = new Border
            {
                Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.LightGray),
                Height = 25,
                Child = new DockPanel
                {
                    Children =
                    {
                        new TextBlock
                        {
                            [!TextBlock.TextProperty] = this[!TitleProperty],
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(5, 0)
                        },
                        new Button
                        {
                            Content = "⇱",
                            Width = 25,
                            Height = 25,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            [!Button.IsVisibleProperty] = this[!IsFloatingProperty].Not()
                        }
                    }
                }
            };

            // Content
            _contentContainer = new ContentControl
            {
                [!ContentControl.ContentProperty] = this[!ContentProperty]
            };

            Grid.SetRow(_headerBorder, 0);
            Grid.SetRow(_contentContainer, 1);

            _mainGrid.Children.Add(_headerBorder);
            _mainGrid.Children.Add(_contentContainer);

            Content = _mainGrid;
        }

        public void Float()
        {
            if (!IsFloating)
            {
                var window = new Window
                {
                    Title = Title,
                    Content = this,
                    Width = 300,
                    Height = 400
                };
                IsFloating = true;
                window.Show();
            }
        }

        public void Dock()
        {
            IsFloating = false;
            // TODO: Implement docking logic
        }
    }
} 