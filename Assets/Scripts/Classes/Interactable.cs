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

    public enum TypeOfInteractable { Modifier, Stat }
    public TypeOfInteractable typeOfInteractable;

    public Modifier modifier;
    public AttackEffect.StatEnumToChange stat;

    public string interactableName;
    [TextArea]
    public string interactableDescription;

    public TextMeshProUGUI interactableText;

    public void Start()
    {
        if (typeOfInteractable == TypeOfInteractable.Modifier)
        {
            interactableName = modifier.modifierName;
            interactableDescription = modifier.modifierDescription;
        }
        else
        {
            interactableName = ($"{stat.ToString()}");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        interactableDescriptionWindow.SetActive(true);
        interactableText.text = 
            ($"<b>{interactableName}</b>" +
            $"\n{interactableDescription}");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        interactableDescriptionWindow.SetActive(false);
    }
}
