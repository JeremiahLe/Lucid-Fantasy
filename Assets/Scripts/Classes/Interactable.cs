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

    public enum TypeOfInteractable { Modifier, Stat, UI, Node, Item, Elements }
    public TypeOfInteractable typeOfInteractable;

    public Modifier modifier;
    public AttackEffect.StatEnumToChange stat;
    public CreateNode node;
    public Item item;

    public string interactableName;
    [TextArea]
    public string interactableDescription;

    public TextMeshProUGUI interactableText;

    public void Start()
    {
        InitiateInteractable(modifier);
    }

    public void SetUIInteractable(string _interactableName, string _interactableDescription)
    {
        interactableName = _interactableName;
        interactableDescription = _interactableDescription;
    }

    public void InitiateInteractable(Modifier mod)
    {
        interactableText = interactableDescriptionWindow.GetComponentInChildren<TextMeshProUGUI>();

        if (mod != null && typeOfInteractable == TypeOfInteractable.Modifier)
        {
            modifier = mod;
            interactableName = mod.modifierName;
            interactableDescription = mod.modifierDescription;

            if (mod.modifierOwner != null && mod.adventureEquipment)
            {
                interactableDescription += ($"\nEquipped by: {mod.modifierOwner.name}");
            }
        }
        else if (typeOfInteractable == TypeOfInteractable.Stat)
        {
            interactableName = ($"{stat.ToString()}");
        }
        else if (typeOfInteractable == TypeOfInteractable.Node)
        {
            node = GetComponent<CreateNode>();
            interactableName = ($"{node.nodeName}");
            interactableDescription = ($"{node.nodeDescription}");
        }
        else if (item != null && typeOfInteractable == TypeOfInteractable.Item)
        {
            interactableName = item.itemName;
            interactableDescription = item.itemDescription;
        }
    }

    public void InitiateInteractable(Item _item)
    {
        item = _item;
        typeOfInteractable = TypeOfInteractable.Item;
        interactableName = item.itemName;
        interactableDescription = item.itemDescription;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (modifier == null && typeOfInteractable == TypeOfInteractable.Modifier)
            return;

        if (item == null && typeOfInteractable == TypeOfInteractable.Item)
            return;

        if (typeOfInteractable == TypeOfInteractable.Elements)
        {
            interactableDescriptionWindow.SetActive(true);
            interactableText.text =
                ($"{interactableDescription}");
            return;
        }

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
        item = null;
        interactableName = "";
        interactableDescription = "";
        interactableText.text = ("");
        interactableDescriptionWindow.SetActive(false);
    }

    public void ShowInteractable(string message)
    {
        interactableDescriptionWindow.SetActive(true);
        interactableText.text =
            ($"{message}");
    }
}
