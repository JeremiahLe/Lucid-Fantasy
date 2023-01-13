using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "RisingPotential", menuName = "AdventureModifierTriggers/PassiveModifiers/RisingPotential")]
public class RisingPotential : IAbilityTrigger
{
    public Modifier adventureModifier;

    public override async Task<int> TriggerModifier(AdventureManager adventureManager)
    {
        Debug.Log($"Triggering {adventureModifier.modifierName}!");

        adventureManager.bonusExp += adventureModifier.modifierAmount;

        return 1;
    }
}
