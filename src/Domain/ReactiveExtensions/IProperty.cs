namespace Nextplorer.Domain
{
    public interface IProperty
    {
        protected sealed class Handle(Action dispose)
            : IDisposable
        {
            private Action m_dispose = dispose;

            public void Dispose()
            {
                m_dispose();
            }
        }
    }
}
