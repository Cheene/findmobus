# Modbus 智能扫描工具

## 项目简介

Modbus 地址与参数智能扫描工具，用于自动识别未知 Modbus 设备的通讯参数和寄存器地址。

## 技术栈

- **.NET 8.0**
- **Avalonia UI 11.0** - 跨平台 UI 框架
- **NModbus4** - Modbus 协议库
- **CommunityToolkit.Mvvm** - MVVM 工具包

## 项目结构

```
FindModbus/
├── FindModbus.Core/          # 核心业务逻辑
│   ├── Models/               # 数据模型
│   └── Services/             # 业务服务
├── FindModbus.Protocol/       # Modbus 协议层
│   ├── ModbusTcp/            # TCP 实现
│   └── ModbusRtu/            # RTU 实现
├── FindModbus.UI/            # UI 层（Avalonia）
│   ├── Views/                # 视图
│   └── ViewModels/           # 视图模型
└── FindModbus.sln            # 解决方案文件
```

## 功能特性

### 已实现（P0 核心功能）

- ✅ Modbus TCP 扫描
- ✅ Modbus RTU 扫描
- ✅ 参数优先级策略
- ✅ 基础可信度评估
- ✅ 扫描进度显示
- ✅ 结果展示

### 待实现

- ⏳ 快速失败机制优化
- ⏳ 扫描结果导出
- ⏳ 参数模板保存
- ⏳ 高级模式配置

## 开发环境要求

- .NET 8.0 SDK
- Visual Studio 2022 / Rider / VS Code

## 构建与运行

### 安装依赖

```bash
dotnet restore
```

### 构建项目

```bash
dotnet build
```

### 运行

```bash
cd FindModbus.UI
dotnet run
```

### 发布

```bash
dotnet publish -c Release -r win-x64 --self-contained
dotnet publish -c Release -r osx-x64 --self-contained
dotnet publish -c Release -r linux-x64 --self-contained
```

## 使用说明

### Modbus TCP 扫描

1. 选择 "Modbus TCP" 模式
2. 输入设备 IP 地址和端口（默认 502）
3. 设置扫描范围（Slave ID、寄存器地址）
4. 点击 "开始扫描"

### Modbus RTU 扫描

1. 选择 "Modbus RTU" 模式
2. 选择串口并刷新可用端口
3. 设置波特率（默认 9600）
4. 设置扫描范围
5. 点击 "开始扫描"

## 开发计划

详见 [plan.md](./plan.md)

## 许可证

待定


