using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowMouseScript : MonoBehaviour
{
    public RectTransform canvasRectTransform;

    // Update is called once per frame
    void Update()
    {
        Vector2 anchoredPosition = Input.mousePosition / canvasRectTransform.localScale.x;
        if (anchoredPosition.x + 350 > canvasRectTransform.rect.width)
        {
            anchoredPosition.x = canvasRectTransform.rect.width - 350;
        }

        transform.position = anchoredPosition;
    }
}
