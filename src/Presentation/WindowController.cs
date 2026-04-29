using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shell;

namespace Nextplorer.src.Presentation
{
    public static class ThemeModel
    {
        private static Palette m_current = Palette.Dark;
        public static Palette Current
        {
            get => m_current;
            set
            {
                m_current = value;
                Apply(value);
            }
        }

        public static void Apply(Palette palette)
        {
            WindowHeader.Color = palette.WindowHeader;
            WindowContent.Color = palette.WindowContent;
            Text.Color = palette.Text;
        }

        public static SolidColorBrush WindowHeader { get; private set; }
        public static SolidColorBrush WindowContent { get; private set; }
        public static SolidColorBrush Text { get; private set; }

        static ThemeModel()
        {
            WindowHeader = new SolidColorBrush(Color.FromRgb(0x1E, 0x1E, 0x1E));
            WindowContent = new SolidColorBrush(Color.FromRgb(0x17, 0x17, 0x17));
            Text = new SolidColorBrush(Colors.WhiteSmoke);
        }

        public sealed class Palette
        {
            public Color WindowHeader { get; private set; }
            public Color WindowContent { get; private set; }
            public Color Text { get; private set; }

            public static Palette Dark = new()
            {
                WindowHeader = Color.FromRgb(0x1E, 0x1E, 0x1E),
                WindowContent = Color.FromRgb(0x17, 0x17, 0x17),
                Text = Colors.WhiteSmoke,
            };

            public static Palette Light = new()
            {
                WindowHeader = Color.FromRgb(0xF0, 0xF0, 0xF0),
                WindowContent = Color.FromRgb(0xFF, 0xFF, 0xFF),
                Text = Colors.Black,
            };
        }
    }

    public sealed class View
    {
        private List<Tab> m_tabs;
        private TabControl m_control;

        public int Count => m_tabs.Count;

        public TabControl Control => m_control;

        public View(TabControl control)
        {
            m_tabs = new();
            m_control = control;
        }

        public void Add(Tab target)
        {
            m_tabs.Add(target);
            m_control.Items.Add(target.Item);
        }

        public void Remove(Tab target)
        {
            m_control.Items.Remove(target.Item);
            m_tabs.Remove(target);
        }
    }

    public sealed class Tab
    {
        private string m_current;
        private TabItem m_item;

        public string Header => m_current;
        public TabItem Item => m_item;

        public Tab(string header)
        {
            m_current = header;
            m_item = new()
            {
                Header = header,
                DataContext = this,
            };
        }
    }

    public sealed class WindowController
        : IDisposable
    {
        private static List<WindowController> s_instances = new();

        private Window m_instance;
        private WindowChrome m_chrome;
        private View m_view;

        public View View => m_view;

        private static FontFamily s_windowFontFamiry = new("Marlett");

        public static WindowController Create(MainWindow main)
        {
            var _result = new WindowController(main);
            s_instances.Add(_result);
            return _result;
        }

        public static WindowController Float(WindowController source)
        {
            var _window = new Window()
            {
                Title = "Nextplorer",
                Width = source.m_instance.Width,
                Height = source.m_instance.Height,
                MinWidth = 300.0,
                MinHeight = 240.0,
            };
            var _result = new WindowController(_window);
            s_instances.Add(_result);
            return _result;
        }

        private WindowController(Window target)
        {
            m_instance = target;

            m_chrome = new()
            {
                CaptionHeight = 40.0,
                ResizeBorderThickness = new(7.0),
                GlassFrameThickness = new(1.0),
            };
            WindowChrome.SetWindowChrome(target, m_chrome);

            var _control = new TabControl();
            _control.PreviewMouseLeftButtonDown += OnClickTab;
            m_view = new(_control);

            var _windowTitle = new TextBlock()
            {
                Text = "Nextplorer",
                Foreground = ThemeModel.Text,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new(10.0),
            };
            var _windowButtonPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Children =
                {
                    CreateButton("0", OnClickMinimize),
                    CreateButton("1", OnClickMaximize),
                    CreateButton("r", OnClickClose),
                },
            };
            WindowChrome.SetIsHitTestVisibleInChrome(_windowButtonPanel, true);

            var _windowHeader = new Border()
            {
                Child = new Grid()
                {
                    Background = ThemeModel.WindowHeader,
                    Children =
                    {
                        _windowTitle,
                        _windowButtonPanel,
                    },
                },
            };
            Grid.SetRow(_windowHeader, 0);

            var _contentTree = new Border()
            {
                Child = new TreeView()
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = Brushes.Transparent,
                },
            };
            Grid.SetColumn(_contentTree, 0);
            var _contentTab = new Border()
            {
                Child = m_view.Control,
            };
            Grid.SetColumn(_contentTab, 1);

            var _windowContent = new Border()
            {
                Child = new Grid()
                {
                    Background = ThemeModel.WindowContent,
                    ColumnDefinitions =
                    {
                        new() { Width = new(200.0), },
                        new() { Width = new(1.0, GridUnitType.Star), },
                    },
                    Children =
                    {
                        _contentTree,
                        _contentTab,
                    },
                },
            };
            Grid.SetRow(_windowContent, 1);

            var _windowGrid = new Grid()
            {
                RowDefinitions =
                {
                    new() { Height = new(40.0), },
                    new() { Height = new(100.0, GridUnitType.Star), },
                },
                Children =
                {
                    _windowHeader,
                    _windowContent,
                },
            };

            target.Content = _windowGrid;
        }

        ~WindowController()
        {
            s_instances.Remove(this);
        }

        public void Dispose()
        {
            s_instances.Remove(this);
            m_instance.Close();
            m_chrome = null;
            m_view = null;
        }

        #region window handler

        private void OnClickMinimize(object sender, MouseButtonEventArgs e)
        {
            m_instance.WindowState = WindowState.Minimized;
        }

        private void OnClickMaximize(object sender, MouseButtonEventArgs e)
        {
            m_instance.WindowState = (m_instance.WindowState == WindowState.Maximized)
                ? WindowState.Normal
                : WindowState.Maximized;
        }

        private void OnClickClose(object sender, MouseButtonEventArgs e)
        {
            m_instance.Close();
        }

        #endregion window handler

        #region tab handler

        private void OnClickTab(object sender, MouseButtonEventArgs e)
        {
            if (sender is not FrameworkElement _cast)
            {
                return;
            }

            var _control = m_view.Control;
            var _item = (TabItem)_control.SelectedItem;

            var _target = (_item.DataContext as Tab)!;

            m_view.Remove(_target);

            var _controller = Float(this);
            _controller.View.Add(_target);

            var _point = m_instance.PointToScreen(e.GetPosition(m_instance));
            _controller.m_instance.Left = _point.X;
            _controller.m_instance.Top = _point.Y;
            _controller.m_instance.Show();
            _controller.m_instance.DragMove();

            if (m_view.Count == 0)
            {
                Dispose();
            }
        }

        #endregion tab handler

        #region tree handler

        private static void OnExpanded(object sender, RoutedEventArgs e)
        {
            var _item = (TreeViewItem)sender;

            _item.Items.Clear();
            var _path = (string)_item.Tag;
            if (Directory.Exists(_path))
            {
                string[] _contents = null;

                try
                {
                    _contents = Directory.GetDirectories(_path);
                }
                catch
                {
                }

                if (_contents is null or { Length: 0 })
                {
                    return;
                }

                var _brush = ThemeModel.Text;
                foreach (var _content in _contents)
                {
                    var _subitem = new TreeViewItem()
                    {
                        Foreground = _brush,
                        Header = Path.GetFileName(_content),
                        Tag = _content,
                    };
                    _subitem.Expanded += OnExpanded;
                    _item.Items.Add(_subitem);
                }
            }
        }

        #endregion tree handler

        private static Button CreateButton(string content, MouseButtonEventHandler callback)
        {
            var _result = new Button()
            {
                Content = content,
                FontFamily = s_windowFontFamiry,
                Width = 46.0,
                Background = Brushes.Transparent,
                Foreground = ThemeModel.Text,
                BorderThickness = new(0.0),
            };

            _result.PreviewMouseLeftButtonUp += callback;

            return _result;
        }
    }
}
