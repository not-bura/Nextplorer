namespace Nextplorer.Domain
{
    public sealed class ReactiveList<T>
        : IDisposable
    {
        private List<T> m_values;
        private event Action<T> m_addCallback;
        private event Action<T> m_removeCallback;

        public ReactiveList()
        {
            m_values = [];
            m_addCallback = null!;
            m_removeCallback = null!;
        }

        public ReactiveList(T source)
        {
            m_values = [source];
            m_addCallback = null!;
            m_removeCallback = null!;
        }

        public ReactiveList(ReadOnlySpan<T> source)
        {
            m_values = [..source];
            m_addCallback = null!;
            m_removeCallback = null!;
        }

        ~ReactiveList()
        {
            DisposeInternal(false);
        }

        public void Dispose()
        {
            DisposeInternal(true);
            GC.SuppressFinalize(this);
        }

        private void DisposeInternal(bool disposing)
        {
            if (m_values is not null)
            {
                m_values.Clear();
                m_values = null!;
            }

            if (m_addCallback is not null)
            {
                m_addCallback = null!;
            }

            if (m_removeCallback is not null)
            {
                m_removeCallback = null!;
            }
        }

        public void Add(T source)
        {
            m_values.Add(source);
            m_addCallback?.Invoke(source);
        }

        public void AddRange(ReadOnlySpan<T> sources)
        {
            if (sources.Length == 0)
            {
                return;
            }

            var _values = m_values;

            if (m_addCallback is null)
            {
                _values.AddRange(sources);
                return;
            }

            var _callback = m_addCallback;

            foreach (var _source in sources)
            {
                _values.Add(_source);
                _callback?.Invoke(_source);
            }
        }

        public void Remove(T source)
        {
            m_values.Remove(source);
            m_removeCallback?.Invoke(source);
        }

        public void RemoveRange(ReadOnlySpan<T> sources)
        {
            if (sources.Length == 0)
            {
                return;
            }

            var _values = m_values;

            if (m_removeCallback is null)
            {
                foreach (var _source in sources)
                {
                    _values.Remove(_source);
                }
                return;
            }

            var _callback = m_removeCallback;

            foreach (var _source in sources)
            {
                _values.Remove(_source);
                _callback?.Invoke(_source);
            }
        }

        public void BindAdd()
        {
        }

        public void BindRemove()
        {
        }
    }
}
