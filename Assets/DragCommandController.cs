using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragCommandController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform currentTransform;
    private GameObject mainContent;
    private Vector3 currentPossition;

    public MonsterStatScreenScript monsterStatScreenScript;
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
        if (!monsterStatScreenScript.currentMonster.monsterIsOwned)
            return;
        
        currentTransform.position =
            new Vector3(currentTransform.position.x, eventData.position.y, currentTransform.position.z);

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
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        currentTransform.position = currentPossition;

        // Assign attack move order
        for (int i = 0; i < totalChild; i++)
        {
            monsterStatScreenScript.currentMonster.ListOfMonsterAttacks[i] = mainContent.transform.GetChild(i).GetComponent<DragCommandController>().monsterAttackReference;
            monsterStatScreenScript.monsterAttackHolders[i] = mainContent.transform.GetChild(i).gameObject;
        }
    }
}