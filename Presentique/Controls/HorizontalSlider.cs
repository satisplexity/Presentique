using System.Windows.Controls;
using System.Windows;

namespace Presentique.Controls;

public class HorizontalSlider : Slider
{
    public static readonly DependencyProperty TitleProperty
        = DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(HorizontalSlider));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty RoundingProperty
        = DependencyProperty.Register(
            nameof(Rounding),
            typeof(CornerRadius),
            typeof(HorizontalSlider));

    public CornerRadius Rounding
    {
        get => (CornerRadius)GetValue(RoundingProperty);
        set => SetValue(RoundingProperty, value);
    }

    public static readonly DependencyProperty ValueSuffixProperty
        = DependencyProperty.Register(
            nameof(ValueSuffix),
            typeof(string),
            typeof(HorizontalSlider));

    public string ValueSuffix
    {
        get => (string)GetValue(ValueSuffixProperty);
        set => SetValue(ValueSuffixProperty, value);
    }

    public static readonly DependencyProperty ValuePrefixProperty
        = DependencyProperty.Register(
            nameof(ValuePrefix),
            typeof(string),
            typeof(HorizontalSlider));

    public string ValuePrefix
    {
        get => (string)GetValue(ValuePrefixProperty);
        set => SetValue(ValuePrefixProperty, value);
    }

    private TextBlock? _valueText;

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _valueText = (TextBlock)GetTemplateChild("PART_ValueText");
    }

    protected override void OnValueChanged(double oldValue, double newValue)
    {
        base.OnValueChanged(oldValue, newValue);

        if(_valueText is not null)
        {
            _valueText.Text = $"{ValuePrefix}{newValue}{ValueSuffix}";
        }
    }
}