using Avalonia.Controls;
using Avalonia.Media;
using Avalonia;

namespace Presentique.Avalonia.Controls;

public class ActionButton : Button
{
    public static readonly StyledProperty<StreamGeometry> IconProperty =
        AvaloniaProperty.Register<ActionButton, StreamGeometry>(nameof(Icon));

    public static readonly StyledProperty<double> CornerSmoothingProperty =
        AvaloniaProperty.Register<ReliefSurface, double>(nameof(CornerSmoothing), 2,
            validate: v => double.IsFinite(v) && v >= 0 && v <= 10);
    public static readonly StyledProperty<double> DepthProperty =
        AvaloniaProperty.Register<ReliefSurface, double>(nameof(Depth), 1, validate: double.IsFinite);
    public static readonly StyledProperty<double> LightStrengthProperty =
        AvaloniaProperty.Register<ReliefSurface, double>(nameof(LightStrength), 1,
            validate: v => double.IsFinite(v) && v >= 0);
    public static readonly StyledProperty<double> LightAngleProperty =
        AvaloniaProperty.Register<ReliefSurface, double>(nameof(LightAngle), 45, validate: double.IsFinite);

    /// <summary>Superellipse exponent minus two: zero is circular; two gives exponent four.</summary>
    public double CornerSmoothing
    {
        get => GetValue(CornerSmoothingProperty); set => SetValue(CornerSmoothingProperty, value);
    }
    /// <summary>Signed bevel width in DIPs. Negative values produce an inset surface.</summary>
    public double Depth
    {
        get => GetValue(DepthProperty); set => SetValue(DepthProperty, value);
    }
    public double LightStrength
    {
        get => GetValue(LightStrengthProperty); set => SetValue(LightStrengthProperty, value);
    }
    /// <summary>Clockwise degrees: zero is above, ninety is right. Light elevation is fixed at 45 degrees.</summary>
    public double LightAngle
    {
        get => GetValue(LightAngleProperty); set => SetValue(LightAngleProperty, value);
    }

    public StreamGeometry Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
}
