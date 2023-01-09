using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Threading.Tasks;
using static AttackEffect;

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
    [EnableIf("modifierType", ModifierType.equipmentModifier)]
    public float equipmentCachedAmount;

    [AssetSelector(Paths = "Assets/Sprites/UI")]
    [PreviewField(150)]
    public Sprite baseSprite;

    public async Task<int> ResetModifiedStat(Monster monsterReference, GameObject monsterReferenceGameObject)
    {
        Destroy(statusEffectIconGameObject);

        modifierCurrentDuration = modifierDuration;

        CreateMonster monsterComponent = monsterReferenceGameObject.GetComponent<CreateMonster>();

        if (isStatusEffect)
        {
            monsterComponent.combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {statusEffectType} status was cleared!", monsterReference.aiType);
            monsterComponent.CreateStatusEffectPopup($"{statusEffectType} cleared!", AttackEffect.StatChangeType.None);
            monsterComponent.listofCurrentStatusEffects.Remove(statusEffectType);

            if (statusEffectType == StatusEffectType.Enraged)
                monsterComponent.monsterEnragedTarget = null;

            return 1;
        }

        if (attackEffect.typeOfEffect == AttackEffect.TypeOfEffect.GrantImmunity)
        {
            switch (attackEffect.immunityType)
            {
                case AttackEffect.ImmunityType.Element:
                    monsterComponent.listOfElementImmunities.Remove(attackEffect.elementImmunity);
                    monsterComponent.combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} is no longer immune to {attackEffect.elementImmunity.element} Element Attacks!", monsterReference.aiType);
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
                    monsterComponent.combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} is no longer immune to Death!", monsterReference.aiType);
                    break;

                default:
                    break;
            }

            return 1;
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

        await Task.Delay(75);
        return 1;
    }

    public async Task<int> CountdownModifierDuration(Monster monsterReference, GameObject monsterReferenceGameObject)
    {
        if (monsterReferenceGameObject == null || monsterReference.health <= 0)
            return 1;

        CreateMonster monsterComponent = monsterReferenceGameObject.GetComponent<CreateMonster>();

        if (modifierDurationType == ModifierDurationType.Temporary)
        {
            modifierCurrentDuration -= 1;

            if (statusEffectIconGameObject != null && statusEffectIconGameObject.TryGetComponent(out StatusEffectIcon statusEffectIcon) != false)
            {
                statusEffectIconGameObject.GetComponent<StatusEffectIcon>().modifierDurationText.text = ($"{modifierCurrentDuration}");
            }

            if (modifierCurrentDuration <= 0)
            {
                await ResetModifiedStat(monsterReference, monsterReferenceGameObject);

                await monsterComponent.UpdateStats(false, null, false, 0);

                monsterReference.ListOfModifiers.Remove(this);
            }
        }

        await Task.Delay(75);
        return 1;
    }

    public async Task<int> DealModifierStatusEffectDamage(Monster monsterReference, GameObject monsterReferenceGameObject)
    {
        CreateMonster monsterComponent = monsterReferenceGameObject.GetComponent<CreateMonster>();
        float statusDamage = 0f;

        switch (statusEffectType)
        {
            case (StatusEffectType.Poisoned):
                statusDamage = Mathf.RoundToInt(modifierAmount * monsterReference.maxHealth);
                break;

            case (StatusEffectType.Burning):
                statusDamage = Mathf.RoundToInt(modifierAmount * monsterReference.health);
                break;

            default:
                Debug.Log($"Missing status effect reference? Current Status Effect Type: {statusEffectType}");
                await Task.Delay(75);
                return 1;
        }

        if (monsterComponent.monsterImmuneToDamage)
        {
            monsterComponent.combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} is immune to damage!");
            monsterComponent.CreateStatusEffectPopup("Immune!!", StatChangeType.None);
            await Task.Delay(75);
            return 1;
        }

        monsterComponent.HitMonster();

        if (statusDamage <= 0)
            statusDamage = 1;

        monsterReference.health -= statusDamage;
        monsterComponent.monsterDamageTakenThisRound += statusDamage;
        monsterComponent.combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} is {statusEffectType} and takes {statusDamage} damage!", monsterReference.aiType);

        await monsterComponent.UpdateStats(true, modifierOwnerGameObject, true, statusDamage);

        MonsterAttack blankAttack = new MonsterAttack(modifierName, attackEffect.elementClass, MonsterAttack.MonsterAttackDamageType.None, 1, 1, monsterReference, monsterReferenceGameObject);

        if (monsterReference.health <= 0)
            await monsterComponent.combatManagerScript.monsterAttackManager.TriggerAbilityEffects(monsterReference, monsterReferenceGameObject, AttackEffect.EffectTime.OnDeath, blankAttack);

        await Task.Delay(115);
        return 1;
    }
}
