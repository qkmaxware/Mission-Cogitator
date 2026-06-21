using System.Text.Json.Serialization;
using Kt.Data.IO;

namespace Kt.Data;

public class Team: IPackagedContent
{
    public Pkg? SourcePackage {get; set;}
    public string? Id { get; set;}
    public string? Name {get; set;}
    public string? Faction {get; set;}
    public string? Version {get; set;}

    public List<string>? FactionRuleIds {get; set;}
    [JsonIgnore] public List<Rule>? FactionRules {get; set;}

    public List<string>? PloyIds {get; set;}
    [JsonIgnore] public List<Ploy>? Ploys {get; set;}

    public List<string>? EquipmentIds {get; set;}
    [JsonIgnore] public List<Equipment>? Equipment {get; set;}

    public List<string>? UnitIds {get; set;}
    [JsonIgnore] public List<Unit>? Units {get; set;}

    [JsonIgnore]
    public string? Description { get; set; }
}