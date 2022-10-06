using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupScript : MonoBehaviour
{
    public bool instantiated = false;
    public GameObject parentObj = null;
    public float deathTime;

    public void OnEnable()
    {
        deathTime = 1.25f;
        Invoke("OnAnimationEnd", deathTime);
    }

    public void OnAnimationEnd()
    {
        if (instantiated)
        {
            Destroy(parentObj);
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
