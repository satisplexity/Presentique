using SkiaSharp;

namespace Presentique.Avalonia.Rendering;

internal static class ReliefShader
{
    private const string ResourceName = "Presentique.Avalonia.Rendering.Relief.sksl";
    private static readonly Lazy<SKRuntimeEffect> CompiledEffect = new(Compile);

    public static SKRuntimeEffect Effect => CompiledEffect.Value;

    private static SKRuntimeEffect Compile()
    {
        using Stream stream = 
            typeof(ReliefShader).Assembly.GetManifestResourceStream(ResourceName)
            ?? throw new InvalidOperationException($"Relief shader resource '{ResourceName}' was not found.");

        using var reader = new StreamReader(stream);

        string source = reader.ReadToEnd();

        return
            SKRuntimeEffect.CreateShader(source, out var errors)
            ?? throw new InvalidOperationException($"Relief shader compilation failed: {errors}");
    }
}
