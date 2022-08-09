using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using MahApps.Metro.IconPacks;

namespace IconBrowser;

public class IconBrowserViewModel : NotifyPropertyChangedBase
{
    private readonly ICollectionView iconsView;

    private string searchText = string.Empty;
    private PackIconMaterialKind? selectedIcon;

    public IconBrowserViewModel()
    {
        Icons = new ObservableCollection<PackIconMaterialKind>(Enum.GetValues<PackIconMaterialKind>().Skip(1));
        selectedIcon = Icons.FirstOrDefault();
        iconsView = CollectionViewSource.GetDefaultView(Icons);
        iconsView.Filter = IconsFilter;
        ClearSearchBoxCommand = new DelegateCommand(() => SearchText = string.Empty, () => !string.IsNullOrEmpty(searchText));
        CopyNameCommand = new DelegateCommand(CopyName, () => selectedIcon.HasValue);
    }

    public DelegateCommand ClearSearchBoxCommand { get; }
    public DelegateCommand CopyNameCommand { get; }

    public ObservableCollection<PackIconMaterialKind> Icons { get; }

    public string SearchText
    {
        get => searchText;
        set
        {
            if (SetProperty(ref searchText, value))
            {
                iconsView.Refresh();
                ClearSearchBoxCommand.RaiseCanExecuteChanged();
                if (!selectedIcon.HasValue)
                {
                    SelectedIcon = iconsView.Cast<PackIconMaterialKind>().FirstOrDefault();
                }
            }
        }
    }

    public PackIconMaterialKind? SelectedIcon
    {
        get => selectedIcon;
        set
        {
            if (SetProperty(ref selectedIcon, value))
            {
                CopyNameCommand.RaiseCanExecuteChanged();
            }
        }
    }

    private static async Task SetTextSafelyAsync(string text)
    {
        for (var i = 0; i < 10; ++i)
        {
            try
            {
                Clipboard.SetText(text);
                break;
            }
            catch
            {
                // ignored
            }

            await Task.Delay(10);
        }
    }

    private async void CopyName()
    {
        var iconString = selectedIcon?.ToString();
        if (iconString is not null)
        {
            await SetTextSafelyAsync(iconString);
        }
    }

    private bool IconsFilter(object obj)
    {
        if (obj is not PackIconMaterialKind icon)
        {
            return false;
        }

        return string.IsNullOrWhiteSpace(searchText)
               || icon.ToString().Contains(searchText, StringComparison.InvariantCultureIgnoreCase);
    }
}
