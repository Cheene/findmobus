using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace FindModbus.UI.Converters;

public class FunctionCodeDisplayConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is byte code)
        {
            return code switch
            {
                1 => "01 - 读取线圈状态",
                2 => "02 - 读取输入状态",
                3 => "03 - 读取保持寄存器",
                4 => "04 - 读取输入寄存器",
                _ => $"功能码 {code}"
            };
        }
        return value?.ToString() ?? "";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

