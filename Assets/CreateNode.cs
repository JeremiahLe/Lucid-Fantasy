using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

public class CreateNode : MonoBehaviour
{
    [Title("This Object Data")]
    public GameObject previousNode;
    public GameObject alternativePreviousNode;

    public GameObject nextNode;
    public GameObject alternativeNextNode;

    public GameObject nodeSelectionTargeter;
    public Transform selectedPosition;

    public SpriteRenderer sr;
    public TextMeshProUGUI routeText;

    [Title("Game Manager Data")]
    public GameObject GameManager;
    public AdventureManager adventureManager;

    [Title("Node Data")]
    public enum NodeType { Start, RandomCombat, PresetCombat, RandomReward, MonsterReward, ModifierReward, Shop, Boss, End }
    public NodeType nodeType;

    public bool nodeLocked;

    public List<GameObject> nodesToUnlock;
    public List<GameObject> nodesToLock;

    #region LineCode
    // Apply these values in the editor
    //public LineRenderer LineRenderer;
    //public Transform TransformOne;
    //public Transform TransformTwo;
    //public Transform TransformThree;
    #endregion

    void Awake()
    {
        GameManager = GameObject.FindGameObjectWithTag("GameManager");
        adventureManager = GameManager.GetComponent<AdventureManager>();

        nodeSelectionTargeter = adventureManager.nodeSelectionTargeter;
        selectedPosition = GetComponentInChildren<Transform>();

        sr = GetComponent<SpriteRenderer>();

        if (nodeLocked)
        {
            sr.color = Color.red;
        }

        #region
        /*
        LineRenderer = GetComponent<LineRenderer>();
        TransformOne = transform;

        if (nextNode != null)
        {
            TransformTwo = nextNode.transform;
        }

        if (alternativeNextNode != null)
        {
            TransformThree = alternativeNextNode.transform;
        }

        // set the color of the line
        LineRenderer.startColor = Color.red;
        LineRenderer.endColor = Color.red;

        // set width of the renderer
        LineRenderer.startWidth = 0.1f;
        LineRenderer.endWidth = 0.1f;

        // set the position
        if (TransformTwo != null)
        {
            LineRenderer.SetPosition(0, TransformOne.position);
            LineRenderer.SetPosition(1, TransformTwo.position);
        }

        if (TransformThree != null)
        {
            // New line
            LineRenderer newLine = new LineRenderer();
            // set the color of the line
            LineRenderer.startColor = Color.red;
            LineRenderer.endColor = Color.red;
            // set width of the renderer
            LineRenderer.startWidth = 0.1f;
            LineRenderer.endWidth = 0.1f;

            newLine.SetPosition(0, TransformOne.position);
            newLine.SetPosition(1, TransformThree.position);
        }
        */
        #endregion
    }

    private void OnEnable()
    {
        nodeSelectionTargeter = adventureManager.nodeSelectionTargeter;
        selectedPosition = GetComponentInChildren<Transform>();

        sr = GetComponent<SpriteRenderer>();

        if (nodeLocked)
        {
            sr.color = Color.red;
        }
    }

    // This function passes in the new target to the combatManager
    private void OnMouseEnter()
    {
        nodeSelectionTargeter.transform.position = new Vector3(selectedPosition.transform.position.x, selectedPosition.transform.position.y + 1.55f, selectedPosition.transform.position.z);
        adventureManager.currentSelectedNode = gameObject;
    }

    // this function runs on click
    public void CheckNodeLocked()
    {
        if (nodeLocked)
        {
            routeText.text = ($"Route is locked! Select previous route first!");
        }
    }
}
