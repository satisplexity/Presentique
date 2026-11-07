using Avalonia.Controls;
using Avalonia.Media;
using Avalonia;

namespace Presentique.Avalonia.Controls;

public class ActionButton : Button
{
    public static readonly StyledProperty<Geometry> IconProperty =
        AvaloniaProperty.Register<ActionButton, Geometry>(nameof(Icon));

    public Geometry Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
}
