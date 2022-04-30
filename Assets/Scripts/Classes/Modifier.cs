using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Modifier", menuName = "Modifiers")]
public class Modifier : ScriptableObject
{
    [DisplayWithoutEdit] public string modifierSource;

    public enum ModifierDurationType { Temporary, Permanent }
    public ModifierDurationType modifierDurationType;

    public AttackEffect.StatEnumToChange statModified;

    [DisplayWithoutEdit] public int modifierDuration;
    [DisplayWithoutEdit] public int modifierCurrentDuration;

    [DisplayWithoutEdit] public float modifierAmount;

    // This function resets the modified stat that was created 
    public void ResetModifiedStat(Monster monsterReference)
    {
        // reset duration
        modifierCurrentDuration = modifierDuration;
       
        switch (statModified)
        {
            case (AttackEffect.StatEnumToChange.Evasion):
                monsterReference.evasion += -1f * (modifierAmount);
                break;

            case (AttackEffect.StatEnumToChange.Speed):
                monsterReference.speed += -1f * (modifierAmount);
                break;

            default:
                Debug.Log("Missing stat modified or type?");
                break;
        }
    }
}
