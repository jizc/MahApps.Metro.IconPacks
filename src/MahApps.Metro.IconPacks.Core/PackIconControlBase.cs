using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MahApps.Metro.IconPacks
{
    /// <summary>
    /// Class PackIconControlBase which is the base class for any PackIcon control.
    /// </summary>
    public abstract class PackIconControlBase : PackIconBase
    {
        static PackIconControlBase()
        {
            OpacityProperty.OverrideMetadata(typeof(PackIconControlBase), new UIPropertyMetadata(1d, (d, _) => { d.CoerceValue(SpinProperty); }));
            VisibilityProperty.OverrideMetadata(typeof(PackIconControlBase), new UIPropertyMetadata(Visibility.Visible, (d, _) => { d.CoerceValue(SpinProperty); }));
        }

        private static readonly DependencyPropertyKey DataPropertyKey
            = DependencyProperty.RegisterReadOnly(nameof(Data), typeof(string), typeof(PackIconControlBase), new PropertyMetadata(""));

        // ReSharper disable once StaticMemberInGenericType
        public static readonly DependencyProperty DataProperty = DataPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the path data for the current icon kind.
        /// </summary>
        [TypeConverter(typeof(GeometryConverter))]
        public string Data
        {
            get => (string)GetValue(DataProperty);
            protected set => SetValue(DataPropertyKey, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            UpdateData();

            CoerceValue(SpinProperty);

            if (Spin)
            {
                StopSpinAnimation();
                BeginSpinAnimation();
            }
        }

        /// <summary>
        /// Identifies the Flip dependency property.
        /// </summary>
        public static readonly DependencyProperty FlipProperty
            = DependencyProperty.Register(
                nameof(Flip),
                typeof(PackIconFlipOrientation),
                typeof(PackIconControlBase),
                new PropertyMetadata(PackIconFlipOrientation.Normal));

        /// <summary>
        /// Gets or sets the flip orientation.
        /// </summary>
        public PackIconFlipOrientation Flip
        {
            get => (PackIconFlipOrientation)GetValue(FlipProperty);
            set => SetValue(FlipProperty, value);
        }

        /// <summary>
        /// Identifies the RotationAngle dependency property.
        /// </summary>
        public static readonly DependencyProperty RotationAngleProperty
            = DependencyProperty.Register(
                nameof(RotationAngle),
                typeof(double),
                typeof(PackIconControlBase),
                new PropertyMetadata(0d, null, (_, value) =>
                {
                    var val = (double)value;
                    return val < 0 ? 0d : (val > 360 ? 360d : value);
                }));

        /// <summary>
        /// Gets or sets the rotation (angle).
        /// </summary>
        /// <value>The rotation.</value>
        public double RotationAngle
        {
            get => (double)GetValue(RotationAngleProperty);
            set => SetValue(RotationAngleProperty, value);
        }

        /// <summary>
        /// Identifies the Spin dependency property.
        /// </summary>
        public static readonly DependencyProperty SpinProperty
            = DependencyProperty.Register(
                nameof(Spin),
                typeof(bool),
                typeof(PackIconControlBase),
                new PropertyMetadata(default(bool), SpinPropertyChangedCallback, SpinPropertyCoerceValueCallback));

        private static object SpinPropertyCoerceValueCallback(DependencyObject dependencyObject, object value)
        {
            if (dependencyObject is PackIconControlBase packIcon && (!packIcon.IsVisible || packIcon.Opacity <= 0 || packIcon.SpinDuration <= 0.0))
            {
                return false;
            }

            return value;
        }

        private static void SpinPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is PackIconControlBase packIcon && e.OldValue != e.NewValue && e.NewValue is bool newBool)
            {
                packIcon.ToggleSpinAnimation(newBool);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the inner icon is spinning.
        /// </summary>
        /// <value><c>true</c> if spin; otherwise, <c>false</c>.</value>
        public bool Spin
        {
            get => (bool)GetValue(SpinProperty);
            set => SetValue(SpinProperty, value);
        }

        private void ToggleSpinAnimation(bool spin)
        {
            if (spin)
            {
                BeginSpinAnimation();
            }
            else
            {
                StopSpinAnimation();
            }
        }

        private Storyboard spinningStoryboard;
        private FrameworkElement innerGrid;
        private FrameworkElement InnerGrid => innerGrid ??= GetTemplateChild("PART_InnerGrid") as FrameworkElement;

        private void BeginSpinAnimation()
        {
            var element = InnerGrid;
            if (element is null)
            {
                return;
            }

            var transformGroup = element.RenderTransform as TransformGroup ?? new TransformGroup();
            var rotateTransform = transformGroup.Children.OfType<RotateTransform>().LastOrDefault();

            if (rotateTransform is not null)
            {
                rotateTransform.Angle = 0;
            }
            else
            {
                transformGroup.Children.Add(new RotateTransform());
                element.RenderTransform = transformGroup;
            }

            var animation = new DoubleAnimation
            {
                From = 0,
                To = 360,
                AutoReverse = SpinAutoReverse,
                EasingFunction = SpinEasingFunction,
                RepeatBehavior = RepeatBehavior.Forever,
                Duration = new Duration(TimeSpan.FromSeconds(SpinDuration))
            };

            var storyboard = new Storyboard();
            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, element);
            Storyboard.SetTargetProperty(animation, new PropertyPath($"(0).(1)[{transformGroup.Children.Count - 1}].(2)", RenderTransformProperty, TransformGroup.ChildrenProperty, RotateTransform.AngleProperty));

            spinningStoryboard = storyboard;
            storyboard.Begin();
        }

        private void StopSpinAnimation()
        {
            var storyboard = spinningStoryboard;
            storyboard?.Stop();
            spinningStoryboard = null;
        }

        /// <summary>
        /// Identifies the SpinDuration dependency property.
        /// </summary>
        public static readonly DependencyProperty SpinDurationProperty
            = DependencyProperty.Register(
                nameof(SpinDuration),
                typeof(double),
                typeof(PackIconControlBase),
                new PropertyMetadata(1d, SpinDurationPropertyChangedCallback, SpinDurationCoerceValueCallback));

        private static void SpinDurationPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is PackIconControlBase packIcon && e.OldValue != e.NewValue && packIcon.Spin && e.NewValue is double)
            {
                packIcon.StopSpinAnimation();
                packIcon.BeginSpinAnimation();
            }
        }

        private static object SpinDurationCoerceValueCallback(DependencyObject dependencyObject, object value)
        {
            var val = (double)value;
            return val < 0 ? 0d : value;
        }

        /// <summary>
        /// Gets or sets the duration of the spinning animation (in seconds). This will also restart the spin animation.
        /// </summary>
        /// <value>The duration of the spin in seconds.</value>
        public double SpinDuration
        {
            get => (double)GetValue(SpinDurationProperty);
            set => SetValue(SpinDurationProperty, value);
        }

        /// <summary>
        /// Identifies the SpinEasingFunction dependency property.
        /// </summary>
        public static readonly DependencyProperty SpinEasingFunctionProperty
            = DependencyProperty.Register(
                nameof(SpinEasingFunction),
                typeof(IEasingFunction),
                typeof(PackIconControlBase),
                new PropertyMetadata(null, SpinEasingFunctionPropertyChangedCallback));

        private static void SpinEasingFunctionPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is PackIconControlBase packIcon && e.OldValue != e.NewValue && packIcon.Spin)
            {
                packIcon.StopSpinAnimation();
                packIcon.BeginSpinAnimation();
            }
        }

        /// <summary>
        /// Gets or sets the EasingFunction of the spinning animation. This will also restart the spin animation.
        /// </summary>
        /// <value>The spin easing function.</value>
        public IEasingFunction SpinEasingFunction
        {
            get => (IEasingFunction)GetValue(SpinEasingFunctionProperty);
            set => SetValue(SpinEasingFunctionProperty, value);
        }

        /// <summary>
        /// Identifies the SpinAutoReverse dependency property.
        /// </summary>
        public static readonly DependencyProperty SpinAutoReverseProperty
            = DependencyProperty.Register(
                nameof(SpinAutoReverse),
                typeof(bool),
                typeof(PackIconControlBase),
                new PropertyMetadata(default(bool), SpinAutoReversePropertyChangedCallback));

        private static void SpinAutoReversePropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is PackIconControlBase packIcon && e.OldValue != e.NewValue && packIcon.Spin && e.NewValue is bool)
            {
                packIcon.StopSpinAnimation();
                packIcon.BeginSpinAnimation();
            }
        }

        /// <summary>
        /// Gets or sets the AutoReverse of the spinning animation. This will also restart the spin animation.
        /// </summary>
        /// <value><c>true</c> if [spin automatic reverse]; otherwise, <c>false</c>.</value>
        public bool SpinAutoReverse
        {
            get => (bool)GetValue(SpinAutoReverseProperty);
            set => SetValue(SpinAutoReverseProperty, value);
        }
    }
}
