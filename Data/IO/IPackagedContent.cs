namespace Kt.Data.IO;

public interface IPackagedContent
{
    public Pkg? SourcePackage {get; set;}
    
    public string? Id {get; set;}
    public string? Description {get; set;}
}