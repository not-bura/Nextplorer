using System.Runtime.CompilerServices;

namespace Nextplorer.Domain
{
    public sealed class ReactiveProperty<T>
        : IProperty
        , IDisposable
    {
        private T m_value;
        private event Action<T> m_callbacks;

        public T Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => m_value;
        }

        public ReactiveProperty(T value)
        {
            m_value = value;
            m_callbacks = null!;
        }

        public IDisposable Subscribe(Action<T> callback)
        {
            callback(m_value);
            m_callbacks += callback;
            return new IProperty.Handle(() => m_callbacks -= callback);
        }

        public void Dispose()
        {
            if (m_callbacks is not null)
            {
                m_callbacks = null!;
            }
        }

        public static implicit operator ReactiveProperty<T>(T other)
        {
            return new ReactiveProperty<T>(other);
        }

        public static implicit operator T(ReactiveProperty<T> self)
        {
            return self.Value;
        }
    }
}
