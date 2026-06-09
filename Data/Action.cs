using System.Text;

namespace Kt.Data;

public class Action: Rule
{
    public int Ap {get; set;}
    public List<string>? Usage {get; set;}
    public List<string>? Conditions {get; set;}

    public override string? Fmt()
    {
        StringBuilder sb = new StringBuilder(Description?.Length ?? 10); // Default capacity
        sb.Append(Description);

        sb.Append("<ul class='usage-bullet'>");
        foreach (var use in (Usage ?? Enumerable.Empty<string>()))
        {
            sb.Append("<li>");
            sb.Append(use);
            sb.Append("</li>");
        }
        sb.Append("</ul>");

        sb.Append("<ul class='condition-bullet'>");
        foreach (var use in (Conditions ?? Enumerable.Empty<string>()))
        {
            sb.Append("<li>");
            sb.Append(use);
            sb.Append("</li>");
        }
        sb.Append("</ul>");
        return sb.ToString();
    }
}


public class FactionRule: Rule
{
    public int Ap {get; set;}
    public List<string>? Usage {get; set;}
    public List<string>? Conditions {get; set;}

    public override string? Fmt()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(Description);

        sb.Append("<ul class='usage-bullet'>");
        foreach (var use in (Usage ?? Enumerable.Empty<string>()))
        {
            sb.Append("<li>");
            sb.Append(use);
            sb.Append("</li>");
        }
        sb.Append("</ul>");

        sb.Append("<ul class='condition-bullet'>");
        foreach (var use in (Conditions ?? Enumerable.Empty<string>()))
        {
            sb.Append("<li>");
            sb.Append(use);
            sb.Append("</li>");
        }
        sb.Append("</ul>");
        return sb.ToString();
    }
}