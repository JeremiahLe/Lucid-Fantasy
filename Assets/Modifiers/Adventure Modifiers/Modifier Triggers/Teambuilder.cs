using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "TeambuilderModifier", menuName = "AdventureModifierTriggers/PassiveModifiers/Teambuilder")]
public class Teambuilder : IAbilityTrigger
{
    public Modifier adventureModifier;
    public override async Task<int> TriggerModifier(AdventureManager adventureManager)
    {
        Debug.Log($"Triggering {adventureModifier.modifierName}!");

        adventureManager.playerMonsterLimit += (int)adventureModifier.modifierAmount;

        return 1;
    }
}
