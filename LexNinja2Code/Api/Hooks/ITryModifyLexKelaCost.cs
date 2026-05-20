using MegaCrit.Sts2.Core.Models;

namespace LexNinja2.LexNinja2Code.Api.Hooks;

public interface ITryModifyLexKelaCost
{
    bool TryModifyLexKeLaCost(CardModel card, decimal originalCost, out decimal modifiedCost);
}
