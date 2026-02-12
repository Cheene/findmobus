using Avalonia.Data.Converters;
using FindModbus.Core.Models;
using System;
using System.Globalization;

namespace FindModbus.UI.Converters;

public class StopBitsToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is StopBits stopBits)
        {
            return stopBits switch
            {
                StopBits.One => "1位",
                StopBits.Two => "2位",
                _ => value?.ToString() ?? ""
            };
        }
        return value?.ToString() ?? "";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string str)
        {
            return str switch
            {
                "1位" => StopBits.One,
                "2位" => StopBits.Two,
                _ => StopBits.One
            };
        }
        return StopBits.One;
    }
}

