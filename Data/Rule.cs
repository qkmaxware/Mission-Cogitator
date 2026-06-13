using Kt.Data.IO;
using Microsoft.AspNetCore.Components;

namespace Kt.Data;

public class Rule: IPackagedContent
{
    public Pkg? SourcePackage {get; set;}
    public string? Id { get; set;}
    public string? Name {get; set;}
    public string? Description {get; set;}
    public string? ArtPath {get; set;}
    public string? ArtCaption {get; set;}

    public virtual string? Fmt()
    {
        return Description;
    }
}