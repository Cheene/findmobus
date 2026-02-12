# ComboBox 测试说明

## 当前实现

所有下拉框都使用标准的 Avalonia ComboBox 实现：

1. **功能码下拉框**
   - ItemsSource: `FunctionCodeOptions` (ObservableCollection<byte>)
   - SelectedItem: `SelectedFunctionCode` (byte, Mode=TwoWay)
   - 使用 ItemTemplate 显示友好文本

2. **波特率下拉框**
   - ItemsSource: `BaudRateOptions` (ObservableCollection<int>)
   - SelectedItem: `BaudRate` (int, Mode=TwoWay)

3. **校验位下拉框**
   - ItemsSource: `ParityOptions` (ObservableCollection<Parity>)
   - SelectedItem: `SelectedParity` (Parity, Mode=TwoWay)
   - 使用 ItemTemplate 显示中文

4. **停止位下拉框**
   - ItemsSource: `StopBitsOptions` (ObservableCollection<StopBits>)
   - SelectedItem: `SelectedStopBits` (StopBits, Mode=TwoWay)
   - 使用 ItemTemplate 显示中文

## 如果下拉框无法选择

请检查：
1. 应用是否正常启动
2. 是否有运行时错误
3. 控制台是否有错误信息

## 标准实现

所有实现都遵循 Avalonia 标准：
- 使用 ObservableCollection 作为数据源
- 使用 SelectedItem 双向绑定
- 使用 ItemTemplate 自定义显示

