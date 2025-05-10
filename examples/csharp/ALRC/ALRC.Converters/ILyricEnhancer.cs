using ALRC.Abstraction;

namespace ALRC.Converters;

public interface ILyricEnhancer<T>
{
    public T Extract(ALRCFile input);
    public ALRCFile Enhance(T input, ALRCFile target);
}