using System.ComponentModel;

namespace FindModbus.UI.ViewModels;

public class FunctionCodeItem : INotifyPropertyChanged
{
    private byte _code;
    private string _displayText = string.Empty;

    public byte Code
    {
        get => _code;
        set
        {
            if (_code != value)
            {
                _code = value;
                OnPropertyChanged(nameof(Code));
            }
        }
    }

    public string DisplayText
    {
        get => _displayText;
        set
        {
            if (_displayText != value)
            {
                _displayText = value;
                OnPropertyChanged(nameof(DisplayText));
            }
        }
    }

    public override string ToString() => DisplayText;

    public override bool Equals(object? obj)
    {
        if (obj is FunctionCodeItem other)
        {
            return Code == other.Code;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Code.GetHashCode();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

