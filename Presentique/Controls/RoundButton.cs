using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace Presentique.Controls;

/// <summary>
/// Represents a button with a circular appearance and a customizable vector icon.
/// </summary>
public class RoundButton : Button
{
    /// <summary>
    /// Initializes the default style metadata for the
    /// <see cref="RoundButton"/> control.
    /// </summary>
    static RoundButton()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(RoundButton),
            new FrameworkPropertyMetadata(typeof(RoundButton)));
    }

    /// <summary>
    /// Identifies the <see cref="Icon"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IconProperty
        = DependencyProperty.Register(
            nameof(Icon),
            typeof(Geometry),
            typeof(RoundButton),
            new PropertyMetadata(Geometry.Empty));

    /// <summary>
    /// Gets or sets the geometry used to render the button's icon.
    /// </summary>
    /// <value>
    /// A <see cref="Geometry"/> representing the icon.
    /// The default value is <see cref="Geometry.Empty"/>.
    /// </value>
    public Geometry Icon
    {
        get => (Geometry)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
}
