using System.Text.RegularExpressions;

namespace Kt.Data.IO;

/// <summary>
/// An HTML document with yaml/json front matter
/// </summary>
public class FmHtmlDocument
{
    private string document;
    private int fmStart = 0;
    private int fmEnd = 0;
    private int htmlStart = 0;

    public Range FrontMatterRange => new Range(fmStart, fmEnd);

    public Range HtmlRange => new Range(htmlStart, document.Length);

    /// <summary>
    /// The part of the document representing the front matter
    /// </summary>
    public ReadOnlySpan<char> FrontMatter => document.AsSpan(fmStart, fmEnd - fmStart);

    /// <summary>
    /// The part of the document representing the raw HTML
    /// </summary>
    public ReadOnlySpan<char> Html => document.AsSpan(htmlStart);

    private static Regex endRegex = new Regex(@"\r?\n---\r?\n", RegexOptions.Compiled);

    public FmHtmlDocument(string document)
    {
        this.document = document;

        fmStart = 0;
        fmEnd = 0;
        htmlStart = 0;

        if (!document.StartsWith("---"))
        {
            htmlStart = 0;
            return;
        }

        int firstNewline = document.IndexOf('\n');
        if (firstNewline < 0)
        {
            htmlStart = 0;
            return;
        }

        fmStart = firstNewline + 1;

        int closingDelimiter = -1;
        var match = endRegex.Match(document);
        if (match.Success)
        {
            closingDelimiter = match.Index;
        }

        if (closingDelimiter < 0)
        {
            fmEnd = document.Length;
            htmlStart = document.Length;
            return;
        }

        fmEnd = closingDelimiter + 1;
        htmlStart = closingDelimiter + 5;
    }
}