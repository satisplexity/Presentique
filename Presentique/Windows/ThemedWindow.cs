using System.Windows.Input;
using System.Windows;

namespace Presentique.Windows;

/// <summary>
/// Represents a customizable window with built-in commands for
/// minimizing, maximizing, restoring, and closing.
/// </summary>
public class ThemedWindow : Window
{
    /// <summary>
    /// Represents the command used to minimize the window.
    /// </summary>
    public static readonly RoutedUICommand MinimizeCommand =
        new("Minimize",
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
        CommandBindings.Add(
            new CommandBinding(
                MinimizeCommand,
                (_, _) => SystemCommands.MinimizeWindow(this)));

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

        CommandBindings.Add(
            new CommandBinding(
                CloseCommand,
                (_, _) => SystemCommands.CloseWindow(this)));
    }
}
