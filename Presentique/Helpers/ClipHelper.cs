using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace Presentique.Helpers;

/// <summary>
/// Provides attached properties for clipping framework elements
/// to a rectangular shape with rounded corners.
/// </summary>
/// <remarks>
/// The clipping geometry is automatically updated when the element
/// is loaded, resized, or its clipping radius changes.
/// <br/>
/// For <see cref="Border"/> elements, the <see cref="Border.CornerRadius"/>
/// property is used instead of <see cref="RadiusProperty"/>.
/// </remarks>
public static class Clipper
{
    /// <summary>
    /// Identifies the Enable attached dependency property.
    /// </summary>
    public static readonly DependencyProperty EnableProperty
        = DependencyProperty.RegisterAttached(
            "Enable",
            typeof(bool),
            typeof(Clipper),
            new PropertyMetadata(false, OnEnableChanged));

    /// <summary>
    /// Identifies the Radius attached dependency property.
    /// </summary>
    public static readonly DependencyProperty RadiusProperty
        = DependencyProperty.RegisterAttached(
            "Radius",
            typeof(CornerRadius),
            typeof(Clipper),
            new PropertyMetadata(default(CornerRadius), OnRadiusChanged));

    /// <summary>
    /// Sets whether rounded clipping is enabled for the specified element.
    /// </summary>
    /// <param name="element">
    /// The element on which to set the attached property.
    /// </param>
    /// <param name="value">
    /// <see langword="true"/> to enable rounded clipping;
    /// otherwise, <see langword="false"/>.
    /// </param>
    public static void SetEnable(DependencyObject element, bool value)
        => element.SetValue(EnableProperty, value);

    /// <summary>
    /// Gets whether rounded clipping is enabled for the specified element.
    /// </summary>
    /// <param name="element">
    /// The element from which to read the attached property.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if rounded clipping is enabled;
    /// otherwise, <see langword="false"/>.
    /// The default value is <see langword="false"/>.
    /// </returns>
    public static bool GetEnable(DependencyObject element)
        => (bool)element.GetValue(EnableProperty);

    /// <summary>
    /// Sets the corner radius used for clipping the specified element.
    /// </summary>
    /// <param name="element">
    /// The element on which to set the attached property.
    /// </param>
    /// <param name="value">
    /// The radius of each corner.
    /// </param>
    /// <remarks>
    /// For <see cref="Border"/> elements, the
    /// <see cref="Border.CornerRadius"/> property takes precedence.
    /// </remarks>
    public static void SetRadius(DependencyObject element, CornerRadius value)
        => element.SetValue(RadiusProperty, value);

    /// <summary>
    /// Gets the configured clipping corner radius of the specified element.
    /// </summary>
    /// <param name="element">
    /// The element from which to read the attached property.
    /// </param>
    /// <returns>
    /// The configured corner radius.
    /// The default value is zero for all corners.
    /// </returns>
    public static CornerRadius GetRadius(DependencyObject element)
        => (CornerRadius)element.GetValue(RadiusProperty);

    /// <summary>
    /// Handles changes to the Enable attached property.
    /// </summary>
    /// <param name="sender">
    /// The object whose property value has changed.
    /// </param>
    /// <param name="args">
    /// The event data containing the old and new property values.
    /// </param>
    private static void OnEnableChanged(
        DependencyObject sender,
        DependencyPropertyChangedEventArgs args)
    {
        if (sender is not FrameworkElement element)
        {
            return;
        }

        if ((bool)args.NewValue)
        {
            element.SizeChanged += Element_SizeChanged;
            element.Loaded += Element_Loaded;

            UpdateClip(element);
        }
        else
        {
            element.SizeChanged -= Element_SizeChanged;
            element.Loaded -= Element_Loaded;

            element.Clip = null;
        }
    }

    /// <summary>
    /// Handles changes to the Radius attached property
    /// and updates the clipping geometry when enabled.
    /// </summary>
    /// <param name="sender">
    /// The object whose property value has changed.
    /// </param>
    /// <param name="args">
    /// The event data containing the old and new property values.
    /// </param>
    private static void OnRadiusChanged(
        DependencyObject sender,
        DependencyPropertyChangedEventArgs args)
    {
        if (sender is FrameworkElement element && GetEnable(element))
        {
            UpdateClip(element);
        }
    }

    /// <summary>
    /// Updates the clipping geometry when the element is loaded.
    /// </summary>
    /// <param name="sender">
    /// The element that raised the event.
    /// </param>
    /// <param name="args">
    /// The event data.
    /// </param>
    private static void Element_Loaded(
        object sender,
        RoutedEventArgs args)
        => UpdateClip((FrameworkElement)sender);

    /// <summary>
    /// Updates the clipping geometry when the element's size changes.
    /// </summary>
    /// <param name="sender">
    /// The element that raised the event.
    /// </param>
    /// <param name="args">
    /// The event data describing the size change.
    /// </param>
    private static void Element_SizeChanged(
        object sender,
        SizeChangedEventArgs args)
        => UpdateClip((FrameworkElement)sender);

    /// <summary>
    /// Updates the clipping geometry based on the element's
    /// current dimensions and effective corner radius.
    /// </summary>
    /// <param name="element">
    /// The element whose clipping geometry is updated.
    /// </param>
    private static void UpdateClip(FrameworkElement element)
    {
        double width = element.ActualWidth;
        double height = element.ActualHeight;

        if (width <= 0 || height <= 0)
        {
            element.Clip = null;

            return;
        }

        CornerRadius radius = GetEffectiveRadius(element);

        double maxRadius = Math.Min(width, height) / 2;

        radius.TopLeft = Math.Min(radius.TopLeft, maxRadius);
        radius.TopRight = Math.Min(radius.TopRight, maxRadius);
        radius.BottomLeft = Math.Min(radius.BottomLeft, maxRadius);
        radius.BottomRight = Math.Min(radius.BottomRight, maxRadius);

        element.Clip = CreateGeometry(width, height, radius);
    }

    /// <summary>
    /// Gets the effective corner radius used to clip the element.
    /// </summary>
    /// <param name="element">
    /// The element for which to retrieve the corner radius.
    /// </param>
    /// <returns>
    /// The <see cref="Border.CornerRadius"/> value for border elements;
    /// otherwise, the attached clipping radius.
    /// </returns>
    private static CornerRadius GetEffectiveRadius(FrameworkElement element)
    {
        if (element is Border border)
        {
            return border.CornerRadius;
        }

        return GetRadius(element);
    }

    /// <summary>
    /// Creates a rounded rectangular clipping geometry
    /// with individually configurable corner radii.
    /// </summary>
    /// <param name="width">
    /// The width of the clipping region.
    /// </param>
    /// <param name="height">
    /// The height of the clipping region.
    /// </param>
    /// <param name="radius">
    /// The corner radii to apply to the clipping geometry.
    /// </param>
    /// <returns>
    /// A <see cref="StreamGeometry"/> representing
    /// the rounded rectangular clipping region.
    /// </returns>
    private static StreamGeometry CreateGeometry(
        double width,
        double height,
        CornerRadius radius)
    {
        StreamGeometry geometry = new();

        using StreamGeometryContext ctx = geometry.Open();

        ctx.BeginFigure(
            new Point(radius.TopLeft, 0),
            true,
            true);

        ctx.LineTo(
            new Point(width - radius.TopRight, 0),
            true,
            false);

        ctx.ArcTo(
            new Point(width, radius.TopRight),
            new Size(radius.TopRight, radius.TopRight),
            0,
            false,
            SweepDirection.Clockwise,
            true,
            false);

        ctx.LineTo(
            new Point(width, height - radius.BottomRight),
            true,
            false);

        ctx.ArcTo(
            new Point(width - radius.BottomRight, height),
            new Size(radius.BottomRight, radius.BottomRight),
            0,
            false,
            SweepDirection.Clockwise,
            true,
            false);

        ctx.LineTo(
            new Point(radius.BottomLeft, height),
            true,
            false);

        ctx.ArcTo(
            new Point(0, height - radius.BottomLeft),
            new Size(radius.BottomLeft, radius.BottomLeft),
            0,
            false,
            SweepDirection.Clockwise,
            true,
            false);

        ctx.LineTo(
            new Point(0, radius.TopLeft),
            true,
            false);

        ctx.ArcTo(
            new Point(radius.TopLeft, 0),
            new Size(radius.TopLeft, radius.TopLeft),
            0,
            false,
            SweepDirection.Clockwise,
            true,
            false);

        geometry.Freeze();

        return geometry;
    }
}
