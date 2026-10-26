using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace Presentique.Buttons;

public class RoundButton : Button
{
    public static readonly DependencyProperty IconProperty
        = DependencyProperty.Register(
            nameof(Icon),
            typeof(Geometry),
            typeof(RoundButton));

    public Geometry Icon
    {
        get => (Geometry)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
}
