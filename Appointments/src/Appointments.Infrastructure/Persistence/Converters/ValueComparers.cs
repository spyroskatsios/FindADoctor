using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Appointments.Infrastructure.Persistence.Converters;

public static class ValueComparers
{
    public static ValueComparer<List<T>> ListComparer<T>() => new(
        (t1, t2) => t1!.SequenceEqual(t2!),
        t => t.Select(x => x!.GetHashCode()).Aggregate((x, y) => x ^ y),
        t => t);

    public static ValueComparer<T> JsonComparer<T>() => new(
        (l, r) => JsonSerializer.Serialize(l, JsonSerializerOptions.Default) ==
                  JsonSerializer.Serialize(r, JsonSerializerOptions.Default),
        v => v == null ? 0 : JsonSerializer.Serialize(v, JsonSerializerOptions.Default).GetHashCode(),
        v => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
            JsonSerializerOptions.Default)!);
}