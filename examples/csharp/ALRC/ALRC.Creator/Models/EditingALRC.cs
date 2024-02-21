using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ALRC.Abstraction;
using ALRC.Creator.Interfaces;

namespace ALRC.Creator.Models;

public class EditingALRC : IModel
{
    private EditingALRCLyricInfo _lyricInfo = new();
    private ObservableCollection<SongInfoItem> _songInfos = new();
    private ObservableCollection<EditingALRCLine> _lines = new();
    private ObservableCollection<EditingALRCStyle> _styles = new();

    public ObservableCollection<EditingALRCStyle> Styles
    {
        get => _styles;
        set => SetField(ref _styles, value);
    }

    public ObservableCollection<EditingALRCLine> Lines
    {
        get => _lines;
        set => SetField(ref _lines, value);
    }
    
    public ObservableCollection<SongInfoItem> SongInfos
    {
        get => _songInfos;
        set => SetField(ref _songInfos, value);
    }

    public EditingALRCLyricInfo LyricInfo
    {
        get => _lyricInfo;
        set => SetField(ref _lyricInfo, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

public class EditingALRCWord : IModel
{
    private string? _displayWord;
    private string? _transliteration;
    public long Start { get; set; }
    public long End { get; set; }
    public string? Word { get; set; }
    public string? WordStyle { get; set; }

    public string? Transliteration
    {
        get => _transliteration;
        set
        {
            SetField(ref _transliteration, value);
            OnPropertyChanged(nameof(DisplayWord));
        }
    }

    public string? DisplayWord
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(_transliteration))
                return $"{Word} ({_transliteration})";
            return _displayWord ?? Word;
        }
        set => _displayWord = value;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

public class EditingALRCLine : IModel
{
    private int _type;
    private long _start;    
    private long _end;
    private string? _lineStyle;
    private string? _comment;
    private string? _text;
    private string? _translation;
    private string? _id;
    private string? _parentLineId;
    private ObservableCollection<EditingALRCWord>? _words = new();
    private string? _transliteration;

    public ObservableCollection<EditingALRCWord>? Words
    {
        get => _words;
        set => SetField(ref _words, value);
    }


    public string? ParentLineId
    {
        get => _parentLineId;
        set => SetField(ref _parentLineId, value);
    }

    public string? Id
    {
        get => _id;
        set => SetField(ref _id, value);
    }

    public string? Transliteration
    {
        get => _transliteration;
        set => SetField(ref _transliteration, value);
    }

    public int Type
    {
        get => _type;
        set => SetField(ref _type, value);
    }

    public long Start
    {
        get => _start;
        set => SetField(ref _start, value);
    }

    public long End
    {
        get => _end;
        set => SetField(ref _end, value);
    }

    public string? LineStyle
    {
        get => _lineStyle;
        set => SetField(ref _lineStyle, value);
    }

    public string? Comment
    {
        get => _comment;
        set => SetField(ref _comment, value);
    }

    public string? Text
    {
        get => _text;
        set => SetField(ref _text, value);
    }

    public string? Translation
    {
        get => _translation;
        set => SetField(ref _translation, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}


public class EditingALRCStyle : IModel
{
    public bool Hidden
    {
        get => _hidden;
        set
        {
            if (value == _hidden) return;
            _hidden = value;
            OnPropertyChanged();
        }
    }

    public int Type
    {
        get => _type;
        set => SetField(ref _type, value);
    }

    public string? Color
    {
        get => _color;
        set => SetField(ref _color, value);
    }

    public int Position
    {
        get => _position;
        set => SetField(ref _position, value);
    }

    private string _id = string.Empty;
    private int _position = 0;
    private string? _color;
    private int _type = 0;
    private bool _hidden = false;

    public string Id
    {
        get => _id;
        set => SetField(ref _id, value);
    }


    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

public class SongInfoItem : IModel
{
    private string _param = string.Empty;
    private string _value = string.Empty;

    public SongInfoItem(string param, string value)
    {
        _param = param;
        _value = value;
    }

    public SongInfoItem()
    {
    }

    public string Param
    {
        get => _param;
        set => SetField(ref _param, value);
    }

    public string Value
    {
        get => _value;
        set => SetField(ref _value, value);
    }


    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

public class EditingALRCLyricInfo : IModel
{
    private string? _language;
    private string? _author;
    private string? _translation;
    private string? _timeline;
    private string? _transliteration;
    private string? _proofread;
    private int? _offset;
    private long? _duration;

    public string? Language
    {
        get => _language;
        set => SetField(ref _language, value);
    }

    public string? Author
    {
        get => _author;
        set => SetField(ref _author, value);
    }

    public string? Translation
    {
        get => _translation;
        set => SetField(ref _translation, value);
    }

    public string? Timeline
    {
        get => _timeline;
        set => SetField(ref _timeline, value);
    }

    public string? Transliteration
    {
        get => _transliteration;
        set => SetField(ref _transliteration, value);
    }

    public string? Proofread
    {
        get => _proofread;
        set => SetField(ref _proofread, value);
    }

    public int? Offset
    {
        get => _offset;
        set => SetField(ref _offset, value);
    }

    public long? Duration
    {
        get => _duration;
        set => SetField(ref _duration, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}