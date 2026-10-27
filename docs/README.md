<div align="center">
  <img src="Presentique/Assets/PresentiqueIcon.png" alt="Presentique logo" width="128" />
</div>

# Presentique

**Presentique** is a WPF UI control library for building customizable Windows desktop applications with C# and .NET 10.

The library currently provides a custom window with integrated window controls and a reusable circular icon button. Controls use WPF dependency properties and default styles, so they can be configured directly in XAML.

> **Project status:** Early development. APIs and styles may change.

## Features

- **ThemedWindow** — custom window appearance with rounded corners, a configurable title bar, and minimize, maximize/restore, and close commands.
- **RoundButton** — circular button with a vector (`Geometry`) icon and animated hover feedback.
- **WPF-native customization** — dependency properties, control templates, and XAML styling.
- **Reusable default styles** — control styles are included in `Themes/Generic.xaml`.
- **Demo application** — a separate WPF project showing the controls in use.

## Requirements

- Windows (WPF is Windows-only)
- .NET 10 SDK
- A WPF project targeting `net10.0-windows`

## Getting started

Clone the repository:

```bash
git clone https://github.com/satisplexity/Presentique.git
```

Add a reference to `Presentique/Presentique.csproj` from your WPF application. For example, in your application's `.csproj`:

```xml
<ItemGroup>
  <ProjectReference Include="..\Presentique\Presentique.csproj" />
</ItemGroup>
```

Adjust the relative path to match your solution layout. The default control styles are supplied through `Themes/Generic.xaml`; they do not need to be copied into your application.

## Usage

### ThemedWindow

Use `ThemedWindow` as your window's root element:

```xml
<ui:ThemedWindow
    x:Class="MyApp.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:ui="clr-namespace:Presentique.Windows;assembly=Presentique"
    xmlns:buttons="clr-namespace:Presentique.Buttons;assembly=Presentique"
    Title="My Application"
    Width="1000"
    Height="650"
    TitleBarHeight="48"
    CornerRounding="12">

    <ui:ThemedWindow.TitleBarContent>
        <TextBlock
            Text="Custom title bar content"
            VerticalAlignment="Center"
            Margin="12,0" />
    </ui:ThemedWindow.TitleBarContent>

    <Grid>
        <buttons:RoundButton
            Height="48"
            HorizontalAlignment="Center"
            VerticalAlignment="Center"
            Icon="M 10,2 L 14,2 L 14,10 L 22,10 L 22,14 L 14,14 L 14,22 L 10,22 L 10,14 L 2,14 L 2,10 L 10,10 Z" />
    </Grid>

</ui:ThemedWindow>
```

The corresponding code-behind should inherit from `ThemedWindow`:

```csharp
using Presentique.Windows;

namespace MyApp;

public partial class MainWindow : ThemedWindow
{
    public MainWindow()
    {
        InitializeComponent();
    }
}
```

### Control properties

| Control | Property | Type | Description |
| --- | --- | --- | --- |
| `ThemedWindow` | `TitleBarHeight` | `double` | Height of the title bar (default: `48`). |
| `ThemedWindow` | `CornerRounding` | `CornerRadius` | Corner radii of the window (default: `20`). |
| `ThemedWindow` | `TitleBarContent` | `object` | Optional additional content in the title bar. |
| `RoundButton` | `Icon` | `Geometry` | Vector path rendered inside the button. |

`ThemedWindow` also supports familiar WPF `Window` properties, including `Title`, `Icon`, `Background`, `Foreground`, and `ResizeMode`.

## Build and run the demo

From the repository root:

```bash
dotnet build Presentique.slnx
dotnet run --project Presentique.Demo
```

The demo uses `ThemedWindow` and references the `Presentique` project directly.

## Repository structure

```text
Presentique/
├── Presentique/
│   ├── Buttons/
│   │   ├── RoundButton.cs
│   │   └── Styles/RoundButton.xaml
│   ├── Windows/
│   │   ├── ThemedWindow.cs
│   │   └── ThemedWindow.xaml
│   └── Themes/
│       └── Generic.xaml
├── Presentique.Demo/
└── Presentique.slnx
```

## Contributing

Ideas, bug reports, and pull requests are welcome through [GitHub Issues](https://github.com/satisplexity/Presentique/issues).

---

Built with C#, XAML, and WPF.
