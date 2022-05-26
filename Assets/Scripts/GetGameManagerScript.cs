using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetGameManagerScript : MonoBehaviour
{
    public Button thisButton;
    public GameObject GameManager;
    public AdventureManager adventureManager;

    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.FindGameObjectWithTag("GameManager");
        adventureManager = GameManager.GetComponent<AdventureManager>();
        thisButton = GetComponent<Button>();

        thisButton.onClick.AddListener(adventureManager.CheckNodeLocked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
