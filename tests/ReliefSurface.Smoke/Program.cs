using Avalonia;
using Avalonia.Media;
using Presentique.Avalonia.Rendering;
using SkiaSharp;

static void Check(bool condition, string message)
{
    if (!condition) throw new Exception(message);
}
static SKBitmap Render(IBrush brush, double depth = 4, double strength = 1, double angle = 0,
    CornerRadius? corners = null, double smoothing = 2, float opacity = 1)
{
    var bitmap = new SKBitmap(200, 120);
    using var canvas = new SKCanvas(bitmap);
    canvas.Clear(SKColors.Transparent);
    new ReliefDrawOperation(new Rect(0, 0, 200, 120), brush.ToImmutable(), corners ?? new CornerRadius(24),
        smoothing, depth, strength, angle).Draw(canvas, opacity);
    return bitmap;
}
static double Brightness(SKColor c) => c.Red + c.Green + c.Blue;
var solid = new SolidColorBrush(Avalonia.Media.Color.Parse("#808080"));
using var raised = Render(solid);
using var inset = Render(solid, -4);
Check(raised.GetPixel(0, 0).Alpha == 0, "Corners must be transparent.");
Check(raised.GetPixel(100, 60) == new SKColor(128, 128, 128), "Flat face must preserve background.");
Check(Brightness(raised.GetPixel(100, 1)) > Brightness(raised.GetPixel(100, 118)), "Top light must illuminate top bevel.");
Check(Brightness(inset.GetPixel(100, 1)) < Brightness(inset.GetPixel(100, 118)), "Negative depth must reverse normals.");
using var flat = Render(solid, 0);
using var unlit = Render(solid, 4, 0);
Check(flat.GetPixel(100, 1) == unlit.GetPixel(100, 1), "Zero light and zero depth must leave the background unchanged.");
using var transparent = Render(new SolidColorBrush(Avalonia.Media.Color.Parse("#80808080")), opacity: 0.5f);
Check(Math.Abs(transparent.GetPixel(100, 60).Alpha - 64) <= 1, "Brush and control alpha must multiply.");
using var circular = Render(solid, corners: new CornerRadius(24), smoothing: 0);
Check(raised.GetPixel(4, 4).Alpha > circular.GetPixel(4, 4).Alpha, "Smoothing must change the superellipse contour.");
using var mixed = Render(solid, corners: new CornerRadius(0, 24, 40, 12));
Check(mixed.GetPixel(1, 1).Alpha > 200 && mixed.GetPixel(198, 1).Alpha == 0, "Corner radii must remain independent.");
var stops = new GradientStops { new(Colors.Red, 0), new(Colors.Blue, 1) };
IBrush[] brushes = [
    new LinearGradientBrush { StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative), EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative), GradientStops = stops },
    new RadialGradientBrush { Center = new RelativePoint(0, 0, RelativeUnit.Relative), GradientOrigin = new RelativePoint(0, 0, RelativeUnit.Relative), RadiusX = new RelativeScalar(1, RelativeUnit.Relative), RadiusY = new RelativeScalar(1, RelativeUnit.Relative), GradientStops = stops },
    new ConicGradientBrush { GradientStops = stops }
];
foreach (var brush in brushes)
{
    using var bitmap = Render(brush);
    Check(bitmap.GetPixel(100, 60).Alpha == 255, $"{brush.GetType().Name} must render.");
    Check(bitmap.GetPixel(25, 60) != bitmap.GetPixel(175, 60), $"{brush.GetType().Name} must vary spatially.");
}
using var radial = Render(brushes[1]);
Check(radial.GetPixel(25, 1).Red > radial.GetPixel(25, 1).Blue, "Radial bevel near origin must use red background.");
Check(radial.GetPixel(175, 1).Blue > radial.GetPixel(175, 1).Red, "Radial bevel far from origin must use blue background.");
var identityBrush = new LinearGradientBrush
{
    StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
    EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative),
    GradientStops = stops, Transform = new MatrixTransform(Matrix.Identity)
};
using var identity = Render(identityBrush, 0);
using var original = Render(brushes[0], 0);
Check(identity.GetPixel(100, 60) == original.GetPixel(100, 60), "Identity brush transform must preserve the shader.");
var shiftedRadial = new RadialGradientBrush
{
    Center = new RelativePoint(0, 0, RelativeUnit.Relative),
    GradientOrigin = new RelativePoint(0, 0, RelativeUnit.Relative),
    RadiusX = new RelativeScalar(1, RelativeUnit.Relative),
    RadiusY = new RelativeScalar(1, RelativeUnit.Relative), GradientStops = stops,
    Transform = new TranslateTransform(20, 0)
};
using var shifted = Render(shiftedRadial, 0);
using var radialFlat = Render(brushes[1], 0);
Check(shifted.GetPixel(120, 60) == radialFlat.GetPixel(100, 60), "Brush transform must compose with radial ellipse geometry.");
using var rightLight = Render(solid, angle: 90);
Check(Brightness(rightLight.GetPixel(198, 60)) > Brightness(rightLight.GetPixel(1, 60)), "Ninety degrees must light the right edge.");
using var oversized = Render(solid, corners: new CornerRadius(1000, 2000, 3000, 4000));
Check(oversized.GetPixel(100, 60).Alpha == 255, "Oversized corner radii must normalize without removing the face.");
using var montage = new SKBitmap(600, 240);
using (var canvas = new SKCanvas(montage))
{
    canvas.Clear(new SKColor(25, 25, 25));
    for (int i = 0; i < 3; i++)
    {
        using var top = Render(brushes[i]);
        using var bottom = Render(brushes[i], -4);
        canvas.DrawBitmap(top, i * 200, 0);
        canvas.DrawBitmap(bottom, i * 200, 120);
    }
}
if (args.Length > 0)
{
    using var image = SKImage.FromBitmap(montage);
    using var data = image.Encode(SKEncodedImageFormat.Png, 100);
    using var stream = File.Create(args[0]);
    data.SaveTo(stream);
}
Console.WriteLine("PASS: runtime shader compilation, three gradients, local bevel colors, lighting, inset, alpha, corners and smoothing.");
