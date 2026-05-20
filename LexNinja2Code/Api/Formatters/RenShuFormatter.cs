using System.Text;
using BaseLib.Abstracts;
using LexNinja2.LexNinja2Code.Api.Cards;
using LexNinja2.LexNinja2Code.Api.DynamicVars;
using LexNinja2.LexNinja2Code.Api.Extensions;
using SmartFormat.Core.Extensions;

namespace LexNinja2.LexNinja2Code.Api.Formatters;

public class RenShuFormatter : IAutoRegisterFormatSpecifier
{
    public bool TryEvaluateFormat(IFormattingInfo formattingInfo)
    {
        if (formattingInfo.CurrentValue is not NinjutsuVar renShu)
        {
            return false;
        }
        var owner = renShu.GetOwner();
        if (owner is not LexNinja2Card card)
        {
            return false;
        }
        if (card.HasLexKelaCostX)
        {
            formattingInfo.Write("X");
            return true;
        }
        if (owner is { IsMutable: true })
        {
            formattingInfo.Write(GetLexKelaText(card));
            return true;
        }
        formattingInfo.Write(renShu.ToHighlightedString(true));
        return true;
    }

    public string Name
    {
        get => "renShu";
        set => throw new NotImplementedException();
    }

    public bool CanAutoDetect { get; set; }

    private static string GetLexKelaText(LexNinja2Card card)
    {
        var sb = new StringBuilder();
        var color = NinjaColor.GetLexKelaCostColor(card).GetColorName();
        var hasColor = color != null;
        if (hasColor)
        {
            sb.Append('[');
            sb.Append(color);
            sb.Append(']');
        }
        sb.Append(card.GetLexKelaCostWithModifiers());
        if (!hasColor)
            return sb.ToString();
        sb.Append("[/");
        sb.Append(color);
        sb.Append(']');

        return sb.ToString();
    }
}
