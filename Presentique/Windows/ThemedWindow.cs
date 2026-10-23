using System.Windows.Shell;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows;

namespace Presentique.Windows;

/// <summary>
/// Represents a customizable window with built-in commands for
/// minimizing, maximizing, restoring, and closing.
/// </summary>
public class ThemedWindow : Window
{
    /// <summary>
    /// Identifies the <see cref="CornerRounding"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CornerRoundingProperty
        = DependencyProperty.Register(
            nameof(CornerRounding),
            typeof(CornerRadius),
            typeof(ThemedWindow),
            new PropertyMetadata(new CornerRadius(16)));

    /// <summary>
    /// Gets or sets the corner radius of the window.
    /// </summary>
    /// <value>
    /// The height of the title bar, in device-independent units.
    /// <para>
    /// The default value is 16.
    /// </para>
    /// </value>
    public CornerRadius CornerRounding
    {
        get => (CornerRadius)GetValue(CornerRoundingProperty);
        set => SetValue(CornerRoundingProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="TitleBarHeight"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TitleBarHeightProperty
        = DependencyProperty.Register(
            nameof(TitleBarHeight),
            typeof(double),
            typeof(ThemedWindow),
            new PropertyMetadata(48.0));

    /// <summary>
    /// Gets or sets the height of the window's title bar.
    /// </summary>
    /// <value>
    /// The height of the title bar, in device-independent units.
    /// <para>
    /// The default value is 48.
    /// </para>
    /// </value>
    public double TitleBarHeight
    {
        get => (double)GetValue(TitleBarHeightProperty);
        set => SetValue(TitleBarHeightProperty, value);
    }

    /// <summary>
    /// Represents the command used to minimize the window.
    /// </summary>
    public static readonly RoutedUICommand MinimizeCommand
        = new("Minimize",
            nameof(MinimizeCommand),
            typeof(ThemedWindow));

    /// <summary>
    /// Represents the command used to toggle the window between
    /// maximized and normal states.
    /// </summary>
    public static readonly RoutedUICommand ToggleMaximizeCommand =
        new("ToggleMaximize",
            nameof(ToggleMaximizeCommand),
            typeof(ThemedWindow));

    /// <summary>
    /// Represents the command used to close the window.
    /// </summary>
    public static readonly RoutedUICommand CloseCommand =
        new("Close",
            nameof(CloseCommand),
            typeof(ThemedWindow));

    /// <summary>
    /// Initializes static members of the <see cref="ThemedWindow"/> class.
    /// </summary>
    static ThemedWindow()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(ThemedWindow),
            new FrameworkPropertyMetadata(typeof(ThemedWindow)));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemedWindow"/> class
    /// and registers command bindings for window management.
    /// </summary>
    public ThemedWindow()
    {
        BindMinimize();
        BindMaximize();
        BindClose();

        InitializeChrome();
    }

    private void BindMinimize()
    {
        CommandBindings.Add(
            new CommandBinding(
                MinimizeCommand,
                (_, _) => SystemCommands.MinimizeWindow(this)));
    }

    private void BindMaximize()
    {
        CommandBindings.Add(
            new CommandBinding(
                ToggleMaximizeCommand,
                (_, _) =>
                {
                    if (WindowState == WindowState.Maximized)
                    {
                        SystemCommands.RestoreWindow(this);
                    }
                    else
                    {
                        SystemCommands.MaximizeWindow(this);
                    }
                },
                (_, e) =>
                {
                    e.CanExecute = ResizeMode is
                        ResizeMode.CanResize or
                        ResizeMode.CanResizeWithGrip;
                    e.Handled = true;
                }));
    }

    private void BindClose()
    {
        CommandBindings.Add(
            new CommandBinding(
                CloseCommand,
                (_, _) => SystemCommands.CloseWindow(this)));
    }

    private void InitializeChrome()
    {
        CommandBindings.Add(
            new CommandBinding(
                CloseCommand,
                (_, _) => SystemCommands.CloseWindow(this)));

        var chrome = new WindowChrome()
        {
            ResizeBorderThickness = new(8),
            UseAeroCaptionButtons = false,
            GlassFrameThickness = new(0)
        };

        BindingOperations.SetBinding(
            chrome,
            WindowChrome.CaptionHeightProperty,
            new Binding(nameof(TitleBarHeight))
            {
                Source = this
            });

        BindingOperations.SetBinding(
            chrome,
            WindowChrome.CornerRadiusProperty,
            new Binding(nameof(CornerRounding))
            {
                Source = this
            });

        WindowChrome.SetWindowChrome(this, chrome);
    }
}
