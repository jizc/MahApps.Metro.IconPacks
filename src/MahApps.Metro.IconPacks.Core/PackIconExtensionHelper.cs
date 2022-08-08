namespace MahApps.Metro.IconPacks;

public static class PackIconExtensionHelper
{
    public static PackIconControlBase GetPackIcon<TPack, TKind>(this IPackIconExtension packIconExtension, TKind kind)
        where TPack : PackIconControlBase, new()
    {
        var packIcon = new TPack();
        packIcon.SetKind(kind);

        if (packIconExtension is not PackIconExtensionBase extension)
        {
            return packIcon;
        }

        if (extension.IsFieldChanged(PackIconExtensionBase.ChangedFieldFlags.Width))
        {
            packIcon.Width = packIconExtension.Width;
        }

        if (extension.IsFieldChanged(PackIconExtensionBase.ChangedFieldFlags.Height))
        {
            packIcon.Height = packIconExtension.Height;
        }

        if (extension.IsFieldChanged(PackIconExtensionBase.ChangedFieldFlags.Flip))
        {
            packIcon.Flip = packIconExtension.Flip;
        }

        if (extension.IsFieldChanged(PackIconExtensionBase.ChangedFieldFlags.RotationAngle))
        {
            packIcon.RotationAngle = packIconExtension.RotationAngle;
        }

        if (extension.IsFieldChanged(PackIconExtensionBase.ChangedFieldFlags.Spin))
        {
            packIcon.Spin = packIconExtension.Spin;
        }

        if (extension.IsFieldChanged(PackIconExtensionBase.ChangedFieldFlags.SpinAutoReverse))
        {
            packIcon.SpinAutoReverse = packIconExtension.SpinAutoReverse;
        }

        if (extension.IsFieldChanged(PackIconExtensionBase.ChangedFieldFlags.SpinEasingFunction))
        {
            packIcon.SpinEasingFunction = packIconExtension.SpinEasingFunction;
        }

        if (extension.IsFieldChanged(PackIconExtensionBase.ChangedFieldFlags.SpinDuration))
        {
            packIcon.SpinDuration = packIconExtension.SpinDuration;
        }

        return packIcon;
    }
}
