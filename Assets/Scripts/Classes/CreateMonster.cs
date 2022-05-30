using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using UnityEngine.UI;

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

    [SerializeField] public Image statusEffectUISprite;

    [SerializeField] public GameObject StatScreenWindowGameObject;
    [SerializeField] public Image StatScreenWindow;
    [SerializeField] public TextMeshProUGUI StatScreenWindowText;

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

    [DisplayWithoutEdit] public bool monsterIsPoisoned = false;
    [DisplayWithoutEdit] public bool monsterIsBurning = false;
    [DisplayWithoutEdit] public bool monsterIsDazed = false;
    [DisplayWithoutEdit] public bool monsterIsCrippled = false;
    [DisplayWithoutEdit] public bool monsterIsWeakened = false;

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
        combatManagerScript = combatManagerObject.GetComponent<CombatManagerScript>();

        // this is needed to create instances of the scriptable objects rather than editing them
        if (!combatManagerScript.adventureMode)
        {
            monsterReference = Instantiate(monster);
            monsterReference.aiType = aiType;
            monsterReference.maxHealth = monster.health;

            // Clear cache
            monster.cachedPhysicalAttack = monsterReference.physicalAttack;
            monster.cachedMagicAttack = monsterReference.magicAttack;

            monster.cachedPhysicalDefense = monsterReference.physicalDefense;
            monster.cachedMagicDefense = monsterReference.magicDefense;

            monster.cachedSpeed = monsterReference.speed;
            monster.cachedEvasion = monsterReference.evasion;
            monster.cachedCritChance = monsterReference.critChance;
        }

        // if adevnture mode, cache stats only for Player
        if (combatManagerScript.adventureMode)
        {
            monsterReference = monster;
            monster.cachedPhysicalAttack = monsterReference.physicalAttack;
            monster.cachedMagicAttack = monsterReference.magicAttack;

            monster.cachedPhysicalDefense = monsterReference.physicalDefense;
            monster.cachedMagicDefense = monsterReference.magicDefense;

            monster.cachedSpeed = monsterReference.speed;
            monster.cachedEvasion = monsterReference.evasion;
            monster.cachedCritChance = monsterReference.critChance;

            //monster.maxHealth = monster.health; not needed?
        }

        // Non editable init stats display
        monsterPhysicalAttack = monsterReference.physicalAttack;
        monsterMagicAttack = monsterReference.magicAttack;
        monsterPhysicalDefense = monsterReference.physicalDefense;
        monsterMagicDefense = monsterReference.magicDefense;

        monsterCritChance = monsterReference.critChance;
        monsterEvasion = monsterReference.evasion;
        monsterSpeed = (int)monsterReference.speed; /*Random.Range(1, 10);*/

        nameText.text = monster.name + ($" Lvl: {monsterReference.level}");
        healthText.text = ($"HP: {monsterReference.health.ToString()}/{monster.maxHealth.ToString()}\nSpeed: {monsterReference.speed.ToString()}");
        sr.sprite = monster.baseSprite;
    }

    //
    public void CheckAdventureModifiers()
    {
        // if adventure mode, check adventure modifiers
        if (combatManagerScript.adventureMode && monster.aiType == Monster.AIType.Ally)
        {
            combatManagerScript.adventureManager.ApplyAdventureModifiers(monster);
            CheckAdventureEquipment();
            UpdateStats(false);
        }
    }

    //
    public void CheckAdventureEquipment()
    {
        foreach (Modifier equipment in monsterReference.ListOfModifiers)
        {
            if (equipment.adventureEquipment)
            {
                ModifyStats(equipment.statModified, equipment, "adventure");
                UpdateStats(false);
            }
        }
    }


    // This function should be called when stats get updated
    public void UpdateStats(bool damageTaken)
    {
        CheckStatsCap();

        healthText.text = ($"HP: {monsterReference.health.ToString()}/{monster.maxHealth.ToString()}\nSpeed: {monsterReference.speed.ToString()}");

        // Fix dual calls
        if (damageTaken)
        {
            CheckHealth();
        }

        monsterPhysicalAttack = monsterReference.physicalAttack;
        monsterMagicAttack = monsterReference.magicAttack;
        monsterPhysicalDefense = monsterReference.physicalDefense;
        monsterMagicDefense = monsterReference.magicDefense;

        monsterCritChance = monsterReference.critChance;
        monsterEvasion = monsterReference.evasion;
        monsterSpeed = (int)monsterReference.speed;

        CheckStatsCap();
    }

    // This function checks stat caps
    public void CheckStatsCap()
    {
        if (monsterReference.speed < 1)
        {
            monsterReference.speed = 1;
            healthText.text = ($"HP: {monsterReference.health.ToString()}/{monster.maxHealth.ToString()}\nSpeed: {monsterReference.speed.ToString()}");
        }

        if (monsterPhysicalAttack < 1)
        {
            monsterPhysicalAttack = 1;
        }

        if (monsterMagicAttack < 1)
        {
            monsterMagicAttack = 1;
        }

        if (monsterPhysicalDefense < 1)
        {
            monsterPhysicalDefense = 1;
        }

        if (monsterMagicDefense < 1)
        {
            monsterMagicDefense = 1;
        }

        if (monsterCritChance < 1)
        {
            monsterCritChance = 0;
        }

        if (monsterReference.critDamage > 2.5f)
        {
            monsterReference.critDamage = 2.5f;
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
            foreach (AttackEffect attackEffect in attack.ListOfAttackEffects)
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
                        monsterIsPoisoned = true;
                        statusEffectUISprite.sprite = monsterAttackManager.poisonedUISprite;
                        int poisonDamage = Mathf.RoundToInt(modifier.modifierAmount * monsterReference.maxHealth);

                        monsterAnimator.SetBool("hitAnimationPlaying", true);
                        monsterAttackManager.soundEffectManager.AddSoundEffectToQueue(monsterAttackManager.HitSound);
                        monsterAttackManager.soundEffectManager.BeginSoundEffectQueue();

                        monsterReference.health -= poisonDamage;
                        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} is poisoned and takes {poisonDamage} damage!");
                        UpdateStats(true);
                        break;

                    case (Modifier.StatusEffectType.Burning):
                        monsterIsBurning = true;
                        statusEffectUISprite.sprite = monsterAttackManager.burningUISprite;
                        int burningDamage = Mathf.RoundToInt(modifier.modifierAmount * monsterReference.health);
                        if (burningDamage < 1)
                        {
                            burningDamage = 1; // Fix zero damage burn
                        }

                        monsterAnimator.SetBool("hitAnimationPlaying", true);
                        monsterAttackManager.soundEffectManager.AddSoundEffectToQueue(monsterAttackManager.HitSound);
                        monsterAttackManager.soundEffectManager.BeginSoundEffectQueue();

                        monsterReference.health -= burningDamage;
                        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} is burning and takes {burningDamage} damage!");
                        UpdateStats(true);
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
                    UpdateStats(false);
                    monsterReference.ListOfModifiers.Remove(modifier);
                }
            }
        }
    }

    // This function modifies stats by modifier value
    public void ModifyStats(AttackEffect.StatEnumToChange statToModify, Modifier modifier)
    {
        //Debug.Log("Modify Stats got called!");

        switch (statToModify)
        {
            case (AttackEffect.StatEnumToChange.Evasion):
                monsterReference.evasion += modifier.modifierAmount;
                break;

            case (AttackEffect.StatEnumToChange.Speed):
                monsterReference.speed += (int)modifier.modifierAmount;
                UpdateStats(false);
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

            case (AttackEffect.StatEnumToChange.CritDamage):
                monsterReference.critDamage += modifier.modifierAmount;
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

            case (AttackEffect.StatEnumToChange.BothOffensiveStats):
                monsterReference.physicalAttack += (int)modifier.modifierAmount;
                monsterReference.magicAttack += (int)modifier.modifierAmount;
                break;

            default:
                Debug.Log("Missing stat to modify to modifier?", this);
                break;
        }

    }

    // This function modifies stats by modifier value
    public void ModifyStats(AttackEffect.StatEnumToChange statToModify, Modifier modifier, string equipmentName)
    {
        Debug.Log("Modify Stats with equipment got called!");

        // If buff is not flat, calculate buff
        if (!modifier.modifierAmountFlatBuff)
        {
            modifier.modifierAmount = Mathf.RoundToInt((modifier.modifierAmount / 100f) * GetStatToChange(statToModify, monsterReference));

            if (modifier.modifierAmount < 1)
            {
                modifier.modifierAmount = 1;
            }
        }

        switch (statToModify)
        {
            case (AttackEffect.StatEnumToChange.Evasion):
                monsterReference.evasion += modifier.modifierAmount;
                break;

            case (AttackEffect.StatEnumToChange.Speed):
                monsterReference.speed += (int)modifier.modifierAmount;
                UpdateStats(false);
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

            case (AttackEffect.StatEnumToChange.CritDamage):
                monsterReference.critDamage += (int)modifier.modifierAmount;
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

            case (AttackEffect.StatEnumToChange.BothOffensiveStats):
                monsterReference.physicalAttack += (int)modifier.modifierAmount;
                monsterReference.magicAttack += (int)modifier.modifierAmount;
                break;

            default:
                Debug.Log("Missing stat to modify to modifier?", this);
                break;
        }

        // Send log message
        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {statToModify} was increased by {modifier.modifierName}!");
    }

    // This function inflicts statuses
    public void InflictStatus(Modifier.StatusEffectType statusEffect)
    {
        switch (statusEffect)
        {
            case (Modifier.StatusEffectType.Poisoned):
                monsterIsPoisoned = true;
                statusEffectUISprite.sprite = monsterAttackManager.poisonedUISprite;
                break;

            case (Modifier.StatusEffectType.Burning):
                monsterIsBurning = true;
                statusEffectUISprite.sprite = monsterAttackManager.burningUISprite;
                break;

            default:
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
            // remove statuses to prevent two status death call bug
            monsterIsPoisoned = false;
            monsterIsBurning = false;

            combatManagerScript.RemoveMonsterFromList(gameObject, monsterReference.aiType);
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} has been defeated!");
        }
    }

    // This function initializes a gameObjects components
    public void InitializeComponents()
    {
        monsterAnimator = GetComponent<Animator>();
        //combatManagerScript = combatManagerObject.GetComponent<CombatManagerScript>();
        monsterAttackManager = combatManagerObject.GetComponent<MonsterAttackManager>();

        // Create instances of the monster's attacks
        if (!combatManagerScript.adventureMode)
        {
            monsterReference.ListOfMonsterAttacks.Clear();
            foreach (MonsterAttack attack in monster.ListOfMonsterAttacks)
            {
                MonsterAttack attackInstance = Instantiate(attack);
                monsterReference.ListOfMonsterAttacks.Add(attackInstance);
            }
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

    public float delayTime = 0.01f;
    public float currentTime = 0.0f;
    public bool windowShowing = false;

    // This function passes in the new target to the combatManager
    private void OnMouseEnter()
    {
        combatManagerScript.CycleTargets(gameObject);
        if (currentTime < delayTime)
        {
            return;
        }
        DisplayStatScreenWindow(true);
    }

    private void OnMouseOver()
    {
        if (currentTime >= delayTime)
        {
            if (!windowShowing)
            {
                DisplayStatScreenWindow(true);
                windowShowing = true;
            }
        }
        else
        {
            currentTime += Time.deltaTime;
        }
    }

    // This function passes in the new target to the combatManager
    private void OnMouseExit()
    {
        windowShowing = false;
        currentTime = 0.0f;
        combatManagerScript.CycleTargets(gameObject);
        DisplayStatScreenWindow(false);
    }

    // This function is called when the mouse hovers the monster, bringing up the stat screen window
    public void DisplayStatScreenWindow(bool showWindow)
    {
        if (showWindow)
        {
            StatScreenWindowGameObject.SetActive(true);
            StatScreenWindowText.text =
                ($"Elements: {monster.monsterElement}/{monster.monsterSubElement}" +
                $"\nPhysical Attack: {monsterReference.physicalAttack} ({ReturnSign(monsterReference.physicalAttack, monsterReference.cachedPhysicalAttack)}{monsterReference.physicalAttack - monsterReference.cachedPhysicalAttack})" +
                $"\nMagic Attack: {monsterReference.magicAttack} ({ReturnSign(monsterReference.magicAttack, monsterReference.cachedMagicAttack)}{monsterReference.magicAttack - monsterReference.cachedMagicAttack})" +
                $"\nPhysical Defense: {monsterReference.physicalDefense} ({ReturnSign(monsterReference.physicalDefense, monsterReference.cachedPhysicalDefense)}{monsterReference.physicalDefense - monsterReference.cachedPhysicalDefense})" +
                $"\nMagic Defense: {monsterReference.magicDefense} ({ReturnSign(monsterReference.magicDefense, monsterReference.cachedMagicDefense)}{monsterReference.magicDefense - monsterReference.cachedMagicDefense})" +
                $"\nEvasion: {monsterReference.evasion} ({ReturnSign(monsterReference.evasion, monsterReference.cachedEvasion)}{monsterReference.evasion - monsterReference.cachedEvasion})" +
                $"\nCrit Chance: {monsterReference.critChance} (+{monsterReference.critChance - monsterReference.cachedCritChance})");
        }
        else
        if (!showWindow)
        {
            StatScreenWindowGameObject.SetActive(false);
            StatScreenWindowText.text = "";
        }
    }

    // Return negative or positive sign
    public string ReturnSign(float currentStat, float baseStat)
    {
        // if currentStat is smaller than baseStat, must be stat debuffed
        if (currentStat < baseStat)
        {
            return "";
        }

        // regular buff
        return "+";
    }

    // Get bonus damage source
    float GetStatToChange(AttackEffect.StatEnumToChange statEnumToChange, Monster monsterRef)
    {
        switch (statEnumToChange)
        {
            case (AttackEffect.StatEnumToChange.Speed):
                return monsterRef.speed;

            case (AttackEffect.StatEnumToChange.PhysicalAttack):
                return monsterRef.physicalAttack;

            case (AttackEffect.StatEnumToChange.PhysicalDefense):
                return monsterRef.physicalDefense;

            case (AttackEffect.StatEnumToChange.MagicAttack):
                return monsterRef.magicAttack;

            case (AttackEffect.StatEnumToChange.MagicDefense):
                return monsterRef.magicDefense;

            case (AttackEffect.StatEnumToChange.Evasion):
                return monsterRef.evasion;

            case (AttackEffect.StatEnumToChange.CritChance):
                return monsterRef.critChance;

            case (AttackEffect.StatEnumToChange.CritDamage):
                return monsterRef.critDamage;
            default:
                Debug.Log("Missing stat or monster reference?", this);
                return 0;
        }

    }
}
