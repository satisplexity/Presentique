using Avalonia.Controls;
using Avalonia.Media;
using Avalonia;

namespace Presentique.Avalonia.Controls;

public class ActionButton : Button
{
    public static readonly StyledProperty<StreamGeometry> IconProperty =
        AvaloniaProperty.Register<ActionButton, StreamGeometry>(nameof(Icon));

    public StreamGeometry Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
}
