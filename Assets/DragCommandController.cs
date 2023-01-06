using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DragCommandController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public bool ascensionWindow = false;

    public RectTransform currentTransform;
    private GameObject mainContent;
    private Vector3 currentPossition;

    public MonsterStatScreenScript monsterStatScreenScript;
    public InventoryManager inventoryManager;

    public MonsterAttack monsterAttackReference;

    private int totalChild;

    public void OnPointerDown(PointerEventData eventData)
    {
        currentPossition = currentTransform.position;
        mainContent = currentTransform.parent.gameObject;
        totalChild = mainContent.transform.childCount;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // We shouldn't be allowed to adjust the commands of a monster we are viewing that we don't own
        if (!monsterStatScreenScript.currentMonster.monsterIsOwned)
            return;
        
        // Grab the current position of the command we are dargging
        currentTransform.position =
            new Vector3(currentTransform.position.x, eventData.position.y, currentTransform.position.z);

        // Iterate through each command in the list, and swap their indexes if they are moved
        for (int i = 0; i < totalChild; i++)
        {
            if (i != currentTransform.GetSiblingIndex())
            {
                Transform otherTransform = mainContent.transform.GetChild(i);

                int distance = (int)Vector3.Distance(currentTransform.position,
                    otherTransform.position);

                if (distance <= 10)
                {
                    Vector3 otherTransformOldPosition = otherTransform.position;
                    otherTransform.position = new Vector3(otherTransform.position.x, currentPossition.y,
                        otherTransform.position.z);
                    currentTransform.position = new Vector3(currentTransform.position.x, otherTransformOldPosition.y,
                        currentTransform.position.z);
                    currentTransform.SetSiblingIndex(otherTransform.GetSiblingIndex());
                    currentPossition = currentTransform.position;

                    // If in the ascension window, adjust the inventory manager commands rather than the stats screen commands
                    if (ascensionWindow)
                    {
                        // If the current dragged command is being replaced, only update the other command
                        if (currentTransform.GetSiblingIndex() == 4)
                        {
                            inventoryManager.currentMonster.ListOfMonsterAttacks[otherTransform.GetSiblingIndex()] = otherTransform.GetComponent<DragCommandController>().monsterAttackReference;
                            continue;
                        }

                        // If the other command is being replaced, only update the current command
                        if (otherTransform.GetSiblingIndex() == 4)
                        {
                            inventoryManager.currentMonster.ListOfMonsterAttacks[currentTransform.GetSiblingIndex()] = currentTransform.GetComponent<DragCommandController>().monsterAttackReference;
                            continue;
                        }

                        inventoryManager.currentMonster.ListOfMonsterAttacks[currentTransform.GetSiblingIndex()] = currentTransform.GetComponent<DragCommandController>().monsterAttackReference;
                        inventoryManager.currentMonster.ListOfMonsterAttacks[otherTransform.GetSiblingIndex()] = otherTransform.GetComponent<DragCommandController>().monsterAttackReference;
                        continue;
                    }

                    // Not in the ascension window, adjust the stats screen commands
                    monsterStatScreenScript.currentMonster.ListOfMonsterAttacks[currentTransform.GetSiblingIndex()] = currentTransform.GetComponent<DragCommandController>().monsterAttackReference;
                    monsterStatScreenScript.currentMonster.ListOfMonsterAttacks[otherTransform.GetSiblingIndex()] = otherTransform.GetComponent<DragCommandController>().monsterAttackReference;
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        currentTransform.position = currentPossition;
    }
}