using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace Presentique.Controls;

public class NavigationButton : RadioButton
{
    public static readonly DependencyProperty IconProperty
        = DependencyProperty.Register(
            nameof(Icon),
            typeof(Geometry),
            typeof(NavigationButton));

    public Geometry Icon
    {
        get => (Geometry)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly DependencyProperty TextProperty
        = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(NavigationButton));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty RoundingProperty
        = DependencyProperty.Register(
            nameof(Rounding),
            typeof(CornerRadius),
            typeof(NavigationButton));

    public CornerRadius Rounding
    {
        get => (CornerRadius)GetValue(RoundingProperty);
        set => SetValue(RoundingProperty, value);
    }
}