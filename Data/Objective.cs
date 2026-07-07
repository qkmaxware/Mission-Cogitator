using System.Text.Json.Serialization;
using Kt.Data.IO;

namespace Kt.Data;


public enum ObjectiveCategory
{
    Primary = 0,
    Critical = 0,

    Secondary = 1,
    Tactical = 1,
}

public enum ObjectiveArchetype
{
    Fixed = 0,
    Infiltration, 
    Recon, 
    Security,
    SeekNDestroy
}

public class Objective: Rule
{
    [JsonConverter(typeof(JsonStringEnumConverter))] public ObjectiveCategory Kind {get; set;}
    [JsonConverter(typeof(JsonStringEnumConverter))] public ObjectiveArchetype Archetype {get; set;}

}