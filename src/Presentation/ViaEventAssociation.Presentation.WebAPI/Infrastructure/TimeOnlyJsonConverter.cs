using System.Text.Json;
using System.Text.Json.Serialization;

namespace ViaEventAssociation.Presentation.WebAPI.Infrastructure;

public class TimeOnlyJsonConverter : JsonConverter<TimeOnly>
{
    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString()!.TrimEnd('Z');
        return TimeOnly.Parse(value);
    }

    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString("HH:mm:ss"));
}