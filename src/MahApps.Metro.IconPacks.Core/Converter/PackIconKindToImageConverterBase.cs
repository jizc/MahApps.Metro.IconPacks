﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace MahApps.Metro.IconPacks.Converter;

public abstract class PackIconKindToImageConverterBase : MarkupConverter
{
    /// <summary>
    /// Gets or sets the brush to draw the icon.
    /// </summary>
    public Brush Brush { get; set; } = Brushes.Black;

    /// <summary>
    /// Gets or sets the flip orientation for the icon.
    /// </summary>
    public PackIconFlipOrientation Flip { get; set; } = PackIconFlipOrientation.Normal;

    /// <summary>
    /// Gets or sets the rotation (angle) for the icon.
    /// </summary>
    public double RotationAngle { get; set; }

    /// <summary>
    /// Gets the path data for the given kind.
    /// </summary>
    protected abstract string GetPathData(object iconKind);

    /// <summary>
    /// Gets the ScaleTransform for the given kind.
    /// </summary>
    /// <param name="iconKind">The icon kind to draw.</param>
    protected virtual ScaleTransform GetScaleTransform(object iconKind) => new(1, 1);

    /// <summary>
    /// Gets the <see cref="T:System.Windows.Media.TransformGroup" /> for the <see cref="T:System.Windows.Media.DrawingGroup" />.
    /// </summary>
    /// <param name="iconKind">The icon kind to draw.</param>
    protected Transform GetTransformGroup(object iconKind)
    {
        var transformGroup = new TransformGroup();
        transformGroup.Children.Add(GetScaleTransform(iconKind)); // scale
        transformGroup.Children.Add(new ScaleTransform(
                                        Flip is PackIconFlipOrientation.Horizontal or PackIconFlipOrientation.Both ? -1 : 1,
                                        Flip is PackIconFlipOrientation.Vertical or PackIconFlipOrientation.Both ? -1 : 1)); // flip
        transformGroup.Children.Add(new RotateTransform(RotationAngle)); // rotate

        return transformGroup;
    }

    /// <summary>
    /// Gets the <see cref="T:System.Windows.Media.DrawingGroup" /> object that will be used for the <see cref="T:System.Windows.Media.DrawingImage" />.
    /// </summary>
    protected virtual DrawingGroup GetDrawingGroup(object iconKind, Brush foregroundBrush, string path)
    {
        var geometryDrawing = new GeometryDrawing
        {
            Geometry = Geometry.Parse(path),
            Brush = foregroundBrush
        };

        var drawingGroup = new DrawingGroup
        {
            Children = { geometryDrawing },
            Transform = GetTransformGroup(iconKind)
        };

        return drawingGroup;
    }

    /// <summary>
    /// Gets the ImageSource for the given kind.
    /// </summary>
    protected ImageSource CreateImageSource(object iconKind, Brush foregroundBrush)
    {
        var path = GetPathData(iconKind);

        if (string.IsNullOrEmpty(path))
        {
            return null;
        }

        var drawingImage = new DrawingImage(GetDrawingGroup(iconKind, foregroundBrush, path));
        drawingImage.Freeze();

        return drawingImage;
    }

    /// <inheritdoc />
    protected override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not Enum)
        {
            return DependencyProperty.UnsetValue;
        }

        var imageSource = CreateImageSource(value, parameter as Brush ?? Brush ?? Brushes.Black);
        return imageSource ?? DependencyProperty.UnsetValue;
    }

    /// <inheritdoc />
    protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => DependencyProperty.UnsetValue;
}
