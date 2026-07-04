using System.Text.Json.Serialization;

namespace Kt.Data;

public enum PloyKind
{
    strategy, firefight
}

public class Ploy: Rule
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PloyKind Kind {get; set;}

    public int CP() => 1; // All ploys cost 1 command point, for now. This may change in the future.
}