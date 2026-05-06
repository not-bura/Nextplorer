using System.Windows.Threading;

namespace Nextplorer.Domain
{
    public sealed class TimerEvent<T>
        : IDisposable
    {
        private DispatcherTimer m_timer;
        private Action<object, T> m_handler;

        private object m_sender;
        private T m_arguments;

        public TimerEvent(double interval, Action<object, T> handler)
        {
            var _timer = m_timer = new(DispatcherPriority.Background)
            {
                Interval = TimeSpan.FromSeconds(interval),
            };
            _timer.Stop();

            _timer.Tick += (_, _) =>
            {
                if (m_sender is not null && m_arguments is not null)
                {
                    m_handler!(m_sender, m_arguments);
                }

                m_timer.Stop();
            };

            m_handler = handler;

            m_sender = null!;
            m_arguments = default!;
        }

        public void Dispose()
        {
            if (m_timer is not null)
            {
                m_timer.Stop();
                m_timer = null!;
            }

            if (m_handler is not null)
            {
                m_handler = null!;
            }

            if (m_sender is not null)
            {
                m_sender = null!;
            }

            if (m_arguments is not null)
            {
                m_arguments = default!;
            }
        }
        
        public void Handle(object sender, T args)
        {
            m_sender = sender;
            m_arguments = args;

            m_timer.Stop();
            m_timer.Start();
        }
    }
}
