using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasDetachAndDieScript : MonoBehaviour
{
    private void OnTransformParentChanged()
    {
        Debug.Log("Transform parent changed!");
        Invoke("Die", 0.82f);
    }

    public void Die()
    {
        Debug.Log("Called die!");
        Destroy(gameObject);
    }

}
