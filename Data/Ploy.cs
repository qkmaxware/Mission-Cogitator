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
}