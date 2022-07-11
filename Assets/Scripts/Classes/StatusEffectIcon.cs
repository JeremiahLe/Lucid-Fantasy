using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class StatusEffectIcon : MonoBehaviour
{
    public Modifier modifier;
    public string statusEffectName;
    public Sprite statusEffectIcon;

    public int modifierDuration;
    public TextMeshProUGUI modifierDurationText;
    public int currentModifierStack;

    public AttackEffect.StatEnumToChange statEnumToChange;
    public AttackEffect.StatChangeType statChangeType;
    public Modifier.StatusEffectType statusEffectType;

    public CreateMonster monsterRef;

    public StatusEffectIcon(Modifier _modifier)
    {   
        modifier = _modifier;
        modifierDurationText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        if (modifier != null)
        {
            statusEffectName = modifier.modifierSource;
            modifierDuration = modifier.modifierCurrentDuration;
            modifierDurationText.text = ($"{modifierDuration}");
            statEnumToChange = modifier.statModified;
            statChangeType = modifier.statChangeType;
        }
    }

    public void InitiateStatusEffectIcon(CreateMonster _monsterRef)
    {
        modifierDurationText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        if (modifier != null)
        {
            statusEffectName = modifier.modifierSource;
            modifierDuration = modifier.modifierCurrentDuration;
            if (modifier.modifierDurationType == Modifier.ModifierDurationType.Temporary)
            {
                modifierDurationText.text = ($"{modifierDuration}");
            }
            else
            {
                modifierDurationText.text = ($"");
            }
            statEnumToChange = modifier.statModified;
            statChangeType = modifier.statChangeType;
            currentModifierStack = 1;
            monsterRef = _monsterRef;
            GetComponent<Image>().sprite = monsterRef.ReturnStatusEffectSprite(modifier);
            if (!modifier.statusEffect)
            {
                if (modifier.statChangeType == AttackEffect.StatChangeType.Debuff)
                {
                    GetComponent<Image>().color = Color.red;
                }
                else
                {
                    GetComponent<Image>().color = Color.green;
                }
            }
        }
    }

    public void InitiateSpecialEffectIcon(CreateMonster _monsterRef)
    {
        modifierDurationText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        if (modifier != null)
        {
            statusEffectName = modifier.modifierSource;
            modifierDuration = modifier.modifierCurrentDuration;
            if (modifier.modifierDurationType == Modifier.ModifierDurationType.Temporary)
            {
                modifierDurationText.text = ($"{modifierDuration}");
            }
            else
            {
                modifierDurationText.text = ($"");
            }
            statEnumToChange = modifier.statModified;
            statChangeType = modifier.statChangeType;
            currentModifierStack = 1;
            monsterRef = _monsterRef;
            GetComponent<Image>().sprite = modifier.baseSprite;
            /*
            if (!modifier.statusEffect)
            {
                if (modifier.statChangeType == AttackEffect.StatChangeType.Debuff)
                {
                    GetComponent<Image>().color = Color.red;
                }
                else
                {
                    GetComponent<Image>().color = Color.green;
                }
            }
            */
        }
    }
}
