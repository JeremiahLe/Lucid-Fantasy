using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.EventSystems;

public class Interactable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject interactableDescriptionWindow;

    public enum TypeOfInteractable { Modifier, Stat, UI }
    public TypeOfInteractable typeOfInteractable;

    public Modifier modifier;
    public AttackEffect.StatEnumToChange stat;

    public string interactableName;
    [TextArea]
    public string interactableDescription;

    public TextMeshProUGUI interactableText;

    public void Start()
    {
        InitiateInteractable(modifier);
    }

    public void InitiateInteractable(Modifier mod)
    {
        if (mod != null && typeOfInteractable == TypeOfInteractable.Modifier)
        {
            modifier = mod;
            interactableName = mod.modifierName;
            interactableDescription = mod.modifierDescription;
        }
        else if (typeOfInteractable == TypeOfInteractable.Stat)
        {
            interactableName = ($"{stat.ToString()}");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (modifier == null && typeOfInteractable == TypeOfInteractable.Modifier)
            return;

        interactableDescriptionWindow.SetActive(true);
        interactableText.text = 
            ($"<b>{interactableName}</b>" +
            $"\n{interactableDescription}");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        interactableDescriptionWindow.SetActive(false);
    }

    public void ResetInteractable()
    {
        modifier = null;
        interactableText.text = ("");
    }

    public void ShowInteractable(string message)
    {
        interactableDescriptionWindow.SetActive(true);
        interactableText.text =
            ($"{message}");
    }
}
