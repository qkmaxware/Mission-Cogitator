using System.Text;

namespace Kt.Data;

public class Action: Rule
{
    public int Ap {get; set;}

    public override string? Fmt()
    {
        return Description;
    }
}

public class FactionRule: Rule
{
    public int Ap {get; set;}

    public override string? Fmt()
    {
        return Description;
    }
}