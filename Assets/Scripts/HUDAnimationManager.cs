using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDAnimationManager : MonoBehaviour
{
    public TextMeshProUGUI MonsterCurrentTurnText;
    public ScrollRect vertScroll;

    public float frequency;
    public float amplitude;

    void Update()
    {
        HoverText(MonsterCurrentTurnText);
    }

    // This function animates a text object passed in with a simple hover animation
    void HoverText(TextMeshProUGUI textToHover)
    {
        Vector3 newPos = textToHover.transform.position;
        newPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;
        textToHover.transform.position = newPos;
    }

    public void UpdateScrollBar()
    {
        Canvas.ForceUpdateCanvases();

        vertScroll.verticalNormalizedPosition = 0f;

        Canvas.ForceUpdateCanvases();
    }
}
