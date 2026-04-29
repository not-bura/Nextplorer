namespace Nextplorer.Domain
{
    public interface IHierarchyNode
    {
        public string Name { get; }
    }

    public sealed class DirectoryNode
        : IHierarchyNode
    {
        private string m_name;
        public string Name => m_name;
        public DirectoryNode(string name)
        {
            m_name = name;
        }
    }

    public sealed class FileNode
        : IHierarchyNode
    {
        private string m_name;
        public string Name => m_name;
        public FileNode(string name)
        {
            m_name = name;
        }
    }
}
