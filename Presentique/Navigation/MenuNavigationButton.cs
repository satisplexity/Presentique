using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace Presentique.Navigation;

public class MenuNavigationButton : RadioButton
{
    static MenuNavigationButton()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(MenuNavigationButton),
            new FrameworkPropertyMetadata(typeof(MenuNavigationButton)));
    }

    public static readonly DependencyProperty IconProperty
        = DependencyProperty.Register(
            nameof(Icon),
            typeof(Geometry),
            typeof(MenuNavigationButton));

    public Geometry Icon
    {
        get => (Geometry)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly DependencyProperty CornerRoundingProperty
        = DependencyProperty.Register(
            nameof(CornerRounding),
            typeof(CornerRadius),
            typeof(MenuNavigationButton));

    public CornerRadius CornerRounding
    {
        get => (CornerRadius)GetValue(CornerRoundingProperty);
        set => SetValue(CornerRoundingProperty, value);
    }

    public static readonly DependencyProperty GlowBrushProperty
        = DependencyProperty.Register(
            nameof(GlowBrush),
            typeof(SolidColorBrush),
            typeof(MenuNavigationButton));

    public SolidColorBrush GlowBrush
    {
        get => (SolidColorBrush)GetValue(GlowBrushProperty);
        set => SetValue(GlowBrushProperty, value);
    }
}
