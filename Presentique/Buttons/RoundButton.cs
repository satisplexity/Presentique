using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace Presentique.Buttons;

public class RoundButton : Button
{
    static RoundButton()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(RoundButton),
            new FrameworkPropertyMetadata(typeof(RoundButton)));
    }

    public static readonly DependencyProperty IconProperty
        = DependencyProperty.Register(
            nameof(Icon),
            typeof(Geometry),
            typeof(RoundButton),
            new PropertyMetadata(Geometry.Empty));

    public Geometry Icon
    {
        get => (Geometry)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
}
