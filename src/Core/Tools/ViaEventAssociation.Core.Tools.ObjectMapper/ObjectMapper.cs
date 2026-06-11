using System.Text.Json;

namespace ViaEventAssociation.Core.Tools.ObjectMapper;

public class ObjectMapper(IServiceProvider serviceProvider) : IMapper
{
    public TOutput Map<TOutput>(object input) where TOutput : class
    {
        var type = typeof(IMappingConfig<,>).MakeGenericType(input.GetType(), typeof(TOutput));
        var mappingConfig = serviceProvider.GetService(type);

        if (mappingConfig != null)
        {
            var mapMethod = type.GetMethod(nameof(IMappingConfig<object, object>.Map))!;
            return (TOutput)mapMethod.Invoke(mappingConfig, [input])!;
        }

        var json = JsonSerializer.Serialize(input);
        return JsonSerializer.Deserialize<TOutput>(json)!;
    }
}