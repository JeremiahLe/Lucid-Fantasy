using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[Serializable]
[CreateAssetMenu(fileName = "New Modifier", menuName = "Modifiers")]
public class Modifier : ScriptableObject
{
    [DisplayWithoutEdit] public string modifierSource;

    public GameObject modifierOwnerGameObject;
    public Monster modifierOwner;

    public enum ModifierDurationType { Temporary, Permanent }
    public ModifierDurationType modifierDurationType;

    public AttackEffect.StatToChange statModified;
    public AttackEffect.StatChangeType statChangeType;

    [DisableIf("modifierDurationType", ModifierDurationType.Permanent)]
    public int modifierDuration;
    [DisableIf("modifierDurationType", ModifierDurationType.Permanent)]
    public int modifierCurrentDuration;

    public float modifierAmount;
    public bool modifierAmountFlatBuff;

    public bool isStatusEffect = false;

    public enum StatusEffectType { None, Poisoned, Stunned, Dazed, Crippled, Weakened, Burning, Silenced, Enraged }
    public StatusEffectType statusEffectType;

    public GameObject statusEffectIconGameObject;
    public StatusEffectIcon statusEffectIcon;

    [Header("Adventure Variables")]
    public string modifierName;

    [EnableIf("modifierType", ModifierType.adventureModifier)]
    public AdventureModifiers.AdventureModifierReferenceList modifierAdventureReference;
    public string modifierDescription;

    public enum ModifierType { regularModifier, adventureModifier, equipmentModifier }
    public ModifierType modifierType;

    public enum ModifierAdventureCallTime { GameStart, RoundStart, OOCPassive, RoundEnd }
    public ModifierAdventureCallTime modifierAdventureCallTime;

    public enum ModifierRarity { Common, Uncommon, Rare, Legendary }
    public ModifierRarity modifierRarity;

    [EnableIf("modifierType", ModifierType.equipmentModifier)]
    public int equipmentRank = 1;
    [EnableIf("modifierType", ModifierType.equipmentModifier)]
    public static int equipmentMaxRank = 3;

    [AssetSelector(Paths = "Assets/Sprites/UI")]
    [PreviewField(150)]
    public Sprite baseSprite;

    // This function resets the modified stat that was created 
    public void ResetModifiedStat(Monster monsterReference, GameObject monsterReferenceGameObject)
    {
        // Remove icon from monster's HUD
        Destroy(statusEffectIconGameObject);

        // reset duration
        modifierCurrentDuration = modifierDuration;

        if (isStatusEffect)
        {
            monsterReferenceGameObject.GetComponent<CreateMonster>().combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {statusEffectType.ToString()} status was cleared!", monsterReference.aiType);
            monsterReferenceGameObject.GetComponent<CreateMonster>().listofCurrentStatusEffects.Remove(statusEffectType);

            //switch (statusEffectType)
            //{
            //    case StatusEffectType.Poisoned:
            //        monsterReferenceGameObject.GetComponent<CreateMonster>().monsterIsPoisoned = false;
            //        break;

            //    case StatusEffectType.Burning:
            //        monsterReferenceGameObject.GetComponent<CreateMonster>().monsterIsBurning = false;
            //        break;

            //    case StatusEffectType.Dazed:
            //        monsterReferenceGameObject.GetComponent<CreateMonster>().monsterIsDazed = false;
            //        break;

            //    case StatusEffectType.Stunned:
            //        monsterReferenceGameObject.GetComponent<CreateMonster>().monsterIsStunned = false;
            //        break;

            //    case StatusEffectType.Crippled:
            //        monsterReferenceGameObject.GetComponent<CreateMonster>().monsterIsCrippled = false;
            //        break;

            //    case StatusEffectType.Weakened:
            //        monsterReferenceGameObject.GetComponent<CreateMonster>().monsterIsWeakened = false;
            //        break;

            //    default:
            //        break;
            //}

        }
       
        switch (statModified)
        {
            case (AttackEffect.StatToChange.Evasion):
                monsterReference.evasion += -1f * (modifierAmount);
                break;

            case (AttackEffect.StatToChange.Speed):
                monsterReference.speed += -1f * (modifierAmount);
                break;

            case (AttackEffect.StatToChange.CritChance):
                monsterReference.critChance += -1f * (modifierAmount);
                break;

            case (AttackEffect.StatToChange.CritDamage):
                monsterReference.critDamage += -1f * (modifierAmount);
                break;

            case (AttackEffect.StatToChange.PhysicalAttack):
                monsterReference.physicalAttack += -1f * (modifierAmount);
                break;

            case (AttackEffect.StatToChange.PhysicalDefense):
                monsterReference.physicalDefense += -1f * (modifierAmount);
                break;

            case (AttackEffect.StatToChange.MagicAttack):
                monsterReference.magicAttack += -1f * (modifierAmount);
                break;

            case (AttackEffect.StatToChange.MagicDefense):
                monsterReference.magicDefense += -1f * (modifierAmount);
                break;

            case (AttackEffect.StatToChange.Debuffs):
                monsterReferenceGameObject.GetComponent<CreateMonster>().monsterImmuneToDebuffs = false;
                monsterReferenceGameObject.GetComponent<CreateMonster>().combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} is no longer immune to status effects and debuffs!", monsterReference.aiType);
                break;

            //case (AttackEffect.StatEnumToChange.Buffs):
            //    monsterReferenceGameObject.GetComponent<CreateMonster>().monsterImmuneToBuffs = false;
            //    monsterReferenceGameObject.GetComponent<CreateMonster>().combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} is no longer immune to buffs!", monsterReference.aiType);
            //    break;

            case (AttackEffect.StatToChange.Damage):
                monsterReferenceGameObject.GetComponent<CreateMonster>().monsterImmuneToDamage = false;
                monsterReferenceGameObject.GetComponent<CreateMonster>().combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} is no longer immune to damage!", monsterReference.aiType);
                break;

            case (AttackEffect.StatToChange.Accuracy):
                monsterReference.bonusAccuracy += -1f * (modifierAmount);
                break;

            default:
                Debug.Log("Missing stat modified or type?");
                break;
        }
    }
}
