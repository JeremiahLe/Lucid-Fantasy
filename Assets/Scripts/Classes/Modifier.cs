using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[Serializable]
[CreateAssetMenu(fileName = "New Modifier", menuName = "Modifiers")]
public class Modifier : ScriptableObject
{
    [Header("Modifier Object Data")]
    [DisplayWithoutEdit] public string modifierSource;

    [DisplayWithoutEdit] public GameObject modifierOwnerGameObject;
    [DisplayWithoutEdit] public Monster modifierOwner;

    [DisplayWithoutEdit] public GameObject statusEffectIconGameObject;
    [DisplayWithoutEdit] public StatusEffectIcon statusEffectIcon;

    [DisplayWithoutEdit] public AttackEffect attackEffect;

    [Title("Modifier Informaton")]
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
    [EnableIf("isStatusEffect", true)]
    public StatusEffectType statusEffectType;

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

        CreateMonster monsterComponent = monsterReferenceGameObject.GetComponent<CreateMonster>();

        if (isStatusEffect)
        {
            monsterComponent.combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {statusEffectType.ToString()} status was cleared!", monsterReference.aiType);
            monsterComponent.listofCurrentStatusEffects.Remove(statusEffectType);
            return;
        }

        if (attackEffect.typeOfEffect == AttackEffect.TypeOfEffect.GrantImmunity)
        {
            switch (attackEffect.immunityType)
            {
                case AttackEffect.ImmunityType.Element:
                    monsterComponent.listOfElementImmunities.Remove(attackEffect.elementImmunity);
                    monsterComponent.combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} is no longer immune to {attackEffect.elementImmunity.element.ToString()} Element Attacks!", monsterReference.aiType);
                    break;

                case AttackEffect.ImmunityType.Status:
                    monsterComponent.listOfStatusImmunities.Remove(attackEffect.statusImmunity);
                    monsterComponent.combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} is no longer immune to {attackEffect.statusImmunity} Status!", monsterReference.aiType);
                    break;

                case AttackEffect.ImmunityType.SpecificStatChange:
                    monsterComponent.listOfStatImmunities.Remove(attackEffect.statImmunity);
                    monsterComponent.combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} is no longer immune to {attackEffect.statImmunity} Debuffs!", monsterReference.aiType);
                    break;

                case AttackEffect.ImmunityType.Damage:
                    monsterComponent.monsterImmuneToDamage = false;
                    monsterComponent.combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} is no longer immune to Damage!", monsterReference.aiType);
                    break;

                case AttackEffect.ImmunityType.Debuffs:
                    monsterComponent.monsterImmuneToDebuffs = false;
                    monsterComponent.combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} is no longer immune to Status Effects and Debuffs!", monsterReference.aiType);
                    break;

                case AttackEffect.ImmunityType.Death:
                    //monsterComponent.listOfStatusImmunities.Remove(attackEffect.statusImmunity);
                    monsterComponent.combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} is no longer immune to Death!", monsterReference.aiType);
                    break;

                default:
                    break;
            }

            return;
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
