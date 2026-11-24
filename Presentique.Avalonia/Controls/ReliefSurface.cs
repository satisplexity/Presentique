using Presentique.Avalonia.Rendering;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia;

namespace Presentique.Avalonia.Controls;

public class ReliefSurface : Control
{
    public static readonly StyledProperty<IBrush?> BackgroundProperty =
        Border.BackgroundProperty.AddOwner<ReliefSurface>();

    public static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
        Border.CornerRadiusProperty.AddOwner<ReliefSurface>();

    public static readonly StyledProperty<double> CornerSmoothingProperty =
        AvaloniaProperty.Register<ReliefSurface, double>(
            nameof(CornerSmoothing),
            2,
            validate: v => double.IsFinite(v) && v >= 0 && v <= 10);

    public static readonly StyledProperty<double> DepthProperty =
        AvaloniaProperty.Register<ReliefSurface, double>(
            nameof(Depth), 
            1, 
            validate: double.IsFinite);

    public static readonly StyledProperty<double> LightStrengthProperty =
        AvaloniaProperty.Register<ReliefSurface, double>(
            nameof(LightStrength),
            1,
            validate: v => double.IsFinite(v) && v >= 0);
    public static readonly StyledProperty<double> LightAngleProperty =
        AvaloniaProperty.Register<ReliefSurface, double>(
            nameof(LightAngle), 
            45, 
            validate: double.IsFinite);

    static ReliefSurface()
    {
        AffectsRender<ReliefSurface>(
            BackgroundProperty, 
            CornerRadiusProperty, 
            CornerSmoothingProperty,
            DepthProperty, 
            LightStrengthProperty,
            LightAngleProperty);

        IsHitTestVisibleProperty.OverrideDefaultValue<ReliefSurface>(false);
    }

    public IBrush? Background
    { 
        get => GetValue(BackgroundProperty); 
        set => SetValue(BackgroundProperty, value); 
    }

    public CornerRadius CornerRadius 
    { 
        get => GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public double CornerSmoothing
    { 
        get => GetValue(CornerSmoothingProperty); 
        set => SetValue(CornerSmoothingProperty, value);
    }

    public double Depth 
    { 
        get => GetValue(DepthProperty); 
        set => SetValue(DepthProperty, value); 
    }

    public double LightStrength 
    {
        get => GetValue(LightStrengthProperty); 
        set => SetValue(LightStrengthProperty, value); 
    }

    public double LightAngle 
    {
        get => GetValue(LightAngleProperty); 
        set => SetValue(LightAngleProperty, value); 
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (Bounds.Width <= 0 || Bounds.Height <= 0 || Background is null)
        {
            return;
        }
            
        context.Custom(new ReliefDrawOperation(
            new Rect(Bounds.Size),
            Background.ToImmutable(),
            CornerRadius,
            CornerSmoothing, 
            Depth,
            LightStrength, 
            LightAngle));
    }
}
