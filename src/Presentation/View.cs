using Nextplorer.Domain;
using System.Windows.Controls;

namespace Nextplorer.Presentation
{
    public sealed class ViewModel
    {
        public required ReactiveList<TabModel> Children { get; init; }

        public ViewModel(TabModel child)
        {
            Children = new(child);
        }

        public ViewModel(ReadOnlySpan<TabModel> children)
        {
            Children = new(children);
        }
    }

    public sealed class TabModel
    {
        public required ReactiveProperty<string> Current { get; init; }

        public TabModel()
        {
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
}
