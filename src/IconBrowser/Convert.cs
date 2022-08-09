using System.Windows;
using System.Windows.Data;
using LambdaConverters;

namespace IconBrowser;

public static class Convert
{
    public static readonly IValueConverter HiddenIfFalse =
        ValueConverter.Create<bool, Visibility>(e => !e.Value ? Visibility.Hidden : Visibility.Visible);

    public static readonly IValueConverter HiddenIfTrue =
        ValueConverter.Create<bool, Visibility>(e => e.Value ? Visibility.Hidden : Visibility.Visible);

    public static readonly IValueConverter CollapsedIfFalse =
        ValueConverter.Create<bool, Visibility>(e => !e.Value ? Visibility.Collapsed : Visibility.Visible);

    public static readonly IValueConverter CollapsedIfTrue =
        ValueConverter.Create<bool, Visibility>(e => e.Value ? Visibility.Collapsed : Visibility.Visible);
}
