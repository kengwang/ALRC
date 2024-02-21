using ALRC.Abstraction;

namespace ALRC.Converters;

public interface ILrcTranslationConverter<T>
{
    public T Convert(ALRCFile input);
    public ALRCFile ConvertBack(T input, ALRCFile target);
}