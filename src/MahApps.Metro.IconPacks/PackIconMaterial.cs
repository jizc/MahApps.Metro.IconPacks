using System.Windows;

namespace MahApps.Metro.IconPacks;

/// <summary>
/// All icons sourced from Material Design Icons Font <see><cref>https://materialdesignicons.com</cref></see>
/// In accordance of <see><cref>https://github.com/Templarian/MaterialDesign/blob/master/LICENSE</cref></see>.
/// </summary>
public class PackIconMaterial : PackIconControlBase
{
    public static readonly DependencyProperty KindProperty = DependencyProperty.Register(
        nameof(Kind),
        typeof(PackIconMaterialKind), 
        typeof(PackIconMaterial),
        new PropertyMetadata(default(PackIconMaterialKind), KindPropertyChangedCallback));

    static PackIconMaterial()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(PackIconMaterial), new FrameworkPropertyMetadata(typeof(PackIconMaterial)));
    }

    /// <summary>
    /// Gets or sets the icon to display.
    /// </summary>
    public PackIconMaterialKind Kind
    {
        get => (PackIconMaterialKind)GetValue(KindProperty);
        set => SetValue(KindProperty, value);
    }

    protected override void SetKind<TKind>(TKind iconKind) => SetCurrentValue(KindProperty, iconKind);

    protected override void UpdateData()
    {
        if (Kind != default)
        {
            string data = null;
            PackIconMaterialDataFactory.DataIndex.Value?.TryGetValue(Kind, out data);
            Data = data;
        }
        else
        {
            Data = null;
        }
    }

    private static void KindPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue != e.OldValue)
        {
            ((PackIconMaterial)dependencyObject).UpdateData();
        }
    }
}
