using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using Sirenix.OdinInspector;

[Serializable]
[CreateAssetMenu(fileName = "New Ability", menuName = "Abilities")]
public class Ability : ScriptableObject
{
    public string abilityName;
    [TextArea(5, 10)]
    public string abilityDescription;

    public List<AttackEffect> listOfAbilityEffects;

    public List<IAbilityTrigger> listOfAbilityTriggers;
}
