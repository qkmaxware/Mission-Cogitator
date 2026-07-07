using System.Text.Json.Serialization;
using Kt.Data.IO;

namespace Kt.Data;

public class UnitAttributes
{
    public int Apl {get; set;}
    public int Move {get;set;}
    public int Save {get; set;}
    public int Wounds {get; set;}
}

public class Unit: IPackagedContent
{
    [JsonIgnore] public Pkg? SourcePackage {get; set;}
    [JsonIgnore] public string? Id { get; set;}
    public string? Name {get; set;}
    public string? ArtPath {get; set;}
    public string? FlavourText {get; set;}
    public List<string?>? Tags {get; set;}
    public UnitAttributes Attributes {get; set;} = new();
    public List<Weapon> Weapons {get; set;} = new();
    [JsonIgnore]
    public string? Description {get; set;}
    public List<string>? Rules {get; set;} = new();
}