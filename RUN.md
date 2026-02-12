# 运行说明

## 当前状态

项目正在修复编译错误。主要问题是 NModbus 3.0 API 的使用方式需要调整。

## 快速运行步骤

一旦编译成功，可以按以下步骤运行：

```bash
# 1. 恢复依赖
dotnet restore

# 2. 构建项目
dotnet build

# 3. 运行应用
dotnet run --project FindModbus.UI
```

## 已知问题

1. NModbus 3.0 API 与代码中的使用方式不匹配
2. 需要正确实现 SerialPort 到 IStreamResource 的适配

## 临时解决方案

如果编译仍有问题，可以考虑：
1. 降级到 NModbus 2.x 版本
2. 或者使用其他 Modbus 库（如 EasyModbus）

## 下一步

修复编译错误后，项目应该可以正常运行。

