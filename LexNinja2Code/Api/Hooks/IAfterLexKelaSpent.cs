using MegaCrit.Sts2.Core.Entities.Players;

namespace LexNinja2.LexNinja2Code.Api.Hooks;

public interface IAfterLexKelaSpent
{
    Task AfterLexKelaSpent(int amount, Player spender);
}
