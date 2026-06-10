using HtmlAgilityPack;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Kt.Data;

public static class DescriptionRenderer
{
    
    public static RenderFragment RenderDescription(string? text, object reciever, Func<string, Task>? seeTagAction = null) => builder =>
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        var doc = new HtmlAgilityPack.HtmlDocument();

        // Wrap in a root element because descriptions are fragments
        doc.LoadHtml(text);

        int seq = 0;

        foreach (var child in doc.DocumentNode.ChildNodes)
        {
            RenderNode(builder, child, ref seq, reciever, seeTagAction);
        }
    };

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

                builder.OpenElement(seq++, node.Name);

                foreach (var attr in node.Attributes)
                {
                    builder.AddAttribute(seq++, attr.Name, attr.Value);
                }

                foreach (var child in node.ChildNodes)
                {
                    RenderNode(builder, child, ref seq, reciever, seeTagAction);
                }

                builder.CloseElement();

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

        builder.AddAttribute(seq++, "href", "#");

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