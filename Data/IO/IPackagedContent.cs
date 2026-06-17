using System.Text.Json.Serialization;

namespace Kt.Data.IO;

public interface IPackagedContent
{
    [JsonIgnore] public Pkg? SourcePackage {get; set;}
    
    public string? Id {get; set;}
    public string? Name {get; set;}
    public string? Description {get; set;}
}