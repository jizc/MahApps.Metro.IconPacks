using System;
using System.Windows.Markup;
using System.Windows.Media;

namespace MahApps.Metro.IconPacks
{
    [MarkupExtensionReturnType(typeof(ImageSource))]
    public class MaterialImageExtension : PackIconImageExtensionBase
    {
        public MaterialImageExtension()
        {
        }

        public MaterialImageExtension(PackIconMaterialKind kind)
        {
            Kind = kind;
        }

        [ConstructorArgument("kind")]
        public PackIconMaterialKind Kind { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return CreateImageSource(Kind, Brush ?? Brushes.Black);
        }

        /// <inheritdoc />
        protected override string GetPathData(object iconKind)
        {
            string data = null;
            if (iconKind is PackIconMaterialKind kind)
            {
                PackIconMaterialDataFactory.DataIndex.Value?.TryGetValue(kind, out data);
            }
            return data;
        }
    }
}