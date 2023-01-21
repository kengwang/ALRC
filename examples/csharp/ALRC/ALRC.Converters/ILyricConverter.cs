using ALRC.Abstraction;

namespace ALRC.Converters;

public interface ILyricConverter<T>
{
    public ALRCFile Convert(T input);
    public T ConvertBack(ALRCFile input);
}