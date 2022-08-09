using System;
using System.Windows.Markup;

namespace MahApps.Metro.IconPacks;

[MarkupExtensionReturnType(typeof(PackIconMaterial))]
public class MaterialExtension : PackIconExtensionBase
{
    public MaterialExtension()
    {
    }

    public MaterialExtension(PackIconMaterialKind kind)
    {
        Kind = kind;
    }

    [ConstructorArgument("kind")]
    public PackIconMaterialKind Kind { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider) => this.GetPackIcon<PackIconMaterial, PackIconMaterialKind>(Kind);
}
