using System.Text;
using System.Text.Json.Serialization;
using Kt.Data.IO;

namespace Kt.Data;

public class TeamDescriptionRule: Rule
{
    public Team Team {get; init;}

    public TeamDescriptionRule(Team team, string? description)
    {
        this.Team = team;

        SourcePackage=team.SourcePackage;
        Id=team.Id;
        Name=team.Name;
        Description=description ?? team.Description;
    }
}

public class Team: IPackagedContent
{
    [JsonIgnore] public Pkg? SourcePackage {get; set;}
    [JsonIgnore] public string? Id { get; set;}
    public string? Name {get; set;}
    public string? ArtPath {get; set;}
    public string? Faction {get; set;}
    public string? Version {get; set;}

    public List<string>? FactionRuleIds {get; set;}
    [JsonIgnore] public List<Rule>? FactionRules {get; set;}

    public List<string>? PloyIds {get; set;}
    [JsonIgnore] public List<Ploy>? Ploys {get; set;}

    public List<string>? EquipmentIds {get; set;}
    [JsonIgnore] public List<Equipment>? Equipment {get; set;}

    public List<string>? UnitIds {get; set;}
    public List<string>? ExternalUnitIds {get; set;}
    [JsonIgnore] public List<Unit>? Units {get; set;}

    [JsonIgnore]
    public string? Description { get; set; }
}