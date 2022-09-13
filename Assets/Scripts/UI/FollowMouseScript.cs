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

        if (anchoredPosition.x + 460 > canvasRectTransform.rect.width)
        {
            anchoredPosition.x = canvasRectTransform.rect.width - 460;
        }

        if (anchoredPosition.y + 120 > canvasRectTransform.rect.height)
        {
            anchoredPosition.y = canvasRectTransform.rect.height - 120;
        }

        transform.position = anchoredPosition;

        if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1))
        {
            gameObject.SetActive(false);
        }
    }
}
