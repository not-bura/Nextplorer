using Nextplorer.src.Presentation;
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

            Width = 800.0;
            Height = 450.0;
            m_controller = WindowController.Create(this);
            var _view = m_controller.View;
            _view.Add(new Tab("Tab1"));
            _view.Add(new Tab("Tab2"));
        }

        ~MainWindow()
        {
            if (m_controller is not null)
            {
                m_controller.Dispose();
                m_controller = null;
            }
        }
    }
}
