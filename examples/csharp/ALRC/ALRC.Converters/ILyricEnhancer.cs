using ALRC.Abstraction;

namespace ALRC.Converters;

public interface ILyricEnhancer<T>
{
    public T Enhance(ALRCFile input);
    public ALRCFile Deserialize(T input, ALRCFile target);
}