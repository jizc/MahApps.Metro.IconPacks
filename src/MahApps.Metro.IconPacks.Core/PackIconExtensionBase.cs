using System;
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace MahApps.Metro.IconPacks;

[MarkupExtensionReturnType(typeof(PackIconBase))]
public abstract class PackIconExtensionBase : MarkupExtension, IPackIconExtension
{
    private double width = 16d;
    private double height = 16d;
    private PackIconFlipOrientation flip = PackIconFlipOrientation.Normal;
    private double rotationAngle;
    private bool spin;
    private bool spinAutoReverse;
    private IEasingFunction spinEasingFunction;
    private double spinDuration = 1d;
    private ChangedFieldFlags changedField; // Cache changed field bits

    [Flags]
    internal enum ChangedFieldFlags : ushort
    {
        Width = 0x0001,
        Height = 0x0002,
        Flip = 0x0004,
        RotationAngle = 0x0008,
        Spin = 0x0010,
        SpinAutoReverse = 0x0020,
        SpinEasingFunction = 0x0040,
        SpinDuration = 0x0080
    }

    public double Width
    {
        get => width;
        set
        {
            if (Equals(width, value))
            {
                return;
            }

            width = value;
            WriteFieldChangedFlag(ChangedFieldFlags.Width, true);
        }
    }

    public double Height
    {
        get => height;
        set
        {
            if (Equals(height, value))
            {
                return;
            }

            height = value;
            WriteFieldChangedFlag(ChangedFieldFlags.Height, true);
        }
    }

    public PackIconFlipOrientation Flip
    {
        get => flip;
        set
        {
            if (Equals(flip, value))
            {
                return;
            }

            flip = value;
            WriteFieldChangedFlag(ChangedFieldFlags.Flip, true);
        }
    }

    public double RotationAngle
    {
        get => rotationAngle;
        set
        {
            if (Equals(rotationAngle, value))
            {
                return;
            }

            rotationAngle = value;
            WriteFieldChangedFlag(ChangedFieldFlags.RotationAngle, true);
        }
    }

    public bool Spin
    {
        get => spin;
        set
        {
            if (Equals(spin, value))
            {
                return;
            }

            spin = value;
            WriteFieldChangedFlag(ChangedFieldFlags.Spin, true);
        }
    }

    public bool SpinAutoReverse
    {
        get => spinAutoReverse;
        set
        {
            if (Equals(spinAutoReverse, value))
            {
                return;
            }

            spinAutoReverse = value;
            WriteFieldChangedFlag(ChangedFieldFlags.SpinAutoReverse, true);
        }
    }

    public IEasingFunction SpinEasingFunction
    {
        get => spinEasingFunction;
        set
        {
            if (Equals(spinEasingFunction, value))
            {
                return;
            }

            spinEasingFunction = value;
            WriteFieldChangedFlag(ChangedFieldFlags.SpinEasingFunction, true);
        }
    }

    public double SpinDuration
    {
        get => spinDuration;
        set
        {
            if (Equals(spinDuration, value))
            {
                return;
            }

            spinDuration = value;
            WriteFieldChangedFlag(ChangedFieldFlags.SpinDuration, true);
        }
    }

    internal bool IsFieldChanged(ChangedFieldFlags reqFlag) => (changedField & reqFlag) is not 0;

    internal void WriteFieldChangedFlag(ChangedFieldFlags reqFlag, bool set)
    {
        if (set)
        {
            changedField |= reqFlag;
        }
        else
        {
            changedField &= (~reqFlag);
        }
    }
}
