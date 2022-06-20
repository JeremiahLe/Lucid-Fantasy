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
    [SerializeField] private GameObject monsterCanvas;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private SpriteRenderer sr;

    [SerializeField] public Image statusEffectUISprite;

    [SerializeField] public GameObject StatScreenWindowGameObject;
    [SerializeField] public Image StatScreenWindow;
    [SerializeField] public TextMeshProUGUI StatScreenWindowText;

    [SerializeField] public Transform popupPosTransform;
    [SerializeField] private GameObject monsterStatusTextObjectCanvas;

    [SerializeField] public Slider HealthbarSlider;
    [SerializeField] public Slider HealthbarSliderDamaged;

    Color HealthbarSliderOriginalColor;
    Color HealthbarSliderTargetColor;
    Color damagedColor;

    const float DAMAGED_HEALTH_FADE_TIMER_MAX = .75f;
    float damagedHealthFadeTimer;

    [SerializeField] public Image HealthbarSliderFill;
    [SerializeField] public Image HealthbarSliderFillDamagedFade;

    [SerializeField] public GameObject monsterStatusTextObject;
    [SerializeField] public TextMeshProUGUI monsterStatusText;

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

    public bool monsterImmuneToDebuffs = false;
    public bool monsterImmuneToStatChanges = false;
    public bool monsterImmuneToDamage = false;

    public bool monsterIsPoisoned = false;
    public bool monsterIsBurning = false;
    public bool monsterIsDazed = false;
    public bool monsterIsCrippled = false;
    public bool monsterIsWeakened = false;

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

        InitiateCreateMonsterObjectStats();

        InitiateHealthBars();
    }

    public void InitiateCreateMonsterObjectStats()
    {
        // Non editable init stats display
        monsterPhysicalAttack = monsterReference.physicalAttack;
        monsterMagicAttack = monsterReference.magicAttack;
        monsterPhysicalDefense = monsterReference.physicalDefense;
        monsterMagicDefense = monsterReference.magicDefense;

        monsterCritChance = monsterReference.critChance;
        monsterEvasion = monsterReference.evasion;
        monsterSpeed = (int)monsterReference.speed; /*Random.Range(1, 10);*/

        nameText.text = monster.name + ($" Lvl: {monsterReference.level}");
        healthText.text = ($"{monsterReference.health.ToString()}/{monster.maxHealth.ToString()}"); //\nSpeed: {monsterReference.speed.ToString()}
        sr.sprite = monster.baseSprite;

        InitiateHealthBars();
    }

    public void InitiateHealthBars()
    {
        // Initiate healthbars
        HealthbarSlider.maxValue = monsterReference.maxHealth;
        HealthbarSlider.value = monsterReference.health;

        HealthbarSliderDamaged.value = HealthbarSlider.value;
        HealthbarSliderDamaged.maxValue = HealthbarSlider.maxValue;

        // Set damaged health bar alpha to - and color to white
        damagedColor.a = 0f;
        damagedColor = Color.white;
        HealthbarSliderFillDamagedFade.color = damagedColor;
    }

    private void Update()
    {
        if (damagedColor.a > 0)
        {
            damagedHealthFadeTimer -= Time.deltaTime;
            if (damagedHealthFadeTimer < 0)
            {
                float fadeAmount = 5f;
                damagedColor.a -= fadeAmount * Time.deltaTime;
                HealthbarSliderFillDamagedFade.color = damagedColor;
                HealthbarSliderDamaged.value = monsterReference.health;
            }
        }
    }

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

    public void UpdateHealthBar(bool damageTaken)
    {
        if (damageTaken)
        {
            //HealthbarSliderDamaged.value = monsterReference.health;

            // Should also be called when healed
            HealthbarSlider.value = monsterReference.health;
            HealthbarSliderFill.color = new Color(Mathf.Clamp((1 - monsterReference.health / monsterReference.maxHealth), 0, .75f), Mathf.Clamp((monsterReference.health / monsterReference.maxHealth), 0, .75f), 0, 1f);

            if (damagedColor.a <= 0)
            {
                //HealthbarSliderFillDamagedFade.fillAmount = HealthbarSlider.value;
                //StartCoroutine("FadeHealthbarDamageTaken", 0.1f);
            }

            damagedColor.a = 1;
            HealthbarSliderFillDamagedFade.color = damagedColor;
            damagedHealthFadeTimer = DAMAGED_HEALTH_FADE_TIMER_MAX;
        }
    }

    IEnumerator FadeHealthbarDamageTaken()
    {
        yield return new WaitForSeconds(.75f);
        float fadeAmount = 5f;
        damagedColor.a -= fadeAmount * Time.deltaTime;
        HealthbarSliderFillDamagedFade.color = damagedColor;
    }

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
        // Initial check of stats are out of bounds
        CheckStatsCap();

        // Update healthbar and text
        healthText.text = ($"{monsterReference.health.ToString()}/{monster.maxHealth.ToString()}"); // \nSpeed: { monsterReference.speed.ToString()}

        // Only check monster is alive if damage was taken - Fixes dual death call bug
        if (damageTaken)
        {
            CheckHealth();
            UpdateHealthBar(damageTaken);
        }
        else
        {
            UpdateHealthBar(false);
        }

        // Update all stats
        monsterPhysicalAttack = monsterReference.physicalAttack;
        monsterMagicAttack = monsterReference.magicAttack;
        monsterPhysicalDefense = monsterReference.physicalDefense;
        monsterMagicDefense = monsterReference.magicDefense;

        monsterCritChance = monsterReference.critChance;
        monsterEvasion = monsterReference.evasion;
        monsterSpeed = (int)monsterReference.speed;

        // Check if stats are out of bounds
        CheckStatsCap();
    }

    // This function checks stat caps
    public void CheckStatsCap()
    {
        if (monsterReference.speed < 1)
        {
            monsterReference.speed = 1;
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
                if (attack.attackCurrentCooldown <= 0)
                {
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
                        ShowDamageOrStatusEffectPopup(poisonDamage);
                        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} is Poisoned and takes {poisonDamage} damage!", monsterReference.aiType);
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
                        ShowDamageOrStatusEffectPopup(burningDamage);
                        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} is Burning and takes {burningDamage} damage!", monsterReference.aiType);
                        UpdateStats(true);
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

            case (AttackEffect.StatEnumToChange.Health):
                monsterReference.health += (int)modifier.modifierAmount;
                break;

            default:
                Debug.Log("Missing stat to modify to modifier?", this);
                break;
        }

    }

    // This function modifies stats by modifier value from an adventure equipment
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
        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {statToModify} was increased by {modifier.modifierName}!", monsterReference.aiType);
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

            case (Modifier.StatusEffectType.Dazed):
                monsterIsDazed = true;
                statusEffectUISprite.sprite = monsterAttackManager.dazedUISprite;
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
        // Called after a delay
        if (monsterReference.health <= 0)
        {
            transform.DetachChildren();

            // remove statuses to prevent two status death call bug
            monsterIsPoisoned = false;
            monsterIsBurning = false;

            combatManagerScript.RemoveMonsterFromList(gameObject, monsterReference.aiType);
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} has been defeated!", monsterReference.aiType);
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

    // This function ends the buff animation
    public void UseBuffAnimationEnd()
    {
        monsterAnimator.SetBool("useBuffAnimationPlaying", false);
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
        // Confirm Target
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (monsterAttackManager.currentMonsterAttack != null && combatManagerScript.monsterTurn == CombatManagerScript.MonsterTurn.AllyTurn && combatManagerScript.targeting)
            {
                monsterAttackManager.UseMonsterAttack();
            }
        }

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
                $"\nCrit Chance: {monsterReference.critChance} (+{monsterReference.critChance - monsterReference.cachedCritChance})" +
                $"\nSpeed: {monsterReference.speed} ({ReturnSign(monsterReference.speed, monsterReference.cachedSpeed)}{monsterReference.speed - monsterReference.cachedSpeed})");
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

    // This function is called by monster attack manager to show damage popup
    public void ShowDamageOrStatusEffectPopup(float damage)
    {
        monsterStatusTextObject.SetActive(true);
        monsterStatusText.color = Color.red;
        monsterStatusText.text = ($"-{damage}");
    }

    // This function is called by monster attack manager to show condition popup
    public void ShowDamageOrStatusEffectPopup(string condition)
    {
        monsterStatusTextObject.SetActive(true);
        monsterStatusText.color = Color.white;
        monsterStatusText.text = ($"{condition}!");
    }

    // This function is called by monster attack manager to clear any current popups
    public void ShowDamageOrStatusEffectPopup()
    {
        monsterStatusTextObject.SetActive(false);
        monsterStatusText.text = ($"");
    }

    // This function is called by monster attack manager to create a buff/debuff popup
    public void CreateStatusEffectPopup(AttackEffect.StatEnumToChange stat, bool isBuff)
    {
        GameObject effectPopup = Instantiate(monsterStatusTextObjectCanvas, popupPosTransform);
        effectPopup.GetComponentInChildren<PopupScript>().instantiated = true;
        effectPopup.GetComponentInChildren<PopupScript>().parentObj = effectPopup;
        effectPopup.GetComponentInChildren<Animator>().speed = Random.Range(0.25f, 1.5f);

        if (!isBuff)
        {
            effectPopup.GetComponentInChildren<TextMeshProUGUI>().text = ($"{stat.ToString()} down!");
        }
        else
        {
            effectPopup.GetComponentInChildren<TextMeshProUGUI>().text = ($"{stat.ToString()} up!");
        }
    }

    // This function is called by monster attack manager to create a status effect popup
    public void CreateStatusEffectPopup(string condition)
    {
        GameObject effectPopup = Instantiate(monsterStatusTextObjectCanvas, popupPosTransform);
        effectPopup.GetComponentInChildren<PopupScript>().instantiated = true;
        effectPopup.GetComponentInChildren<PopupScript>().parentObj = effectPopup;
        effectPopup.GetComponentInChildren<Animator>().speed = Random.Range(0.25f, 1.5f);

        effectPopup.GetComponentInChildren<TextMeshProUGUI>().text = ($"{condition}!");

    }
}
