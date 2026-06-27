using HtmlAgilityPack;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Kt.Data;

public static class DescriptionRenderer
{

    private static readonly string InvalidHtmlClass = "w3-text-red";
    private static readonly string InvalidHtmlString = "[Invalid HTML]";
    
    public static RenderFragment RenderDescription(string? text, object reciever, Func<string, Task>? seeTagAction = null) => builder =>
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        int seq = 0;

        try {
            var doc = new HtmlAgilityPack.HtmlDocument();

            // Wrap in a root element because descriptions are fragments
            doc.LoadHtml(text);

            foreach (var child in doc.DocumentNode.ChildNodes)
            {
                try {
                    RenderNode(builder, child, ref seq, reciever, seeTagAction);
                } catch
                {
                    //Console.WriteLine(e);
                    builder.OpenElement(seq++, "div");
                    builder.AddAttribute(seq++, "class", InvalidHtmlClass);
                    builder.AddContent(seq++, InvalidHtmlString);
                    builder.CloseElement();
                }
            }
        } catch
        {
            //Console.WriteLine(e);
            builder.OpenElement(seq++, "header");
            builder.AddAttribute(seq++, "class", InvalidHtmlClass);
            builder.AddContent(seq++, InvalidHtmlString);
            builder.CloseElement();
        }
    };

    private static readonly HashSet<string> AllowedTags =
    [
        "b",
        "i",
        "u",
        "strong",
        "em",
        "br",
        "p",
        "ul",
        "ol",
        "li",
        "code",
        "pre",
        "span",
        "div",
        "a",
        "del",
        "blockquote",
        "section",
        "header",
        "footer",
        "figure",
        "details",
        "summary",
        "article",
        "figcaption",
        "h1",
        "h2",
        "h3",
        "h4",
        "h5",
        "h6",
        "label",
        "small",
        "sup",
        "sub",
        "table",
        "thead",
        "tbody",
        "tfoot",
        "tr",
        "th",
        "td",
        "hr",
        "dl",
        "dt",
        "dd",
        "kbd",
        "samp",
        "var",
        "mark",
        "abbr",
        "cite",
        "q",
        "caption",
        "colgroup",
        "col",

        "see"
    ];

    private static void RenderNode(
        RenderTreeBuilder builder,
        HtmlNode node,
        ref int seq,
        object reciever, 
        Func<string, Task>? seeTagAction
    )
    {
        
        switch (node.NodeType)
        {
            case HtmlNodeType.Text:
            {
                if (!string.IsNullOrWhiteSpace(node.InnerText))
                {
                    builder.AddContent(seq++, node.InnerText);
                }
                break;
            }

            case HtmlNodeType.Element:
            {
                // Handle custom <see> tag
                if (node.Name.Equals("see", StringComparison.OrdinalIgnoreCase))
                {
                    RenderSeeTag(builder, node, ref seq, reciever, seeTagAction);
                    return;
                }

                if (!AllowedTags.Contains(node.Name))
                {
                    builder.OpenElement(seq++, "span");
                    builder.AddAttribute(seq++, "class", InvalidHtmlClass);
                    builder.AddContent(seq++, InvalidHtmlString);
                    builder.CloseElement();
                    return;
                }

                builder.OpenElement(seq++, node.Name);

                try {
                    foreach (var attr in node.Attributes)
                    {
                        builder.AddAttribute(seq++, attr.Name, attr.Value);
                    }

                    foreach (var child in node.ChildNodes)
                    {
                        RenderNode(builder, child, ref seq, reciever, seeTagAction);
                    }
                } 
                catch
                {
                    //Console.WriteLine(e);
                }
                finally {
                    builder.CloseElement();
                }

                break;
            }
        }
    }

    private static void RenderSeeTag(
        RenderTreeBuilder builder,
        HtmlNode node,
        ref int seq,
        object reciever, 
        Func<string, Task>? seeTagAction
    )
    {
        var ruleId = node.GetAttributeValue("cref", "");
        var text = node.InnerText;

        builder.OpenElement(seq++, "a");

        builder.AddAttribute(seq++, "href", "javascript:void(0)");

        if (seeTagAction is not null) {
        builder.AddAttribute(
            seq++,
            "onclick",
            EventCallback.Factory.Create<MouseEventArgs>(
                reciever,
                async () => await seeTagAction(ruleId)));
        }
        
        builder.AddContent(seq++, text);

        builder.CloseElement();
    }
}