using System;
using System.Globalization;
using System.Windows;

namespace MahApps.Metro.IconPacks.Converter;

public class DataTypeValueConverter : MarkupConverter
{
    private static DataTypeValueConverter _instance;

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static DataTypeValueConverter()
    {
    }

    public override object ProvideValue(IServiceProvider serviceProvider) => _instance ??= new DataTypeValueConverter();

    protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value?.GetType();

    protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => DependencyProperty.UnsetValue;
}
