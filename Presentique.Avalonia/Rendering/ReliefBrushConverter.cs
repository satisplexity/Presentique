using Avalonia.Media;
using SkiaSharp;
using Avalonia;

namespace Presentique.Avalonia.Rendering;

internal static class ReliefBrushConverter
{
    public static SKShader Create(IBrush brush, Size size)
    {
        if (brush is ISolidColorBrush solid)
        {
            return SKShader.CreateColor(ToSkiaColor(solid.Color, brush.Opacity));
        }

        if (brush is not IGradientBrush gradient)
        {
            throw new NotSupportedException("ReliefSurface supports solid, linear, radial and conic brushes.");
        }

        IGradientStop[] stops = [.. gradient.GradientStops.OrderBy(stop => stop.Offset)];
        if (stops.Length == 0)
        {
            return SKShader.CreateColor(SKColors.Transparent);
        }

        SKColor[] colors = [.. stops.Select(stop => ToSkiaColor(stop.Color, brush.Opacity))];
        if (stops.Length == 1)
        {
            return SKShader.CreateColor(colors[0]);
        }

        float[] offsets = [.. stops.Select(stop => (float)Math.Clamp(stop.Offset, 0, 1))];
        SKShaderTileMode tileMode = ToTileMode(gradient.SpreadMethod);

        return gradient switch
        {
            ILinearGradientBrush linear => CreateLinearGradient(linear, size, colors, offsets, tileMode),
            IRadialGradientBrush radial => CreateRadialGradient(radial, size, colors, offsets, tileMode),
            IConicGradientBrush conic => CreateConicGradient(conic, size, colors, offsets, tileMode),

            _ => throw new NotSupportedException("Unsupported gradient brush.")
        };
    }

    private static SKShader CreateLinearGradient(
        ILinearGradientBrush brush,
        Size size, 
        SKColor[] colors, 
        float[] offsets,
        SKShaderTileMode tileMode)
    {
        SKPoint start = ToSkiaPoint(brush.StartPoint.ToPixels(size));
        SKPoint end = ToSkiaPoint(brush.EndPoint.ToPixels(size));
        
        SKShader shader = 
            start == end
            ? SKShader.CreateColor(colors[^1])
            : SKShader.CreateLinearGradient(start, end, colors, offsets, tileMode);

        return ApplyTransform(shader, brush, size, SKMatrix.Identity);
    }

    private static SKShader CreateRadialGradient(
        IRadialGradientBrush brush,
        Size size,
        SKColor[] colors,
        float[] offsets,
        SKShaderTileMode tileMode)
    {
        Point center = brush.Center.ToPixels(size);
        Point origin = brush.GradientOrigin.ToPixels(size);
        
        float radiusX = ToPixels(brush.RadiusX, size.Width);
        float radiusY = ToPixels(brush.RadiusY, size.Height);

        if (radiusX <= 0 || radiusY <= 0)
        {
            return SKShader.CreateColor(colors[^1]);
        }

        var relativeOrigin = new SKPoint(
            (float)(origin.X - center.X) / radiusX,
            (float)(origin.Y - center.Y) / radiusY);

        var shader = SKShader.CreateTwoPointConicalGradient(
            relativeOrigin, 0, SKPoint.Empty, 1, colors, offsets, tileMode);

        var geometryMatrix = new SKMatrix(
            radiusX, 0, (float)center.X,
            0, radiusY, (float)center.Y,
            0, 0, 1);

        return ApplyTransform(shader, brush, size, geometryMatrix);
    }

    private static SKShader CreateConicGradient(
        IConicGradientBrush brush,
        Size size,
        SKColor[] colors,
        float[] offsets,
        SKShaderTileMode tileMode)
    {
        SKPoint center = ToSkiaPoint(brush.Center.ToPixels(size));
        var shader = SKShader.CreateSweepGradient(center, colors, offsets, tileMode, 0, 360);
        var geometryMatrix = SKMatrix.CreateRotationDegrees((float)brush.Angle - 90, center.X, center.Y);

        return ApplyTransform(shader, brush, size, geometryMatrix);
    }

    private static SKShader ApplyTransform(SKShader shader, IBrush brush, Size size, SKMatrix geometryMatrix)
    {
        SKMatrix localMatrix = geometryMatrix;
        
        if (brush.Transform is { } transform)
        {
            Point pivot = brush.TransformOrigin.ToPixels(size);
            Matrix brushMatrix = Matrix.CreateTranslation(-pivot.X, -pivot.Y)
                * transform.Value
                * Matrix.CreateTranslation(pivot.X, pivot.Y);

            localMatrix = SKMatrix.Concat(ToSkiaMatrix(brushMatrix), geometryMatrix);
        }

        if (localMatrix.IsIdentity)
        {
            return shader;
        }

        using (shader)
        {
            return shader.WithLocalMatrix(localMatrix);
        }
    }

    private static SKShaderTileMode ToTileMode(GradientSpreadMethod spreadMethod) => spreadMethod switch
    {
        GradientSpreadMethod.Repeat => SKShaderTileMode.Repeat,
        GradientSpreadMethod.Reflect => SKShaderTileMode.Mirror,
        _ => SKShaderTileMode.Clamp
    };

    private static SKMatrix ToSkiaMatrix(Matrix matrix) => new(
        (float)matrix.M11, (float)matrix.M21, (float)matrix.M31,
        (float)matrix.M12, (float)matrix.M22, (float)matrix.M32,
        0, 0, 1);

    private static SKPoint ToSkiaPoint(Point point) => new((float)point.X, (float)point.Y);

    private static float ToPixels(RelativeScalar value, double extent) =>
        (float)(value.Scalar * (value.Unit == RelativeUnit.Relative ? extent : 1));

    private static SKColor ToSkiaColor(Color color, double opacity) => new(
        color.R, color.G, color.B, (byte)Math.Round(color.A * Math.Clamp(opacity, 0, 1)));
}
