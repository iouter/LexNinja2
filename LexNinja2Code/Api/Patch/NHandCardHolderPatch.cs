using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using LexNinja2.LexNinja2Code.Api.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;

namespace LexNinja2.LexNinja2Code.Api.Patch;

[HarmonyPatch(typeof(NHandCardHolder))]
public class NHandCardHolderPatch
{
    private static readonly ConditionalWeakTable<
        NHandCardHolder,
        Dictionary<CardModel, Action>
    > _extraHandlers = new();

    private static readonly MethodInfo? _flashMethod = typeof(NHandCardHolder).GetMethod(
        "Flash",
        BindingFlags.NonPublic | BindingFlags.Instance
    );

    [HarmonyPostfix]
    [HarmonyPatch(typeof(NHandCardHolder), "SubscribeToEvents")]
    public static void SubscribePostfix(NHandCardHolder __instance, CardModel? card)
    {
        if (card is not LexNinja2Card ninjaCard || _flashMethod == null)
            return;
        var handler = (Action)Delegate.CreateDelegate(typeof(Action), __instance, _flashMethod);
        ninjaCard.LexKelaCostChanged += handler;
        var dict = _extraHandlers.GetOrCreateValue(__instance);
        dict[ninjaCard] = handler;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(NHandCardHolder), "UnsubscribeFromEvents")]
    public static void UnsubscribePostfix(NHandCardHolder __instance, CardModel? card)
    {
        if (
            card is not LexNinja2Card ninjaCard
            || !_extraHandlers.TryGetValue(__instance, out var dict)
        )
            return;
        if (!dict.TryGetValue(ninjaCard, out var handler))
            return;
        ninjaCard.LexKelaCostChanged -= handler;
        dict.Remove(ninjaCard);
    }
}
