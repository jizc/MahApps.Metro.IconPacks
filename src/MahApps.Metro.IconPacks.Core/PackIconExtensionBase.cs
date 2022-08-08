using System;
using System.Windows.Markup;
using System.Windows.Media.Animation;

namespace MahApps.Metro.IconPacks;

[MarkupExtensionReturnType(typeof(PackIconBase))]
public abstract class PackIconExtensionBase : MarkupExtension, IPackIconExtension
{
    private double width = 16d;

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

    private double height = 16d;

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

    private PackIconFlipOrientation flip = PackIconFlipOrientation.Normal;

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

    private double rotationAngle;

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

    private bool spin;

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

    private bool spinAutoReverse;

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

    private IEasingFunction spinEasingFunction;

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

    private double spinDuration = 1d;

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

    private ChangedFieldFlags changedField; // Cache changed field bits

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
}
