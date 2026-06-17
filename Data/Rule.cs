using System.Text.RegularExpressions;
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

    private static readonly Regex xpattern = new Regex(@"\b[xX]\b", RegexOptions.Compiled);
    public string? XReplace(int x)
    {
        if (Name is not null)
        {
            return xpattern.Replace(Name, x.ToString());
        }

        return Id;
    }
}