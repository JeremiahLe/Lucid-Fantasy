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

    public AttackEffect.StatToChange statEnumToChange;
    public AttackEffect.StatChangeType statChangeType;
    public Modifier.StatusEffectType statusEffectType;

    public CreateMonster monsterRef;
    public Interactable interactable;

    public Color32 buffColor;
    public Color32 debuffColor;

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
        interactable = GetComponent<Interactable>();

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

            // Initiate the modifier icon's interactable component
            interactable.interactableDescriptionWindow = monsterRef.InteractableToolTipWindow;
            interactable.typeOfInteractable = Interactable.TypeOfInteractable.Modifier;
            interactable.modifier = modifier;
            interactable.InitiateInteractable(modifier);

            if (!modifier.statusEffect)
            {
                if (modifier.statChangeType == AttackEffect.StatChangeType.Debuff)
                {
                    debuffColor = new Color32(193, 81, 81, 255);
                    GetComponent<Image>().color = debuffColor;
                }
                else
                {
                    buffColor = new Color32(106, 150, 215, 255);
                    GetComponent<Image>().color = buffColor;
                }
            }
        }
    }

    public void InitiateSpecialEffectIcon(CreateMonster _monsterRef)
    {
        modifierDurationText = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        interactable = GetComponent<Interactable>();

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

            // Initiate the modifier icon's interactable component
            interactable.interactableDescriptionWindow = monsterRef.InteractableToolTipWindow;
            interactable.typeOfInteractable = Interactable.TypeOfInteractable.Modifier;
            interactable.modifier = modifier;
            interactable.InitiateInteractable(modifier);
        }
    }

    //// This function displays monster stats on right-click
    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    if (eventData.button == PointerEventData.InputButton.Right)
    //    {
    //        if (!modifierStatScreenWindow.activeSelf)
    //        {
    //            modifierStatScreenWindow.SetActive(true);
    //            if (modifier.adventureModifier)
    //            {
    //                modifierWindowText.text =
    //                ($"{modifier.modifierName}\n" +
    //                $"{modifier.modifierDescription}");
    //                return;
    //            }
    //            if (modifier.statusEffect)
    //            {
    //                modifierWindowText.text =
    //                ($"{modifier.modifierSource} ({modifier.statusEffectType.ToString()})\n" +
    //                $"{modifier.modifierAmount * 100f}% {monsterRef.ReturnStatusEffectDescription(modifier.statusEffectType)}");
    //                return;
    //            }
    //            modifierWindowText.text =
    //                ($"{modifier.modifierSource}\n" +
    //                $"{ReturnSign(modifier.statChangeType)}{modifier.modifierAmount} {modifier.statModified.ToString()}");
    //        }
    //        else
    //        {
    //            modifierStatScreenWindow.SetActive(false);
    //        }
    //    }
    //}

    //public string ReturnSign(AttackEffect.StatChangeType statChangeType)
    //{
    //    if (statChangeType == AttackEffect.StatChangeType.Buff)
    //    {
    //        return "+";
    //    }
    //    else
    //    {
    //        return "";
    //    }
    //}
}
