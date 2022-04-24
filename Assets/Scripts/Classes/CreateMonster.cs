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
    private CombatOrientation combatOrientation;

    [Header("Additional Editable Variables")]
    [SerializeReference] public int monsterSpeed;
    [SerializeReference] private int monsterLevel;

    [DisplayWithoutEdit] public float monsterPhysicalAttack;
    [DisplayWithoutEdit] public float monsterMagicAttack;
    [DisplayWithoutEdit] public float monsterPhysicalDefense;
    [DisplayWithoutEdit] public float monsterMagicDefense;

    [DisplayWithoutEdit] public float monsterEvasion;

    [DisplayWithoutEdit] public float monsterDamageTakenThisRound;
    [DisplayWithoutEdit] public bool monsterActionAvailable = true;

    List<MonsterAttack> ListOfMonsterAttacksReference;

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
        // this is needed to create instances of the scriptable objects rather than editing them
        monsterReference = Instantiate(monster);
        monsterReference.aiType = aiType;
        monsterReference.maxHealth = monster.health;

        // this one does affect the base scriptable object
        monster.maxHealth = monster.health;

        // Non editable init stats display
        monsterPhysicalAttack = monsterReference.physicalAttack;
        monsterMagicAttack = monsterReference.magicAttack;
        monsterPhysicalDefense = monsterReference.physicalDefense;
        monsterMagicDefense = monsterReference.magicDefense;

        monsterEvasion = monsterReference.evasion;
        monsterReference.speed = monsterSpeed; /*Random.Range(1, 10);*/

        nameText.text = monster.name + ($" Lvl: {monsterReference.level}");
        healthText.text = ($"HP: {monsterReference.health.ToString()}/{monster.maxHealth.ToString()}\nSpeed: {monsterReference.speed.ToString()}");
        sr.sprite = monster.baseSprite;
    }

    // This function should be called when stats get updated
    public void UpdateStats()
    {
        healthText.text = ($"HP: {monsterReference.health.ToString()}/{monster.maxHealth.ToString()}\nSpeed: {monsterReference.speed.ToString()}");
        CheckHealth();

        monsterPhysicalAttack = monsterReference.physicalAttack;
        monsterMagicAttack = monsterReference.magicAttack;
        monsterPhysicalDefense = monsterReference.physicalDefense;
        monsterMagicDefense = monsterReference.magicDefense;

        monsterEvasion = monsterReference.evasion;
        monsterReference.speed = monsterSpeed;
    } 

    // This function is called on round start to refresh cooldowns if needed && reset damage taken per round
    public void CheckCooldowns()
    {
        monsterDamageTakenThisRound = 0;
        monsterActionAvailable = true;

        foreach (MonsterAttack attack in monsterReference.ListOfMonsterAttacks)
        {
            if (attack.attackOnCooldown)
            {
                attack.attackCooldown -= 1;
                if (attack.attackCooldown <= 0) {
                    attack.attackOnCooldown = false;
                    attack.attackCooldown = attack.attackBaseCooldown;
                }
            }
        }
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

        // Create instances of the monster's attacks
        monsterReference.ListOfMonsterAttacks.Clear();
        foreach (MonsterAttack attack in monster.ListOfMonsterAttacks)
        {
            MonsterAttack attackInstance = Instantiate(attack);
            monsterReference.ListOfMonsterAttacks.Add(attackInstance);
        }
    }

    // This function sets monster sprite orientation at battle start
    private void SetPositionAndOrientation(Transform _startPos, CombatOrientation _combatOrientation)
    {
        // Debug.Log("Position set called", this); // TODO - FIX ME - GitHub Commenting

        transform.position = startingPosition.transform.position;

        if (monsterReference.aiType == Monster.AIType.Ally)
        {
            //transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
            //SetMonsterUIStatsRotation();
            sr.flipX = false;
        }
        else if (monsterReference.aiType == Monster.AIType.Enemy)
        {
            //transform.localRotation = new Quaternion(0f, 180f, 0f, 0f);
            sr.flipX = true;
        }
        
    }

    // This function is a temporary rotation fix to monster UI elements facing the camera
    public void SetMonsterUIStatsRotation()
    {
        nameText.transform.LookAt(Camera.main.transform);
        nameText.transform.localRotation = Quaternion.Euler(0, 180, 0);

        healthText.transform.LookAt(Camera.main.transform);
        healthText.transform.localRotation = Quaternion.Euler(0, 180, 0);
    }

    // This function applies ai-specific rules at run-time (ie. red text for name if enemy) and then sets position and orientation
    private void SetAIType()
    {
        if (monsterReference.aiType == Monster.AIType.Enemy)
        {
            nameText.color = Color.red;
            combatOrientation = CombatOrientation.Right;
            SetPositionAndOrientation(startingPosition, combatOrientation);
            monsterReference.aiLevel = aiLevel;
        }
        else if (monsterReference.aiType == Monster.AIType.Ally)
        {
            nameText.color = Color.white;
            combatOrientation = CombatOrientation.Left;
            SetPositionAndOrientation(startingPosition, combatOrientation);
            monsterReference.aiLevel = Monster.AILevel.Player;
        }
    }

    // This function ends the attack animation, and calls other scripts 
    public void AttackAnimationEnd()
    {
        monsterAnimator.SetBool("attackAnimationPlaying", false);
        monsterAttackManager.DealDamage();
        //monsterAttackManager.currentTargetedMonsterGameObject.GetComponent<Animator>().SetBool("hitAnimationPlaying", true);
    }

    // This function ends the hit animation
    public void HitAnimationEnd()
    {
        monsterAnimator.SetBool("hitAnimationPlaying", false);
    }

    // This function passes in the new target to the combatManager
    private void OnMouseEnter()
    {
        combatManagerScript.CycleTargets(gameObject);
    }
}
