using Avalonia;
using Avalonia.Media;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SkiaSharp;

namespace Presentique.Avalonia.Rendering;

internal sealed class ReliefDrawOperation(Rect bounds, IBrush background, CornerRadius radii,
    double smoothing, double depth, double strength, double angle) : ICustomDrawOperation
{
    private static readonly Lazy<SKRuntimeEffect> Effect = new(() =>
        SKRuntimeEffect.CreateShader(Source, out var errors)
        ?? throw new InvalidOperationException($"Relief shader compilation failed: {errors}"));

    public Rect Bounds { get; } = bounds;
    public bool HitTest(Point p) => false;
    public bool Equals(ICustomDrawOperation? other) => other is ReliefDrawOperation op &&
        Bounds == op.Bounds && background.Equals(op.Background) && radii == op.Radii &&
        smoothing == op.Smoothing && depth == op.Depth && strength == op.Strength && angle == op.Angle;
    private IBrush Background => background;
    private CornerRadius Radii => radii;
    private double Smoothing => smoothing;
    private double Depth => depth;
    private double Strength => strength;
    private double Angle => angle;
    public void Dispose() { }

    public void Render(ImmediateDrawingContext context)
    {
        var feature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>()
            ?? throw new NotSupportedException("ReliefSurface requires the Avalonia Skia renderer.");
        using var lease = feature.Lease();
        Draw(lease.SkCanvas, (float)lease.CurrentOpacity);
    }

    internal void Draw(SKCanvas canvas, float opacity = 1)
    {
        var size = Bounds.Size;
        var r = new[] { radii.TopLeft, radii.TopRight, radii.BottomRight, radii.BottomLeft };
        // Scale all corners together so adjacent corner regions never overlap.
        double scale = 1;
        void Limit(double extent, double sum) { if (sum > 0) scale = Math.Min(scale, extent / sum); }
        Limit(size.Width, r[0] + r[1]); Limit(size.Width, r[3] + r[2]);
        Limit(size.Height, r[0] + r[3]); Limit(size.Height, r[1] + r[2]);
        var matrix = canvas.TotalMatrix;
        var pixelScale = Math.Max(0.001, Math.Min(
            Math.Sqrt(matrix.ScaleX * matrix.ScaleX + matrix.SkewY * matrix.SkewY),
            Math.Sqrt(matrix.ScaleY * matrix.ScaleY + matrix.SkewX * matrix.SkewX)));
        var radians = angle % 360 * Math.PI / 180;
        var effect = Effect.Value;
        using var fill = ReliefBrushConverter.Create(background, size);
        var uniforms = new SKRuntimeEffectUniforms(effect)
        {
            ["size"] = new[] { (float)size.Width, (float)size.Height },
            ["radii"] = r.Select(v => (float)(v * scale)).ToArray(),
            ["exponent"] = (float)(2 + smoothing),
            ["depth"] = (float)Math.Clamp(depth, -Math.Min(size.Width, size.Height) / 2, Math.Min(size.Width, size.Height) / 2),
            ["strength"] = (float)Math.Min(strength, 100),
            ["light"] = new[] { (float)(Math.Sin(radians) / Math.Sqrt(2)), (float)(-Math.Cos(radians) / Math.Sqrt(2)), (float)(1 / Math.Sqrt(2)) },
            ["aa"] = (float)(0.75 / pixelScale),
            ["opacity"] = opacity
        };
        var children = new SKRuntimeEffectChildren(effect) { ["background"] = fill };
        using var shader = effect.ToShader(uniforms, children);
        using var paint = new SKPaint { Shader = shader, IsAntialias = true };
        // Coverage is evaluated in the shader, including the straight edges.
        canvas.DrawRect(new SKRect(0, 0, (float)size.Width, (float)size.Height), paint);
    }

    internal const string Source = """
        uniform shader background;
        uniform float2 size;
        uniform float4 radii;
        uniform float exponent;
        uniform float depth;
        uniform float strength;
        uniform float3 light;
        uniform float aa;
        uniform float opacity;

        // First-order signed distance to a superellipse in a corner's local coordinates.
        float corner(float2 q, float r) {
            if (r <= 0 || q.x <= 0 || q.y <= 0) return -1000000;
            float2 v = max(q / r, float2(0.000001));
            float sum = pow(v.x, exponent) + pow(v.y, exponent);
            float norm = pow(sum, 1.0 / exponent);
            float2 g = pow(v, float2(exponent - 1)) * pow(sum, 1.0 / exponent - 1);
            return (norm - 1) * r / max(length(g), 0.00001);
        }
        float distanceToEdge(float2 p) {
            float d = max(max(-p.x, p.x - size.x), max(-p.y, p.y - size.y));
            d = max(d, corner(float2(radii.x) - p, radii.x));
            d = max(d, corner(float2(p.x - size.x + radii.y, radii.y - p.y), radii.y));
            d = max(d, corner(p - size + float2(radii.z), radii.z));
            d = max(d, corner(float2(radii.w - p.x, p.y - size.y + radii.w), radii.w));
            return d;
        }
        half4 main(float2 p) {
            float d = distanceToEdge(p);
            float coverage = 1 - smoothstep(-aa, aa, d);
            if (coverage <= 0) return half4(0);
            half4 sampleColor = background.eval(p);
            float alpha = float(sampleColor.a);
            float3 color = float3(sampleColor.rgb) / max(alpha, 0.00001);
            float width = abs(depth);
            if (width > 0 && d > -width && strength > 0) {
                float epsilon = max(aa * 0.25, 0.001);
                float2 gradient = float2(
                    distanceToEdge(p + float2(epsilon, 0)) - distanceToEdge(p - float2(epsilon, 0)),
                    distanceToEdge(p + float2(0, epsilon)) - distanceToEdge(p - float2(0, epsilon)));
                float2 outward = gradient / max(length(gradient), 0.00001);
                float t = clamp(1 + d / width, 0, 1);
                float slope = t * 1.570796327;
                float3 normal = float3(outward * sin(slope) * sign(depth), cos(slope));
                // Lambert diffuse plus a small dielectric specular lobe, relative to the flat face.
                float diffuse = max(dot(normal, light), 0);
                float3 halfway = normalize(light + float3(0, 0, 1));
                float specular = pow(max(dot(normal, halfway), 0), 32);
                float flatSpecular = pow(halfway.z, 32);
                float3 linearColor = pow(max(color, float3(0)), float3(2.2));
                float illumination = (0.35 + strength * diffuse) / (0.35 + strength * light.z);
                linearColor = linearColor * illumination
                    + float3(0.08 * strength / (1 + strength) * (specular - flatSpecular));
                color = pow(clamp(linearColor, float3(0), float3(1)), float3(1.0 / 2.2));
            }
            return half4(color * alpha, alpha) * half(coverage * opacity);
        }
        """;
}
