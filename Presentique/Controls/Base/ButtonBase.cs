using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace Presentique.Controls.Base;

/// <summary>
/// Provides a base class for buttons that support a geometry-based icon.
/// </summary>
public abstract class ButtonBase : Button
{
    /// <summary>
    /// Identifies the <see cref="Icon"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IconProperty
        = DependencyProperty.Register(
            nameof(Icon),
            typeof(Geometry),
            typeof(ButtonBase));

    /// <summary>
    /// Gets or sets the geometry used to display the button's icon.
    /// </summary>
    public Geometry Icon
    {
        get => (Geometry)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
}
