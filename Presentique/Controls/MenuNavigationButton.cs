using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace Presentique.Controls;

/// <summary>
/// Represents a navigation button with an icon and configurable corner rounding.
/// </summary>
public class MenuNavigationButton : RadioButton
{
    static MenuNavigationButton()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(MenuNavigationButton),
            new FrameworkPropertyMetadata(typeof(MenuNavigationButton)));
    }

    /// <summary>
    /// Identifies the <see cref="Icon"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IconProperty
        = DependencyProperty.Register(
            nameof(Icon),
            typeof(Geometry),
            typeof(MenuNavigationButton),
            new PropertyMetadata(Geometry.Empty));

    /// <summary>
    /// Gets or sets the geometry used to display the navigation button's icon.
    /// </summary>
    public Geometry Icon
    {
        get => (Geometry)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="CornerRounding"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CornerRoundingProperty
        = DependencyProperty.Register(
            nameof(CornerRounding),
            typeof(CornerRadius),
            typeof(MenuNavigationButton));

    /// <summary>
    /// Gets or sets the radius of the navigation button's corners.
    /// </summary>
    public CornerRadius CornerRounding
    {
        get => (CornerRadius)GetValue(CornerRoundingProperty);
        set => SetValue(CornerRoundingProperty, value);
    }
}
