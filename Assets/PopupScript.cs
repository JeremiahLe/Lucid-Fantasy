using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupScript : MonoBehaviour
{
    public bool instantiated = false;
    public GameObject parentObj = null;

    public void OnEnable()
    {
        float deathTime = Random.Range(0.75f, 1.5f);
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
