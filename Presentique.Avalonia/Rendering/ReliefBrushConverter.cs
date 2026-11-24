using Avalonia;
using Avalonia.Media;
using SkiaSharp;

namespace Presentique.Avalonia.Rendering;

internal static class ReliefBrushConverter
{
    public static SKShader Create(IBrush brush, Size size)
    {
        if (brush is ISolidColorBrush solid)
        {
            return SKShader.CreateColor(Color(solid.Color, brush.Opacity));
        }

        if (brush is not IGradientBrush gradient)
        {
            throw new NotSupportedException("ReliefSurface supports solid, linear, radial and conic brushes.");
        }

        IGradientStop[] stops = gradient.GradientStops.OrderBy(s => s.Offset).ToArray();
        
        if (stops.Length == 0)
        {
            return SKShader.CreateColor(SKColors.Transparent);
        }

        SKColor[] colors = stops.Select(s => Color(s.Color, brush.Opacity)).ToArray();

        if (stops.Length == 1)
        {
            return SKShader.CreateColor(colors[0]);
        }

        float[] offsets = stops.Select(s => (float)Math.Clamp(s.Offset, 0, 1)).ToArray();

        SKShaderTileMode tile = gradient.SpreadMethod switch
        {
            GradientSpreadMethod.Repeat => SKShaderTileMode.Repeat,
            GradientSpreadMethod.Reflect => SKShaderTileMode.Mirror,
            _ => SKShaderTileMode.Clamp
        };

        SKShader shader;
        SKMatrix geometryMatrix = SKMatrix.Identity;

        switch (gradient)
        {
            case ILinearGradientBrush linear:
                SKPoint start = Point(linear.StartPoint.ToPixels(size));
                SKPoint end = Point(linear.EndPoint.ToPixels(size));

                shader = start == end 
                    ? SKShader.CreateColor(colors[^1])
                    : SKShader.CreateLinearGradient(start, end, colors, offsets, tile);

                break;

            case IRadialGradientBrush radial:

                Point center = radial.Center.ToPixels(size);
                Point origin = radial.GradientOrigin.ToPixels(size);

                float rx = Scalar(radial.RadiusX, size.Width);
                float ry = Scalar(radial.RadiusY, size.Height);

                if (rx <= 0 || ry <= 0)
                {
                    return SKShader.CreateColor(colors[^1]);
                }

                shader = SKShader.CreateTwoPointConicalGradient(
                    new SKPoint(
                        (float)(origin.X - center.X) / rx,
                        (float)(origin.Y - center.Y) / ry),
                    0,
                    SKPoint.Empty, 1,
                    colors,
                    offsets,
                    tile);

                geometryMatrix = new SKMatrix(rx, 0, (float)center.X, 0, ry, (float)center.Y, 0, 0, 1);

                break;

            case IConicGradientBrush conic:

                SKPoint c = Point(conic.Center.ToPixels(size));

                shader = SKShader.CreateSweepGradient(c, colors, offsets, tile, 0, 360);
                geometryMatrix = SKMatrix.CreateRotationDegrees((float)conic.Angle - 90, c.X, c.Y);

                break;

            default:
                throw new NotSupportedException("Unsupported gradient brush.");
        }

        if (brush.Transform is not { } transform)
        {
            if (geometryMatrix.IsIdentity)
            {
                return shader;
            }

            using (shader)
            {
                return shader.WithLocalMatrix(geometryMatrix);
            }
        }

        Point pivot = brush.TransformOrigin.ToPixels(size);
        Matrix matrix = Matrix.CreateTranslation(-pivot.X, -pivot.Y) 
            * transform.Value 
            * Matrix.CreateTranslation(pivot.X, pivot.Y);

        SKMatrix combined = SKMatrix.Concat(
            new SKMatrix(
                (float)matrix.M11, (float)matrix.M21, (float)matrix.M31,
                (float)matrix.M12, (float)matrix.M22, (float)matrix.M32, 0, 0, 1),
            geometryMatrix);
        
        if (combined.IsIdentity)
        {
            return shader;
        }

        using (shader)
        {
            return shader.WithLocalMatrix(combined);
        }
    }

    private static SKPoint Point(Point p) => new((float)p.X, (float)p.Y);

    private static float Scalar(RelativeScalar value, double extent) =>
        (float)(value.Scalar * (value.Unit == RelativeUnit.Relative ? extent : 1));

    private static SKColor Color(Color c, double opacity) => new(c.R, c.G, c.B,
        (byte)Math.Round(c.A * Math.Clamp(opacity, 0, 1)));
}
