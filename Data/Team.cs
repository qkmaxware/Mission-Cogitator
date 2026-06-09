using Kt.Data.IO;

namespace Kt.Data;

public class Team: IPackagedContent
{
    public string? Id { get; set;}
    public string? Name {get; set;}
    public string? Faction {get; set;}
    public string? Version {get; set;}
    public List<string>? UnitIds {get; set;}
    public List<Unit>? Units {get; set;}

    public string? Description { get; set; }
    public Dictionary<string, FactionRule>? Rules {get; set;}
}