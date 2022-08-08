using System;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace MahApps.Metro.IconPacks.Converter;

/// <summary>
/// ValueConverter which converts the PackIconFlipOrientation enumeration value to ScaleX value of a ScaleTransformation.
/// </summary>
[MarkupExtensionReturnType(typeof(FlipToScaleXValueConverter))]
public class FlipToScaleXValueConverter : MarkupConverter
{
    private static FlipToScaleXValueConverter _instance;

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static FlipToScaleXValueConverter()
    {
    }

    public static FlipToScaleXValueConverter Instance { get; } = _instance ??= new FlipToScaleXValueConverter();

    public override object ProvideValue(IServiceProvider serviceProvider) => Instance;

    protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is PackIconFlipOrientation flip)
        {
            var scaleX = flip is PackIconFlipOrientation.Horizontal or PackIconFlipOrientation.Both ? -1 : 1;
            return scaleX;
        }

        return 1;
    }

    protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => DependencyProperty.UnsetValue;
}