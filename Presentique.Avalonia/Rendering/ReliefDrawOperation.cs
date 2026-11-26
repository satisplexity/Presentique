using Avalonia.Rendering.SceneGraph;
using Avalonia.Media;
using Avalonia.Skia;
using SkiaSharp;
using Avalonia;

namespace Presentique.Avalonia.Rendering;

internal sealed class ReliefDrawOperation(
    Rect bounds,
    IBrush background,
    CornerRadius radii,
    double smoothing,
    double depth,
    double strength,
    double angle) : ICustomDrawOperation
{
    public Rect Bounds { get; } = bounds;

    private IBrush Background { get; } = background;
    private CornerRadius Radii { get; } = radii;
    private double Smoothing { get; } = smoothing;
    private double Depth { get; } = depth;
    private double Strength { get; } = strength;
    private double Angle { get; } = angle;

    public void Dispose() { }

    public bool HitTest(Point p) => false;

    public bool Equals(ICustomDrawOperation? other)
        => other is ReliefDrawOperation operation
        && Background.Equals(operation.Background)
        && Smoothing == operation.Smoothing
        && Strength == operation.Strength
        && Bounds == operation.Bounds
        && Radii == operation.Radii
        && Depth == operation.Depth
        && Angle == operation.Angle;

    public void Render(ImmediateDrawingContext context)
    {
        ISkiaSharpApiLeaseFeature feature =
            context.TryGetFeature<ISkiaSharpApiLeaseFeature>()
            ?? throw new NotSupportedException("ReliefSurface requires the Avalonia Skia renderer.");

        using ISkiaSharpApiLease lease = feature.Lease();
        Draw(lease.SkCanvas, (float)lease.CurrentOpacity);
    }

    internal void Draw(SKCanvas canvas, float opacity = 1)
    {
        Size size = Bounds.Size;
        SKRuntimeEffect effect = ReliefShader.Effect;

        using SKShader fill = ReliefBrushConverter.Create(Background, size);
        SKRuntimeEffectUniforms uniforms = CreateUniforms(effect, canvas.TotalMatrix, opacity);
        var children = new SKRuntimeEffectChildren(effect) { ["background"] = fill };
        using SKShader shader = effect.ToShader(uniforms, children);
        using var paint = new SKPaint { Shader = shader, IsAntialias = true };

        canvas.DrawRect(new SKRect(0, 0, (float)size.Width, (float)size.Height), paint);
    }

    private SKRuntimeEffectUniforms CreateUniforms(SKRuntimeEffect effect, SKMatrix matrix, float opacity)
    {
        Size size = Bounds.Size;
        double maximumDepth = Math.Min(size.Width, size.Height) / 2;

        return new SKRuntimeEffectUniforms(effect)
        {
            ["size"] = new[] { (float)size.Width, (float)size.Height },
            ["radii"] = GetNormalizedRadii(size),
            ["exponent"] = (float)(2 + Smoothing),
            ["depth"] = (float)Math.Clamp(Depth, -maximumDepth, maximumDepth),
            ["strength"] = (float)Math.Min(Strength, 100),
            ["light"] = GetLightDirection(),
            ["aa"] = (float)(0.75 / GetPixelScale(matrix)),
            ["opacity"] = opacity
        };
    }

    private float[] GetNormalizedRadii(Size size)
    {
        double scale = 1;

        LimitScale(size.Width, Radii.TopLeft + Radii.TopRight);
        LimitScale(size.Width, Radii.BottomLeft + Radii.BottomRight);
        LimitScale(size.Height, Radii.TopLeft + Radii.BottomLeft);
        LimitScale(size.Height, Radii.TopRight + Radii.BottomRight);

        return
        [
            (float)(Radii.TopLeft * scale),
            (float)(Radii.TopRight * scale),
            (float)(Radii.BottomRight * scale),
            (float)(Radii.BottomLeft * scale)
        ];

        void LimitScale(double extent, double radiiSum)
        {
            if (radiiSum > 0)
            {
                scale = Math.Min(scale, extent / radiiSum);
            }
        }
    }

    private float[] GetLightDirection()
    {
        double radians = Angle % 360 * Math.PI / 180;
        double normalization = Math.Sqrt(2);

        return
        [
            (float)(Math.Sin(radians) / normalization),
            (float)(-Math.Cos(radians) / normalization),
            (float)(1 / normalization)
        ];
    }

    private static double GetPixelScale(SKMatrix matrix)
    {
        double horizontalScale = Math.Sqrt(matrix.ScaleX * matrix.ScaleX + matrix.SkewY * matrix.SkewY);
        double verticalScale = Math.Sqrt(matrix.ScaleY * matrix.ScaleY + matrix.SkewX * matrix.SkewX);

        return Math.Max(0.001, Math.Min(horizontalScale, verticalScale));
    }
}
