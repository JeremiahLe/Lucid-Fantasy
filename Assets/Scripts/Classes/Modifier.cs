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

    [DisplayWithoutEdit] public GameObject modifierOwnerGameObject;
    [DisplayWithoutEdit] public Monster modifierOwner;

    public enum ModifierDurationType { Temporary, Permanent }
    public ModifierDurationType modifierDurationType;

    public AttackEffect.StatEnumToChange statModified;
    public AttackEffect.StatChangeType statChangeType;

    [DisableIf("modifierDurationType", ModifierDurationType.Permanent)]
    public int modifierDuration;
    [DisableIf("modifierDurationType", ModifierDurationType.Permanent)]
    public int modifierCurrentDuration;

    public float modifierAmount;
    public bool modifierAmountFlatBuff;

    public bool statusEffect = false;
    [EnableIf("statusEffect")]
    public enum StatusEffectType { None, Poisoned, Stunned, Dazed, Crippled, Weakened, Burning }
    public StatusEffectType statusEffectType;

    public GameObject statusEffectIconGameObject;
    public StatusEffectIcon statusEffectIcon;

    [Header("Adventure Variables")]
    public string modifierName;
    [DisableIf("adventureEquipment")]
    public AdventureModifiers.AdventureModifierReferenceList modifierAdventureReference;
    public string modifierDescription;

    [DisableIf("adventureEquipment")]
    public bool adventureModifier = false;
    [DisableIf("adventureModifier")]
    public bool adventureEquipment = false;

    public enum ModifierAdventureCallTime { GameStart, RoundStart, OOCPassive, RoundEnd }
    public ModifierAdventureCallTime modifierAdventureCallTime;

    public enum ModifierRarity { Common, Uncommon, Rare, Legendary }
    public ModifierRarity modifierRarity;

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

        if (statusEffect)
        {
            monsterReferenceGameObject.GetComponent<CreateMonster>().statusEffectUISprite.sprite = monsterReferenceGameObject.GetComponent<CreateMonster>().monsterAttackManager.noUISprite;
            monsterReferenceGameObject.GetComponent<CreateMonster>().combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {statusEffectType.ToString()} status was cleared!", monsterReference.aiType);

            switch (statusEffectType)
            {
                case StatusEffectType.Poisoned:
                    monsterReferenceGameObject.GetComponent<CreateMonster>().monsterIsPoisoned = false;
                    break;

                case StatusEffectType.Burning:
                    monsterReferenceGameObject.GetComponent<CreateMonster>().monsterIsBurning = false;
                    break;

                case StatusEffectType.Dazed:
                    monsterReferenceGameObject.GetComponent<CreateMonster>().monsterIsDazed = false;
                    break;

                case StatusEffectType.Stunned:
                    monsterReferenceGameObject.GetComponent<CreateMonster>().monsterIsStunned = false;
                    break;

                case StatusEffectType.Crippled:
                    monsterReferenceGameObject.GetComponent<CreateMonster>().monsterIsCrippled = false;
                    break;

                case StatusEffectType.Weakened:
                    monsterReferenceGameObject.GetComponent<CreateMonster>().monsterIsWeakened = false;
                    break;

                default:
                    break;
            }

        }
       
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

            case (AttackEffect.StatEnumToChange.CritDamage):
                monsterReference.critDamage += -1f * (modifierAmount);
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
                monsterReferenceGameObject.GetComponent<CreateMonster>().combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} is no longer immune to status effects and debuffs!", monsterReference.aiType);
                break;

            case (AttackEffect.StatEnumToChange.StatChanges):
                monsterReferenceGameObject.GetComponent<CreateMonster>().monsterImmuneToStatChanges = false;
                break;

            case (AttackEffect.StatEnumToChange.Damage):
                monsterReferenceGameObject.GetComponent<CreateMonster>().monsterImmuneToDamage = false;
                monsterReferenceGameObject.GetComponent<CreateMonster>().combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} is no longer immune to damage!", monsterReference.aiType);
                break;

            case (AttackEffect.StatEnumToChange.Accuracy):
                monsterReference.bonusAccuracy += -1f * (modifierAmount);
                break;

            default:
                Debug.Log("Missing stat modified or type?");
                break;
        }
    }
}
