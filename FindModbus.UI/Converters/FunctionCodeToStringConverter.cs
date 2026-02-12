using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace FindModbus.UI.Converters;

public class FunctionCodeToStringConverter : IValueConverter
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
        if (value is string str)
        {
            // 从字符串中提取功能码数字
            if (str.StartsWith("01")) return (byte)1;
            if (str.StartsWith("02")) return (byte)2;
            if (str.StartsWith("03")) return (byte)3;
            if (str.StartsWith("04")) return (byte)4;
        }
        return (byte)3; // 默认值
    }
}

