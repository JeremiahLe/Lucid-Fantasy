using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreateMonster : MonoBehaviour
{
    public Monster monster;
    public Monster monsterReference;

    [Header("Display Variables")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private SpriteRenderer sr;

    [SerializeField] private Monster.AIType aiType;
    [SerializeField] private Monster.AILevel aiLevel;

    [SerializeField] public Transform startingPosition;
    private enum CombatOrientation { Left, Right };
    [SerializeField] private CombatOrientation combatOrientation;

    [Header("Additional Editable Variables")]
    [SerializeReference] private int monsterSpeed;
    [SerializeReference] private int monsterLevel;

    public Animator monsterAnimator;
    public GameObject combatManagerObject;
    public CombatManagerScript combatManagerScript;
    public MonsterAttackManager monsterAttackManager;


    private void Start()
    {
        InitateStats();
        InitializeComponents();
        SetAIType();
    }

    // This function sets monster stats on HUD at battle start
    private void InitateStats()
    {
        monsterReference = Instantiate(monster); // this is needed to create instances of the scriptable objects rather than editing them
        monsterReference.aiType = aiType;

        //monsterSpeed = monster.speed; // this is needed to not edit the base scriptable objects
        monsterReference.speed = monsterSpeed;

        //monsterReference.level = monsterLevel; // optional level adjustments

        nameText.text = monster.name + ($" Lvl: {monsterReference.level}");
        healthText.text = ($"HP: {monsterReference.health.ToString()}/{monster.maxHealth.ToString()}\nSpeed: {monsterReference.speed.ToString()}");
        sr.sprite = monster.baseSprite;
    }

    // This function should be called when stats get updated
    public void UpdateStats()
    {
        healthText.text = ($"HP: {monsterReference.health.ToString()}/{monster.maxHealth.ToString()}\nSpeed: {monsterReference.speed.ToString()}");
        CheckHealth();
    } 

    // This function checks the monster's health
    public void CheckHealth()
    {
        if (monsterReference.health <= 0)
        {
            combatManagerScript.RemoveMonsterFromList(gameObject, monsterReference.aiType);
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} has been defeated!");
        }
    }

    // This function initializes a gameObjects components
    public void InitializeComponents()
    {
        monsterAnimator = GetComponent<Animator>();
        combatManagerScript = combatManagerObject.GetComponent<CombatManagerScript>();
        monsterAttackManager = combatManagerObject.GetComponent<MonsterAttackManager>();
    }

    // This function sets monster sprite orientation at battle start
    private void SetPositionAndOrientation(Transform _startPos, CombatOrientation _combatOrientation)
    {
        Debug.Log("Position set called", this); // TODO - FIX ME

        transform.position = _startPos.transform.position;
        combatOrientation = _combatOrientation;

        if (combatOrientation == CombatOrientation.Left)
        {
            sr.flipX = true;
        }
        else
        {
            sr.flipX = false;
        }
    }

    // This function applies ai-specific rules at run-time (ie. red text for name if enemy) and then sets position and orientation
    private void SetAIType()
    {
        if (monsterReference.aiType == Monster.AIType.Enemy)
        {
            nameText.color = Color.red;
            combatOrientation = CombatOrientation.Left;
            SetPositionAndOrientation(startingPosition, combatOrientation);
            monsterReference.aiLevel = aiLevel;
        }
        else if (monsterReference.aiType == Monster.AIType.Ally)
        {
            nameText.color = Color.white;
            combatOrientation = CombatOrientation.Right;
            SetPositionAndOrientation(startingPosition, combatOrientation);
            monsterReference.aiLevel = Monster.AILevel.Player;
        }
    }

    // This function ends the attack animation, and calls other scripts 
    public void AttackAnimationEnd()
    {
        monsterAnimator.SetBool("attackAnimationPlaying", false);
        monsterAttackManager.DealDamage();
    }

    // This function passes in the new target to the combatManager
    private void OnMouseEnter()
    {
        combatManagerScript.CycleTargets(gameObject);
    }
}
