using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace Presentique.Navigation;

public class MenuNavigationButton : RadioButton
{
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

    public static readonly DependencyProperty TextProperty
        = DependencyProperty.Register(
            nameof(Text),
            typeof(string),
            typeof(MenuNavigationButton));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly DependencyProperty IsTextCollapsedProperty
        = DependencyProperty.Register(
            nameof(IsTextCollapsed),
            typeof(bool),
            typeof(MenuNavigationButton));

    public bool IsTextCollapsed
    {
        get => (bool)GetValue(IsTextCollapsedProperty);
        set => SetValue(IsTextCollapsedProperty, value);
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
