using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using LexNinja2.LexNinja2Code.Api.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Models;

namespace LexNinja2.LexNinja2Code.Api.Patch;

[HarmonyPatch(typeof(CombatStateTracker))]
public static class CombatStateTrackerExtraEventsPatch
{
    private static readonly ConditionalWeakTable<
        CombatStateTracker,
        Dictionary<CardModel, Action>
    > _extraHandlers = new();

    private static readonly MethodInfo? _onCardValueChangedMethod =
        typeof(CombatStateTracker).GetMethod(
            "OnCardValueChanged",
            BindingFlags.NonPublic | BindingFlags.Instance
        );

    [HarmonyPostfix]
    [HarmonyPatch(nameof(CombatStateTracker.Subscribe), typeof(CardModel))]
    public static void SubscribePostfix(CombatStateTracker __instance, CardModel card)
    {
        if (card is not LexNinja2Card ninjaCard || _onCardValueChangedMethod == null)
            return;
        var handler = (Action)
            Delegate.CreateDelegate(typeof(Action), __instance, _onCardValueChangedMethod);
        ninjaCard.LexKelaCostChanged += handler;
        var dict = _extraHandlers.GetOrCreateValue(__instance);
        dict[ninjaCard] = handler;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(CombatStateTracker.Unsubscribe), typeof(CardModel))]
    public static void UnsubscribePostfix(CombatStateTracker __instance, CardModel card)
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
