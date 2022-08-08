using System;
using System.Windows.Markup;

namespace MahApps.Metro.IconPacks
{
    [MarkupExtensionReturnType(typeof(PackIconMaterial))]
    public class MaterialExtension : BasePackIconExtension
    {
        public MaterialExtension()
        {
        }

        public MaterialExtension(PackIconMaterialKind kind)
        {
            this.Kind = kind;
        }

        [ConstructorArgument("kind")]
        public PackIconMaterialKind Kind { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this.GetPackIcon<PackIconMaterial, PackIconMaterialKind>(this.Kind);
        }
    }
}