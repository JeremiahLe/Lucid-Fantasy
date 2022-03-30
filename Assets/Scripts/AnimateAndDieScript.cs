using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateAndDieScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        Invoke("Die", .75f);
    }

    public float frequency;
    public float amplitude;

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = transform.position;
        newPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;
        transform.position = newPos;

        Invoke("Die", .75f);
    }

    // Die
    void Die()
    {
        gameObject.SetActive(false);
    }
}
