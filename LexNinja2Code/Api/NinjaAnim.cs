using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;

namespace LexNinja2.LexNinja2Code.Api;

public static class NinjaAnim
{
    public static async Task TriggerCastAnim(CardModel card)
    {
        try
        {
            //TODO: 等待baselib更新
            await CreatureCmd.TriggerAnim(
                card.Owner.Creature,
                "Cast",
                card.Owner.Character.CastAnimDelay
            );
        }
        catch (Exception e)
        {
            MainFile.Logger.Error(e.Message);
            throw;
        }
    }
}
