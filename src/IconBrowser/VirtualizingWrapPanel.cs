// LYCJ (c) 2010 - http://www.quickzip.org/components
// Release under LGPL license.
//
// This code used part of Dan Crevier's and Ben Constable's work
// (http://blogs.msdn.com/dancre/archive/2006/02/16/implementing-a-virtualizingpanel-part-4-the-goods.aspx)
// (http://blogs.msdn.com/bencon/)

#nullable disable
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace IconBrowser;

public class VirtualizingWrapPanel : VirtualizingPanel, IScrollInfo
{
    public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register(
        nameof(ItemWidth),
        typeof(double),
        typeof(VirtualizingWrapPanel),
        new FrameworkPropertyMetadata(200.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

    public static readonly DependencyProperty ItemHeightProperty = DependencyProperty.Register(
        nameof(ItemHeight),
        typeof(double),
        typeof(VirtualizingWrapPanel),
        new FrameworkPropertyMetadata(200.0, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        nameof(Orientation),
        typeof(Orientation),
        typeof(VirtualizingWrapPanel),
        new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

    public static readonly DependencyProperty ScrollSpeedProperty = DependencyProperty.Register(
        nameof(ScrollSpeed),
        typeof(uint),
        typeof(VirtualizingWrapPanel),
        new FrameworkPropertyMetadata(50u, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange));

    private readonly TranslateTransform trans = new();

    private Size extent;
    private Size viewport;
    private Point offset;

    public VirtualizingWrapPanel()
    {
        // For use in the IScrollInfo implementation
        RenderTransform = trans;
    }

    public ScrollViewer ScrollOwner { get; set; }

    public bool CanHorizontallyScroll { get; set; }
    public bool CanVerticallyScroll { get; set; }

    public double HorizontalOffset => offset.X;
    public double VerticalOffset => offset.Y;

    public double ExtentHeight => extent.Height;
    public double ExtentWidth => extent.Width;

    public double ViewportHeight => viewport.Height;
    public double ViewportWidth => viewport.Width;

    public double ItemWidth
    {
        get => (double)GetValue(ItemWidthProperty);
        set => SetValue(ItemWidthProperty, value);
    }

    public double ItemHeight
    {
        get => (double)GetValue(ItemHeightProperty);
        set => SetValue(ItemHeightProperty, value);
    }

    public Size ItemSize => new(ItemWidth, ItemHeight);

    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public uint ScrollSpeed
    {
        get => (uint)GetValue(ScrollSpeedProperty);
        set => SetValue(ScrollSpeedProperty, value);
    }

    public void AddOffset(bool pageOffset)
    {
        var yChanges = pageOffset ? viewport.Height : ScrollSpeed;
        var xChanges = pageOffset ? viewport.Width : ScrollSpeed;

        if (Orientation is Orientation.Horizontal)
        {
            SetVerticalOffset(VerticalOffset + yChanges);
        }
        else
        {
            SetHorizontalOffset(HorizontalOffset + xChanges);
        }
    }

    public void SubtractOffset(bool pageOffset)
    {
        var yChanges = pageOffset ? viewport.Height : ScrollSpeed;
        var xChanges = pageOffset ? viewport.Width : ScrollSpeed;

        if (Orientation is Orientation.Horizontal)
        {
            SetVerticalOffset(VerticalOffset - yChanges);
        }
        else
        {
            SetHorizontalOffset(HorizontalOffset - xChanges);
        }
    }

    public void LineUp() => SetVerticalOffset(VerticalOffset - ScrollSpeed);
    public void LineDown() => SetVerticalOffset(VerticalOffset + ScrollSpeed);
    public void LineLeft() => SetHorizontalOffset(HorizontalOffset - ScrollSpeed);
    public void LineRight() => SetHorizontalOffset(HorizontalOffset + ScrollSpeed);

    public void PageUp() => SubtractOffset(false);
    public void PageDown() => AddOffset(true);
    public void PageLeft() => SetHorizontalOffset(HorizontalOffset - viewport.Width);
    public void PageRight() => SetHorizontalOffset(HorizontalOffset + viewport.Width);

    public void MouseWheelUp() => SubtractOffset(false);
    public void MouseWheelDown() => AddOffset(false);
    public void MouseWheelLeft() => SetHorizontalOffset(HorizontalOffset - ScrollSpeed);
    public void MouseWheelRight() => SetHorizontalOffset(HorizontalOffset + ScrollSpeed);

    public void SetHorizontalOffset(double horizontalOffset) => SetHorizontalOffsetImpl(horizontalOffset, true, true);
    public void SetVerticalOffset(double verticalOffset) => SetVerticalOffsetImpl(verticalOffset, true, true);

    public Rect MakeVisible(Visual visual, Rect rectangle) => default;

    public Rect GetChildRect(int itemIndex) => GetChildRect(itemIndex, extent);

    /// <summary>
    /// Measure the children
    /// </summary>
    /// <param name="availableSize">Size available</param>
    /// <returns>Size desired</returns>
    protected override Size MeasureOverride(Size availableSize)
    {
        UpdateScrollInfo(availableSize);

        // Figure out range that's visible based on layout algorithm
        GetVisibleRange(out var firstVisibleItemIndex, out var lastVisibleItemIndex);

        // We need to access InternalChildren before the generator to work around a bug
        var children = InternalChildren;
        var generator = ItemContainerGenerator;

        // Get the generator position of the first visible data item
        var startPos = generator.GeneratorPositionFromIndex(firstVisibleItemIndex);

        // Get index where we'd insert the child for this position. If the item is realized
        // (position.Offset is 0), it's just position.Index, otherwise we have to add one to
        // insert after the corresponding child
        var childIndex = startPos.Offset is 0 ? startPos.Index : startPos.Index + 1;

        using (generator.StartAt(startPos, GeneratorDirection.Forward, true))
        {
            for (var itemIndex = firstVisibleItemIndex; itemIndex <= lastVisibleItemIndex; ++itemIndex, ++childIndex)
            {
                // Get or create the child
                if (generator.GenerateNext(out var newlyRealized) is not UIElement child)
                {
                    break;
                }

                if (newlyRealized)
                {
                    // Figure out if we need to insert the child at the end or somewhere in the middle
                    if (childIndex >= children.Count)
                    {
                        AddInternalChild(child);
                    }
                    else
                    {
                        InsertInternalChild(childIndex, child);
                    }

                    generator.PrepareItemContainer(child);
                }
                else
                {
                    // The child has already been created, let's be sure it's in the right spot
                    Debug.Assert(child == children[childIndex], "Wrong child was generated");
                }

                // Measurements will depend on layout algorithm
                child.Measure(ItemSize);
            }
        }

        // Note: this could be deferred to idle time for efficiency
        CleanUpItems(firstVisibleItemIndex, lastVisibleItemIndex);

        if (double.IsInfinity(availableSize.Width))
        {
            availableSize.Width = 0.0;
        }

        if (double.IsInfinity(availableSize.Height))
        {
            availableSize.Height = 0.0;
        }

        return availableSize;
    }

    /// <summary>
    /// Arrange the children
    /// </summary>
    /// <param name="finalSize">Size available</param>
    /// <returns>Size used</returns>
    protected override Size ArrangeOverride(Size finalSize)
    {
        var generator = ItemContainerGenerator;

        UpdateScrollInfo(finalSize);

        for (var i = 0; i < Children.Count; i++)
        {
            var child = Children[i];

            // Map the child offset to an item offset
            var itemIndex = generator.IndexFromGeneratorPosition(new GeneratorPosition(i, 0));

            ArrangeChild(itemIndex, child, finalSize);
        }

        return finalSize;
    }

    /// <summary>
    /// When items are removed, remove the corresponding UI if necessary
    /// </summary>
    protected override void OnItemsChanged(object sender, ItemsChangedEventArgs args)
    {
        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Remove:
            case NotifyCollectionChangedAction.Replace:
            case NotifyCollectionChangedAction.Move:
                RemoveInternalChildRange(args.Position.Index, args.ItemUICount);
                break;
        }
    }

    private void SetHorizontalOffsetImpl(double horizontalOffset, bool invalidateScrollInfo, bool invalidateMeasure)
    {
        if (horizontalOffset < 0 || viewport.Width >= extent.Width)
        {
            horizontalOffset = 0;
        }
        else
        {
            if (horizontalOffset + viewport.Width >= extent.Width)
            {
                horizontalOffset = extent.Width - viewport.Width;
            }
        }

        offset.X = horizontalOffset;

        if (invalidateScrollInfo)
        {
            ScrollOwner?.InvalidateScrollInfo();
        }

        trans.X = -horizontalOffset;

        // Force us to realize the correct children
        if (invalidateMeasure)
        {
            InvalidateMeasure();
        }
    }

    private void SetVerticalOffsetImpl(double verticalOffset, bool invalidateScrollInfo, bool invalidateMeasure)
    {
        if (verticalOffset < 0 || viewport.Height >= extent.Height)
        {
            verticalOffset = 0;
        }
        else
        {
            if (verticalOffset + viewport.Height >= extent.Height)
            {
                verticalOffset = extent.Height - viewport.Height;
            }
        }

        offset.Y = verticalOffset;

        if (invalidateScrollInfo)
        {
            ScrollOwner?.InvalidateScrollInfo();
        }

        trans.Y = -verticalOffset;

        // Force us to realize the correct children
        if (invalidateMeasure)
        {
            InvalidateMeasure();
        }
    }

    /// <summary>
    /// Calculate the extent of the view based on the available size
    /// </summary>
    /// <param name="availableSize">available size</param>
    /// <param name="itemCount">number of data items</param>
    private Size CalculateExtent(Size availableSize, int itemCount)
    {
        if (Orientation is Orientation.Horizontal)
        {
            var childPerRow = CalculateChildrenPerRow(availableSize);
            return new Size(
                childPerRow * ItemSize.Width,
                ItemSize.Height * Math.Ceiling((double)itemCount / childPerRow));
        }

        var childPerCol = CalculateChildrenPerCol(availableSize);
        return new Size(
            ItemSize.Width * Math.Ceiling((double)itemCount / childPerCol),
            childPerCol * ItemSize.Height);
    }

    /// <summary>
    /// Get the range of children that are visible
    /// </summary>
    /// <param name="firstVisibleItemIndex">The item index of the first visible item</param>
    /// <param name="lastVisibleItemIndex">The item index of the last visible item</param>
    private void GetVisibleRange(out int firstVisibleItemIndex, out int lastVisibleItemIndex)
    {
        if (Orientation is Orientation.Horizontal)
        {
            var childPerRow = CalculateChildrenPerRow(extent);

            firstVisibleItemIndex = (int)Math.Floor(offset.Y / ItemSize.Height) * childPerRow;
            lastVisibleItemIndex = ((int)Math.Ceiling((offset.Y + viewport.Height) / ItemSize.Height) * childPerRow) - 1;

            var itemsControl = ItemsControl.GetItemsOwner(this);
            var itemCount = itemsControl.HasItems ? itemsControl.Items.Count : 0;
            if (lastVisibleItemIndex >= itemCount)
            {
                lastVisibleItemIndex = itemCount - 1;
            }
        }
        else
        {
            var childPerCol = CalculateChildrenPerCol(extent);

            firstVisibleItemIndex = (int)Math.Floor(offset.X / ItemSize.Width) * childPerCol;
            lastVisibleItemIndex = ((int)Math.Ceiling((offset.X + viewport.Width) / ItemSize.Width) * childPerCol) - 1;

            var itemsControl = ItemsControl.GetItemsOwner(this);
            var itemCount = itemsControl.HasItems ? itemsControl.Items.Count : 0;
            if (lastVisibleItemIndex >= itemCount)
            {
                lastVisibleItemIndex = itemCount - 1;
            }
        }
    }

    private Rect GetChildRect(int itemIndex, Size finalSize)
    {
        if (Orientation is Orientation.Horizontal)
        {
            var childPerRow = CalculateChildrenPerRow(finalSize);

            var row = itemIndex / childPerRow;
            var column = itemIndex % childPerRow;

            return new Rect(column * ItemSize.Width, row * ItemSize.Height, ItemSize.Width, ItemSize.Height);
        }
        else
        {
            var childPerCol = CalculateChildrenPerCol(finalSize);

            var column = itemIndex / childPerCol;
            var row = itemIndex % childPerCol;

            return new Rect(column * ItemSize.Width, row * ItemSize.Height, ItemSize.Width, ItemSize.Height);
        }
    }

    /// <summary>
    /// Position a child
    /// </summary>
    /// <param name="itemIndex">The data item index of the child</param>
    /// <param name="child">The element to position</param>
    /// <param name="finalSize">The size of the panel</param>
    private void ArrangeChild(int itemIndex, UIElement child, Size finalSize)
        => child.Arrange(GetChildRect(itemIndex, finalSize));

    /// <summary>
    /// Helper function for tiling layout
    /// </summary>
    /// <param name="availableSize">Size available</param>
    private int CalculateChildrenPerRow(Size availableSize)
    {
        // Figure out how many children fit on each row
        var childrenPerRow = double.IsPositiveInfinity(availableSize.Width)
            ? Children.Count
            : Math.Max(1, (int)Math.Floor(availableSize.Width / ItemSize.Width));

        return childrenPerRow;
    }

    /// <summary>
    /// Helper function for tiling layout
    /// </summary>
    /// <param name="availableSize">Size available</param>
    private int CalculateChildrenPerCol(Size availableSize)
    {
        // Figure out how many children fit on each row
        return double.IsPositiveInfinity(availableSize.Height)
            ? Children.Count
            : Math.Max(1, (int)Math.Floor(availableSize.Height / ItemSize.Height));
    }

    /// <summary>
    /// Revirtualize items that are no longer visible
    /// </summary>
    /// <param name="minDesiredGenerated">first item index that should be visible</param>
    /// <param name="maxDesiredGenerated">last item index that should be visible</param>
    private void CleanUpItems(int minDesiredGenerated, int maxDesiredGenerated)
    {
        var children = InternalChildren;
        var generator = ItemContainerGenerator;

        for (var i = children.Count - 1; i >= 0; i--)
        {
            var childGeneratorPos = new GeneratorPosition(i, 0);
            var itemIndex = generator.IndexFromGeneratorPosition(childGeneratorPos);
            if (itemIndex < minDesiredGenerated || itemIndex > maxDesiredGenerated)
            {
                generator.Remove(childGeneratorPos, 1);
                RemoveInternalChildRange(i, 1);
            }
        }
    }

    // See Ben Constable's series of posts at http://blogs.msdn.com/bencon/
    private void UpdateScrollInfo(Size availableSize)
    {
        // See how many items there are
        var itemsControl = ItemsControl.GetItemsOwner(this);
        var itemCount = itemsControl.HasItems ? itemsControl.Items.Count : 0;

        var newExtent = CalculateExtent(availableSize, itemCount);

        var extentChanged = false;
        var viewportChanged = false;

        // Update extent
        if (newExtent != extent)
        {
            extent = newExtent;
            extentChanged = true;
        }

        // Update viewport
        if (availableSize != viewport)
        {
            viewport = availableSize;
            viewportChanged = true;
        }

        if (extentChanged || viewportChanged)
        {
            SetHorizontalOffsetImpl(offset.X, false, false);
            SetVerticalOffsetImpl(offset.Y, false, false);
            ScrollOwner?.InvalidateScrollInfo();
        }
    }
}
