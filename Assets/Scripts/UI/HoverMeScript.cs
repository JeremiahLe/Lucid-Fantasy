using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HoverMeScript : MonoBehaviour
{
    public float frequency;
    public float amplitude;
    public bool menuItem = false;

    public Transform startPos;

    private void OnEnable()
    {
        if (menuItem)
        {
            transform.position = startPos.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = transform.position;
        newPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;
        transform.position = newPos;
    }

    public void ResetText()
    {
        TextMeshProUGUI textMesh = GetComponent<TextMeshProUGUI>();
        textMesh.text = "";
    }
}
