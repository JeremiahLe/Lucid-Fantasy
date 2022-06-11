using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine.UI;

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

    public Image nodeImage;
    public Sprite unlockedSprite;
    public Sprite lockedSprite;
    public TextMeshProUGUI nodeName;

    [Title("Game Manager Data")]
    public GameObject GameManager;
    public AdventureManager adventureManager;

    [Title("Node Data")]
    public enum NodeType { Start, RandomCombat, PresetCombat, RandomReward, MonsterReward, ModifierReward, Shop, Boss, End, EquipmentReward }
    public NodeType nodeType;

    public enum NodeState { Locked, Unlocked }
    public NodeState nodeState;

    public bool nodeInDefaultState = true;
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
        GetSceneComponents();

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

    public void GetSceneComponents()
    {
        GameManager = GameObject.FindGameObjectWithTag("GameManager");
        adventureManager = GameManager.GetComponent<AdventureManager>();

        nodeSelectionTargeter = adventureManager.nodeSelectionTargeter;
        routeText = adventureManager.routeText;

        sr = GetComponent<SpriteRenderer>();
    }

    // This function sets the node to the passed state
    public void SetNodeState(NodeState newState)
    {
        switch (newState)
        {
            case (NodeState.Locked):
                nodeImage.color = Color.red;
                nodeImage.sprite = lockedSprite;
                nodeState = NodeState.Locked;
                break;

            case (NodeState.Unlocked):
                nodeImage.color = Color.white;
                nodeImage.sprite = unlockedSprite;
                nodeState = NodeState.Unlocked;
                break;

            default:
                break;
        }

        SetNodeName();
    }

    // This function handles node naming
    public void SetNodeName()
    {
        //nodeName.text = ($"{nodeType.ToString()}");

        
        switch (nodeType)
        {
            case (NodeType.Start):
                nodeName.text = ($"Start");
                break;

            case (NodeType.Boss):
                nodeName.text = ($"Boss");
                break;

            case (NodeType.RandomCombat):
                nodeName.text = ($"Combat");
                break;

            case (NodeType.EquipmentReward):
                nodeName.text = ($"Equipment");
                break;

            case (NodeType.ModifierReward):
                nodeName.text = ($"Modifier");
                break;

            case (NodeType.MonsterReward):
                nodeName.text = ($"Monster");
                break;

            case (NodeType.Shop):
                nodeName.text = ($"Shop");
                break;

            default:
                Debug.Log("Missing node state or name ref?", this);
                break;
        }
        
    }

    // This function is called when a node is unlocked
    IEnumerator NodeUnlockAnimation()
    {
        yield return new WaitForSeconds(1f);
        GetComponent<Animator>().SetBool("unlocked", true);
    }

    private void OnEnable()
    {
        routeText = adventureManager.routeText;
    }

    // This function passes in the new target to the combatManager
    private void OnMouseEnter()
    {
        nodeSelectionTargeter.transform.position = selectedPosition.transform.position;
        adventureManager.currentSelectedNode = gameObject;
    }

    // this function runs on click
    public void CheckNodeLocked()
    {
        if (adventureManager != null && nodeState == NodeState.Unlocked)
        {
            adventureManager.CheckNodeLocked();
            return;
        }

        routeText.text = "Destination is locked!";
    }
}
