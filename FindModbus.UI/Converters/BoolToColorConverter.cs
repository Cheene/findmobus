using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace FindModbus.UI.Converters;

public class BoolToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue && parameter is string param)
        {
            var parts = param.Split('|');
            if (parts.Length == 2)
            {
                var colorString = boolValue ? parts[0] : parts[1];
                if (Color.TryParse(colorString, out var color))
                {
                    return new SolidColorBrush(color);
                }
            }
        }
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

