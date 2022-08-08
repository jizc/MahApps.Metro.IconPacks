using System.Windows.Media.Animation;

namespace MahApps.Metro.IconPacks;

public interface IPackIconExtension
{
    double Width { get; set; }
    double Height { get; set; }
    PackIconFlipOrientation Flip { get; set; }
    double RotationAngle { get; set; }
    bool Spin { get; set; }
    bool SpinAutoReverse { get; set; }
    IEasingFunction SpinEasingFunction { get; set; }
    double SpinDuration { get; set; }
}
