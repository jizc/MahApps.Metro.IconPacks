using System.Windows.Controls;

namespace MahApps.Metro.IconPacks
{
    public abstract class PackIconBase : Control
    {
        protected internal abstract void SetKind<TKind>(TKind iconKind);
        protected abstract void UpdateData();
    }
}