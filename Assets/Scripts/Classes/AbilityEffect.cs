using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AbilityEffect : AttackEffect
{
    public AbilityEffect(StatEnumToChange statEnumToChange, StatChangeType statChangeType, EffectTime effectTime, Modifier.StatusEffectType attackEffectStatus, bool inflictSelf, bool modifierCalledOnce, bool flatBuff, int modifierDuration, float amountToChange, float effectTriggerChance, CombatManagerScript combatManagerScript) : base(statEnumToChange, statChangeType, effectTime, attackEffectStatus, inflictSelf, modifierCalledOnce, flatBuff, modifierDuration, amountToChange, effectTriggerChance, combatManagerScript)
    {
    }
}
