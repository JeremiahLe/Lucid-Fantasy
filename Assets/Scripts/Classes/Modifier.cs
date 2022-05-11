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

    public bool statusEffect = false;
    public enum StatusEffectType { None, Poisoned, Stunned, Dazed, Crippled, Weakened }
    public StatusEffectType statusEffectType;

    // This function resets the modified stat that was created 
    public void ResetModifiedStat(Monster monsterReference, GameObject monsterReferenceGameObject)
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

            case (AttackEffect.StatEnumToChange.CritChance):
                monsterReference.critChance += -1f * (modifierAmount);
                break;

            case (AttackEffect.StatEnumToChange.PhysicalAttack):
                monsterReference.physicalAttack += -1f * (modifierAmount);
                break;

            case (AttackEffect.StatEnumToChange.PhysicalDefense):
                monsterReference.physicalDefense += -1f * (modifierAmount);
                break;

            case (AttackEffect.StatEnumToChange.MagicAttack):
                monsterReference.magicAttack += -1f * (modifierAmount);
                break;

            case (AttackEffect.StatEnumToChange.MagicDefense):
                monsterReference.magicDefense += -1f * (modifierAmount);
                break;

            case (AttackEffect.StatEnumToChange.Debuffs):
                monsterReferenceGameObject.GetComponent<CreateMonster>().monsterImmuneToDebuffs = false;
                break;

            case (AttackEffect.StatEnumToChange.StatChanges):
                monsterReferenceGameObject.GetComponent<CreateMonster>().monsterImmuneToStatChanges = false;
                break;

            case (AttackEffect.StatEnumToChange.Damage):
                monsterReferenceGameObject.GetComponent<CreateMonster>().monsterImmuneToDamage = false;
                break;

            default:
                Debug.Log("Missing stat modified or type?");
                break;
        }
    }
}
