using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace System.Text.Html;

public readonly struct HtmlString
{
    public string Value { get; }

    internal HtmlString(string value)
    {
        Value = value ?? string.Empty;
    }

    public override string ToString() => Value;

    public static implicit operator HtmlString(string text) => new(HtmlEncode(text));
    public static implicit operator string(HtmlString html) => html.Value;

    public static HtmlString Raw(string rawHtml) => new(rawHtml ?? string.Empty);

    private static string HtmlEncode(string text) => WebUtility.HtmlEncode(text ?? string.Empty);
}

public static class HTML
{
    public static HtmlString Html(params IEnumerable<HtmlString> children) => Tag("html", children);
    public static HtmlString Html(IEnumerable<string> children) => Tag("html", children);

    public static HtmlString Head(params IEnumerable<HtmlString> children) => Tag("head", children);
    public static HtmlString Head(IEnumerable<string> children) => Tag("head", children);

    public static HtmlString Body(params IEnumerable<HtmlString> children) => Tag("body", children);
    public static HtmlString Body(IEnumerable<string> children) => Tag("body", children);

    public static HtmlString H1(params IEnumerable<HtmlString> children) => Tag("h1", children);
    public static HtmlString H2(params IEnumerable<HtmlString> children) => Tag("h2", children);
    public static HtmlString H3(params IEnumerable<HtmlString> children) => Tag("h3", children);
    public static HtmlString H4(params IEnumerable<HtmlString> children) => Tag("h4", children);
    public static HtmlString H5(params IEnumerable<HtmlString> children) => Tag("h5", children);
    public static HtmlString H6(params IEnumerable<HtmlString> children) => Tag("h6", children);

    public static HtmlString P(params IEnumerable<HtmlString> children) => Tag("p", children);
    public static HtmlString P(IEnumerable<string> children) => Tag("p", children);

    public static HtmlString Span(params IEnumerable<HtmlString> children) => Tag("span", children);
    public static HtmlString Span(IEnumerable<string> children) => Tag("span", children);

    public static HtmlString Div(params IEnumerable<HtmlString> children) => Tag("div", children);
    public static HtmlString Div(IEnumerable<string> children) => Tag("div", children);

    public static HtmlString Ul(params IEnumerable<HtmlString> children) => Tag("ul", children);
    public static HtmlString Ol(params IEnumerable<HtmlString> children) => Tag("ol", children);
    public static HtmlString Li(params IEnumerable<HtmlString> children) => Tag("li", children);
    
    public static HtmlString A(params IEnumerable<HtmlString> children) => Tag("a", children);
    public static HtmlString Button(params IEnumerable<HtmlString> children) => Tag("button", children);

    public static HtmlString Table(params IEnumerable<HtmlString> children) => Tag("table", children);
    public static HtmlString THead(params IEnumerable<HtmlString> children) => Tag("thead", children);
    public static HtmlString Th(params IEnumerable<HtmlString> children) => Tag("th", children);
    public static HtmlString TBody(params IEnumerable<HtmlString> children) => Tag("tbody", children);
    public static HtmlString Tr(params IEnumerable<HtmlString> children) => Tag("tr", children);
    public static HtmlString Td(params IEnumerable<HtmlString> children) => Tag("td", children);

    public static HtmlString Text(string text) => text;
    public static HtmlString Raw(string rawHtml) => HtmlString.Raw(rawHtml);

    public static HtmlString Tag(string tagName, IEnumerable<HtmlString> children)
    {
        var sb = new StringBuilder();
        sb.Append('<').Append(tagName).Append('>');

        foreach (var child in children)
        {
            if (child.Value.Length > 0)
            {
                sb.Append(child.Value);
            }
        }

        sb.Append("</").Append(tagName).Append('>');
        return HtmlString.Raw(sb.ToString());
    }

    public static HtmlString Tag(string tagName, IEnumerable<string> children) => Tag(tagName, children.Select(child => (HtmlString)child));
}
