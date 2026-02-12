using Avalonia.Data.Converters;
using FindModbus.Core.Models;
using System;
using System.Globalization;

namespace FindModbus.UI.Converters;

public class ParityToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Parity parity)
        {
            return parity switch
            {
                Parity.None => "无校验",
                Parity.Even => "偶校验",
                Parity.Odd => "奇校验",
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
                "无校验" => Parity.None,
                "偶校验" => Parity.Even,
                "奇校验" => Parity.Odd,
                _ => Parity.None
            };
        }
        return Parity.None;
    }
}

