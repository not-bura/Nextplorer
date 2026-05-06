using Nextplorer.Domain;
using Nextplorer.Presentation;
using System.IO;
using System.Windows;

namespace Nextplorer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
        : Window
    {
        private WindowController m_controller;

        public MainWindow()
        {
            InitializeComponent();

            //var _resolver = CompositeResolver.Create(
            //    GeneratedMessagePackResolver.Instance,
            //    StandardResolver.Instance
            //);

            //MessagePackSerializer.DefaultOptions = MessagePackSerializer.DefaultOptions
            //    .WithResolver(_resolver);

            var _config = Registory.Config;
            Width = _config.Size.X;
            Height = _config.Size.Y;

            TimerEvent<SizeChangedEventArgs> m_event = new(1.0, (_, e) =>
            {
                var _size = e.NewSize;

                Width = _size.Width;
                Height = _size.Height;

                var _config = Registory.Config;
                _config.Size = new(_size.Width, _size.Height);
                _config.Save();

            });
            SizeChanged += m_event.Handle;

            m_controller = WindowController.Create(this);
            var _view = m_controller.View;
            _view.Add(new Tab("Tab1"));
            _view.Add(new Tab("Tab2"));

            ColorPickerWindow.Show(this, v =>
            {
            });
        }

        private Temporary LoadTemporary(DirectoryInfo info)
        {
            var _name = "temp";

            var _path = Path.Combine(info.FullName, _name);
            if (Directory.Exists(_path))
            {
                return Temporary.Default();
            }

            Directory.CreateDirectory(_path);
            return Temporary.Default();
        }

        ~MainWindow()
        {
            if (m_controller is not null)
            {
                m_controller.Dispose();
                m_controller = null!;
            }
        }
    }
}
