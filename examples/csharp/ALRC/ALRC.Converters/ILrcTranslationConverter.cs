using ALRC.Abstraction;

namespace ALRC.Converters;

public interface ILrcTranslationConverter<T>
{
    public T Convert(ALRCFile input, string lang);
    public ALRCFile ConvertBack(T input, ALRCFile target, string lang);
}