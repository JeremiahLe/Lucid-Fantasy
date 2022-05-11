using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

public class CreateMonster : MonoBehaviour
{
    [Title("Monster ScriptableObject and Reference Clone")]
    [Required] public Monster monster;
    public Monster monsterReference;
    List<MonsterAttack> ListOfMonsterAttacksReference;

    [Title("UI Elements")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private SpriteRenderer sr;

    [Title("Monster AI and Scene Setup")]
    [SerializeField] private Monster.AIType aiType;
    [SerializeField] private Monster.AILevel aiLevel;

    [SerializeField] public Transform startingPosition;
    private enum CombatOrientation { Left, Right };
    private CombatOrientation combatOrientation;

    [Title("Combat Stats")]
    [DisplayWithoutEdit] private int monsterLevel;
    [SerializeReference] public int monsterSpeed;

    [DisplayWithoutEdit] public float monsterPhysicalAttack;
    [DisplayWithoutEdit] public float monsterMagicAttack;
    [DisplayWithoutEdit] public float monsterPhysicalDefense;
    [DisplayWithoutEdit] public float monsterMagicDefense;

    [DisplayWithoutEdit] public float monsterCritChance;
    [DisplayWithoutEdit] public float monsterEvasion;

    [Title("Combat Functions & Status")]
    [DisplayWithoutEdit] public float monsterDamageTakenThisRound;
    [DisplayWithoutEdit] public bool monsterActionAvailable = true;
    [DisplayWithoutEdit] public bool monsterRecievedStatBoostThisRound = false;
    [DisplayWithoutEdit] public bool monsterCriticallyStrikedThisRound = false;

    [DisplayWithoutEdit] public bool monsterImmuneToDebuffs = false;
    [DisplayWithoutEdit] public bool monsterImmuneToStatChanges = false;
    [DisplayWithoutEdit] public bool monsterImmuneToDamage = false;

    [DisplayWithoutEdit] public List<Modifier> ListOfModifiers;

    [Title("Components")]
    [Required] public GameObject combatManagerObject;
    public Animator monsterAnimator;
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

        monsterCritChance = monsterReference.critChance;
        monsterEvasion = monsterReference.evasion;
        monsterReference.speed = monsterSpeed; /*Random.Range(1, 10);*/

        nameText.text = monster.name + ($" Lvl: {monsterReference.level}");
        healthText.text = ($"HP: {monsterReference.health.ToString()}/{monster.maxHealth.ToString()}\nSpeed: {monsterReference.speed.ToString()}");
        sr.sprite = monster.baseSprite;
    }

    // This function should be called when stats get updated
    public void UpdateStats()
    {
        CheckStatsCap();

        healthText.text = ($"HP: {monsterReference.health.ToString()}/{monster.maxHealth.ToString()}\nSpeed: {monsterReference.speed.ToString()}");
        CheckHealth();

        monsterPhysicalAttack = monsterReference.physicalAttack;
        monsterMagicAttack = monsterReference.magicAttack;
        monsterPhysicalDefense = monsterReference.physicalDefense;
        monsterMagicDefense = monsterReference.magicDefense;

        monsterCritChance = monsterReference.critChance;
        monsterEvasion = monsterReference.evasion;
        monsterReference.speed = monsterSpeed;

        CheckStatsCap();
    } 

    // This function checks stat caps
    public void CheckStatsCap()
    {
        if (monsterSpeed < 1)
        {
            Debug.Log("Called stat minimum cap! (Speed)");
            monsterSpeed = 1;
            healthText.text = ($"HP: {monsterReference.health.ToString()}/{monster.maxHealth.ToString()}\nSpeed: {monsterReference.speed.ToString()}");
        }
    }

    // This function is called on round start to refresh cooldowns if needed && reset damage taken per round
    public void CheckCooldowns()
    {
        foreach (MonsterAttack attack in monsterReference.ListOfMonsterAttacks)
        {
            if (attack.attackOnCooldown)
            {
                attack.attackCurrentCooldown -= 1;
                if (attack.attackCurrentCooldown <= 0) {
                    attack.attackOnCooldown = false;
                    attack.attackCurrentCooldown = attack.attackBaseCooldown;
                }
            }

            // Check for resetting single called modifiers
            foreach(AttackEffect attackEffect in attack.ListOfAttackEffects)
            {
                if (attackEffect.modifierCalledOnce)
                {
                    attackEffect.modifierCalledOnce = false;
                }
            }
        }
    }

    // This function is called on round start to adjust all round start variables
    public void OnRoundStart()
    {
        CheckCooldowns(); // Check for attack cooldowns
        ResetRoundCombatVariables(); // Refresh per-round combat variables
        CheckModifiers();
    }

    // This function checks modifiers, permanent or tempoerary
    public void CheckModifiers()
    {
        foreach (Modifier modifier in monsterReference.ListOfModifiers.ToArray())
        {   
            // Check statuses
            if (modifier.statusEffect)
            {
                switch (modifier.statusEffectType)
                {
                    case (Modifier.StatusEffectType.Poisoned):
                        int poisonDamage = Mathf.RoundToInt(.10f * monsterReference.maxHealth);

                        monsterAnimator.SetBool("hitAnimationPlaying", true);
                        monsterAttackManager.soundEffectManager.AddSoundEffectToQueue(monsterAttackManager.HitSound);
                        monsterAttackManager.soundEffectManager.BeginSoundEffectQueue();

                        monsterReference.health -= poisonDamage;
                        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} is poisoned and takes {poisonDamage} damage!");
                        UpdateStats();
                        break;

                    default:
                        break;
                }
            }

            // Check temps
            if (modifier.modifierDurationType == Modifier.ModifierDurationType.Temporary)
            {
                modifier.modifierCurrentDuration -= 1;
                if (modifier.modifierCurrentDuration == 0)
                {
                    modifier.ResetModifiedStat(monsterReference, gameObject);
                    UpdateStats();
                    monsterReference.ListOfModifiers.Remove(modifier);
                }
            }
        }
    }

    // This function modifies stats by modifier value
    public void ModifyStats(AttackEffect.StatEnumToChange statToModify, Modifier modifier)
    {
        Debug.Log("Modify Stats got called!");

        switch (statToModify)
        {
            case (AttackEffect.StatEnumToChange.Evasion):
                monsterReference.evasion += modifier.modifierAmount;
                break;

            case (AttackEffect.StatEnumToChange.Speed):
                monsterSpeed += (int)modifier.modifierAmount;
                UpdateStats();
                break;

            case (AttackEffect.StatEnumToChange.MagicAttack):
                monsterReference.magicAttack += (int)modifier.modifierAmount;
                break;
            case (AttackEffect.StatEnumToChange.MagicDefense):
                monsterReference.magicDefense += (int)modifier.modifierAmount;
                break;

            case (AttackEffect.StatEnumToChange.PhysicalAttack):
                monsterReference.physicalAttack += (int)modifier.modifierAmount;
                break;

            case (AttackEffect.StatEnumToChange.PhysicalDefense):
                monsterReference.physicalDefense += (int)modifier.modifierAmount;
                break;

            case (AttackEffect.StatEnumToChange.CritChance):
                monsterReference.critChance += (int)modifier.modifierAmount;
                break;

            case (AttackEffect.StatEnumToChange.Debuffs):
                monsterImmuneToDebuffs = true;
                break;

            case (AttackEffect.StatEnumToChange.StatChanges):
                monsterImmuneToStatChanges = true;
                break;

            case (AttackEffect.StatEnumToChange.Damage):
                monsterImmuneToDamage = true;
                break;

            default:
                Debug.Log("Missing stat to modify to modifier?", this);
                break;
        }

    }

    // Refresh per-round combat variables
    public void ResetRoundCombatVariables()
    {
        monsterDamageTakenThisRound = 0;
        monsterActionAvailable = true;
        monsterRecievedStatBoostThisRound = false;
        monsterCriticallyStrikedThisRound = false;
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

    // This function ends the buff animation
    public void BuffAnimationEnd()
    {
        monsterAnimator.SetBool("buffAnimationPlaying", false);
    }

    // This function passes in the new target to the combatManager
    private void OnMouseEnter()
    {
        combatManagerScript.CycleTargets(gameObject);
    }
}
