using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupScript : MonoBehaviour
{
    public void OnEnable()
    {
        Invoke("OnAnimationEnd", 1.0f);
    }

    public void OnAnimationEnd()
    {
        gameObject.SetActive(false);
    }
}
