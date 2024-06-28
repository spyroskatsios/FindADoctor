using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Appointments.Infrastructure.Persistence.Converters;

public static class ValueConverters
{
    public static ValueConverter<T, string> JsonConverter<T>(ConverterMappingHints? mappingHints = null) => new(
        x => JsonSerializer.Serialize(x, JsonSerializerOptions.Default),
        x => JsonSerializer.Deserialize<T>(x, JsonSerializerOptions.Default)!,
        mappingHints);
}

