using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace FindModbus.UI.Converters;

public class ByteToBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is byte byteValue && parameter is string paramStr && byte.TryParse(paramStr, out byte paramValue))
        {
            return byteValue == paramValue;
        }
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // RadioButton 选中时返回对应的功能码值
        if (value is bool boolValue && boolValue && parameter is string paramStr && byte.TryParse(paramStr, out byte paramValue))
        {
            return paramValue;
        }
        // 取消选中时返回 UnsetValue，保持当前值不变
        return Avalonia.AvaloniaProperty.UnsetValue;
    }
}

