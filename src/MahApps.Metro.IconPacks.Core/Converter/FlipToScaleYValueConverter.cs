using System;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace MahApps.Metro.IconPacks.Converter;

/// <summary>
/// ValueConverter which converts the PackIconFlipOrientation enumeration value to ScaleY value of a ScaleTransformation.
/// </summary>
[MarkupExtensionReturnType(typeof(FlipToScaleYValueConverter))]
public class FlipToScaleYValueConverter : MarkupConverter
{
    private static FlipToScaleYValueConverter _instance;

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static FlipToScaleYValueConverter()
    {
    }

    public static FlipToScaleYValueConverter Instance { get; } = _instance ??= new FlipToScaleYValueConverter();

    public override object ProvideValue(IServiceProvider serviceProvider) => Instance;

    protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is PackIconFlipOrientation flip)
        {
            var scaleY = flip is PackIconFlipOrientation.Vertical or PackIconFlipOrientation.Both ? -1 : 1;
            return scaleY;
        }

        return 1;
    }

    protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => DependencyProperty.UnsetValue;
}
