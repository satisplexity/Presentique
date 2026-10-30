using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace Presentique.Controls.Base;

public class ButtonBase : Button
{
    public static readonly DependencyProperty IconProperty
        = DependencyProperty.Register(
            nameof(Icon),
            typeof(Geometry),
            typeof(ButtonBase));

    public Geometry Icon
    {
        get => (Geometry)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
}
