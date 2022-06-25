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

    // Start
    private void Start()
    {
        InitiateStats();
        InitializeComponents();
        SetAIType();
    }

    // This function sets monster stats on HUD at battle start
    private void InitiateStats()
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

        // if adventure mode, cache stats only for Player
        if (combatManagerScript.adventureMode)
        {
            monsterReference = monster;
            monster.cachedLevel = monsterReference.level;
            monster.cachedPhysicalAttack = monsterReference.physicalAttack;
            monster.cachedMagicAttack = monsterReference.magicAttack;

            monster.cachedPhysicalDefense = monsterReference.physicalDefense;
            monster.cachedMagicDefense = monsterReference.magicDefense;

            monster.cachedSpeed = monsterReference.speed;
            monster.cachedEvasion = monsterReference.evasion;
            monster.cachedCritChance = monsterReference.critChance;

            monster.cachedBonusAccuracy = monsterReference.bonusAccuracy;
            //monster.maxHealth = monster.health; not needed?
        }

        InitiateCreateMonsterObjectStats();

        InitiateHealthBars();
    }

    // This function initiates the monster Object's CreateMonster() stats
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

    // This function initiates the monster's healthbars
    public void InitiateHealthBars()
    {
        // Initiate healthbars
        HealthbarSlider.maxValue = monsterReference.maxHealth;
        HealthbarSlider.value = monsterReference.health;

        HealthbarSliderDamaged.value = HealthbarSlider.value;
        HealthbarSliderDamaged.maxValue = HealthbarSlider.maxValue;

        // Set damaged health bar alpha to - and color to white
        damagedColor.a = 1f;
        damagedColor = Color.white;
        HealthbarSliderFillDamagedFade.color = damagedColor;
    }

    // This function initializes a gameObjects components
    public void InitializeComponents()
    {
        monsterAnimator = GetComponent<Animator>();
        monsterAttackManager = combatManagerObject.GetComponent<MonsterAttackManager>();

        // Create instances of the monster's attacks
        if (!combatManagerScript.adventureMode) // accuracy is alterable now
        {
            monsterReference.ListOfMonsterAttacks.Clear();
            foreach (MonsterAttack attack in monster.ListOfMonsterAttacks)
            {
                MonsterAttack attackInstance = Instantiate(attack);
                monsterReference.ListOfMonsterAttacks.Add(attackInstance);
            }
        }
    }

    /*
    // This function deep copies the list of a monster's attacks
    public void MonsterAttackListDeepCopy()
    {
        // Create blank temp monster
        copiedMonsterAttackList = Instantiate(monster);
        copiedMonsterAttackList.ListOfMonsterAttacks.Clear();

        // clone attack list
        foreach (MonsterAttack attack in monster.ListOfMonsterAttacks)
        {
            MonsterAttack attackInstance = Instantiate(attack);
            copiedMonsterAttackList.ListOfMonsterAttacks.Add(attackInstance);
        }

        
        // Create a copy list
        CopyList = new List<MonsterAttack>();
        foreach (var item in copiedMonsterAttackList.ListOfMonsterAttacks)
        {
            CopyList.Add(new MonsterAttack
                {
                   monsterAttackName = item.monsterAttackName,
                   monsterAttackDescription = item.monsterAttackDescription,
                   monsterAttackElement = item.monsterAttackElement,
                   monsterAttackType = item.monsterAttackType,
                   monsterAttackDamageType = item.monsterAttackDamageType,
                   monsterAttackTargetCount = item.monsterAttackTargetCount,
                   monsterAttackTargetType = item.monsterAttackTargetType,
                   monsterAttackSoundEffect = item.monsterAttackSoundEffect,
                   monsterAttackDamage = item.monsterAttackDamage,
                   monsterAttackFlatDamageBonus = item.monsterAttackFlatDamageBonus,
                   monsterAttackAccuracy = item.monsterAttackAccuracy,
                   monsterAttackCritChance = item.monsterAttackCritChance,
                   monsterAttackNeverMiss = item.monsterAttackNeverMiss,
                   attackHasCooldown = item.attackHasCooldown,
                   attackOnCooldown = item.attackOnCooldown,
                   attackBaseCooldown = item.attackBaseCooldown,
                   attackCurrentCooldown = item.attackCurrentCooldown,
                   ListOfAttackEffects = item.ListOfAttackEffects
                });
        }

        // Reference copy list
        //combatManagerScript.CombatLog.SendMessageToCombatLog("Copied list!");
    }
    */

    // This function checks for adventure modifiers and applies them either per round or at game start
    /*
    public void CheckAdventureModifiers()
    {
        // if adventure mode, check adventure modifiers
        if (combatManagerScript.adventureMode && monster.aiType == Monster.AIType.Ally)
        {
            combatManagerScript.adventureManager.ApplyAdventureModifiers(monster);
            CheckAdventureEquipment();
            UpdateStats(false, null, false);
        }
    }
    */

    // Check any adventure equipment modifiers at game start
    public void CheckAdventureEquipment()
    {
        foreach (Modifier equipment in monsterReference.ListOfModifiers)
        {
            if (equipment.adventureEquipment)
            {
                ModifyStats(equipment.statModified, equipment, "adventure");
                UpdateStats(false, null, false);
            }
        }
    }

    // Update current health bar and call damaged healthbar fade
    public void UpdateHealthBar(bool damageTaken)
    {
        if (damageTaken)
        {
            // Should also be called when healed
            HealthbarSlider.value = monsterReference.health;
            HealthbarSliderFill.color = new Color(Mathf.Clamp((1 - monsterReference.health / monsterReference.maxHealth), 0, .75f), Mathf.Clamp((monsterReference.health / monsterReference.maxHealth), 0, .75f), 0, 1f);

            // Fade out damaged healthbar
            Invoke("FadeHealthBarDamageTaken", .5f);
        }
    }

    // Fades the damaged health bar
    public void FadeHealthBarDamageTaken()
    {
        HealthbarSliderFillDamagedFade.CrossFadeAlpha(0, .75f, false);
        Invoke("AdjustHealthBarDamageTaken", .75f);
    }

    // Adjusts the damaged health bar to match the current health bar after its faded
    public void AdjustHealthBarDamageTaken()
    {
        HealthbarSliderDamaged.value = monsterReference.health;
        HealthbarSliderFillDamagedFade.CrossFadeAlpha(1, .1f, false);
    }

    // This function should be called when stats get updated
    public void UpdateStats(bool damageTaken, GameObject damageSourceGameObject, bool externalDamageTaken)
    {
        // Initial check of stats are out of bounds
        CheckStatsCap();

        // Update healthbar and text
        healthText.text = ($"{monsterReference.health.ToString()}/{monster.maxHealth.ToString()}"); // \nSpeed: { monsterReference.speed.ToString()}

        // Only check monster is alive if damage was taken - Fixes dual death call bug
        if (damageTaken)
        {
            CheckHealth(externalDamageTaken, damageSourceGameObject);
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

    // This function is called to grant a monster exp upon kill or combat win in adventure mode
    public void GrantExp(int expGained)
    {
        // Gain exp
        monsterReference.monsterCurrentExp += expGained;
        // Show level up animation
        CreateStatusEffectPopup($"+{expGained} Exp");

        if (monsterReference.monsterCurrentExp >= monsterReference.monsterExpToNextLevel)
        {
            LevelUp(true);
        }
    }

    // This function level ups the monster
    public void LevelUp(bool levelUpOnce)
    {
        // Show level up animation
        if (levelUpOnce)
        {
            CreateStatusEffectPopup("Level Up!!!");
            monsterAttackManager.soundEffectManager.PlaySoundEffect(monsterAttackManager.LevelUpSound);
        }

        // Level up, heal, and update states
        monsterReference.level += 1;
        monsterReference.monsterCurrentExp = Mathf.Abs(monsterReference.monsterExpToNextLevel - monsterReference.monsterCurrentExp);

        monsterReference.monsterExpToNextLevel = Mathf.RoundToInt(monsterReference.monsterExpToNextLevel * 1.25f);

        monsterReference.maxHealth = Mathf.RoundToInt((monsterReference.maxHealth + monsterReference.level) * monsterReference.healthScaler);
        monsterReference.health = monsterReference.maxHealth;

        // Basic int to add new stats to cached monster data
        int newStatToCache = 0;

        // Physical Attack
        newStatToCache = Mathf.RoundToInt((monsterReference.cachedPhysicalAttack + 1) * monsterReference.physicalAttackScaler);
        monsterReference.physicalAttack += newStatToCache - monsterReference.cachedPhysicalAttack;
        monster.cachedPhysicalAttack = newStatToCache;

        // Magic Attack
        newStatToCache = Mathf.RoundToInt((monsterReference.cachedMagicAttack + 1) * monsterReference.magicAttackScaler);
        monsterReference.magicAttack += newStatToCache - monsterReference.cachedMagicAttack;
        monster.cachedMagicAttack = newStatToCache;

        // Physical Defense
        newStatToCache = Mathf.RoundToInt((monsterReference.cachedPhysicalDefense + 1) * monsterReference.physicalDefenseScaler);
        monsterReference.physicalDefense += newStatToCache - monsterReference.cachedPhysicalDefense;
        monster.cachedPhysicalDefense = newStatToCache;

        // Magic Defense
        newStatToCache = Mathf.RoundToInt((monsterReference.cachedMagicDefense + 1) * monsterReference.magicDefenseScaler);
        monsterReference.magicDefense += newStatToCache - monsterReference.cachedMagicDefense;
        monster.cachedMagicDefense = newStatToCache;

        // Speed
        newStatToCache = Mathf.RoundToInt((monsterReference.cachedSpeed + 1) * monsterReference.speedScaler);
        monsterReference.speed += newStatToCache - monsterReference.cachedSpeed;
        monster.cachedSpeed = newStatToCache;

        // Finally, update in-combat stats
        UpdateStats(true, null, false);
        combatManagerScript.SortMonsterBattleSequence();
        nameText.text = monster.name + ($" Lvl: {monsterReference.level}");
        InitiateHealthBars();

        // Check if exp is still overcapped
        while (monsterReference.monsterCurrentExp >= monsterReference.monsterExpToNextLevel)
        {
            LevelUp(false);
        }
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
                        UpdateStats(true, modifier.modifierOwnerGameObject, true);

                        if (modifier.modifierOwnerGameObject != null)
                        {
                            modifier.modifierOwner.cachedDamageDone += poisonDamage;
                        }
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
                        UpdateStats(true, modifier.modifierOwnerGameObject, true);

                        if (modifier.modifierOwnerGameObject != null)
                        {
                            modifier.modifierOwner.cachedDamageDone += burningDamage;
                        }
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
                    UpdateStats(false, null, false);
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
                UpdateStats(false, null, false);
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
                UpdateStats(false, null, false);
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

            case (AttackEffect.StatEnumToChange.Accuracy):
                monsterReference.bonusAccuracy += (int)modifier.modifierAmount;
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

        // Create popup
        CreateStatusEffectPopup($"{statusEffect.ToString()}!");
    }

    // This function refreshs per-round combat variables
    public void ResetRoundCombatVariables()
    {
        monsterDamageTakenThisRound = 0;
        monsterActionAvailable = true;
        monsterRecievedStatBoostThisRound = false;
        monsterCriticallyStrikedThisRound = false;
    }

    // This function checks the monster's health
    public void CheckHealth(bool killedExternally, GameObject externalKillerGameObject)
    {
        // Called after a delay
        if (monsterReference.health <= 0)
        {
            transform.DetachChildren();

            // remove statuses to prevent two status death call bug
            monsterIsPoisoned = false;
            monsterIsBurning = false;

            // Check for status or ability kills
            if (killedExternally && combatManagerScript.adventureMode)
            {
                if (externalKillerGameObject == null)
                {
                    externalKillerGameObject = combatManagerScript.GetRandomTarget(combatManagerScript.ListOfAllys);
                }
                externalKillerGameObject.GetComponent<CreateMonster>().monsterReference.monsterKills += 1;
                externalKillerGameObject.GetComponent<CreateMonster>().GrantExp(15 * monsterReference.level);
            }

            combatManagerScript.RemoveMonsterFromList(gameObject, monsterReference.aiType);
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} has been defeated!", monsterReference.aiType);
        }
    }

    // This function sets monster sprite orientation at battle start
    private void SetPositionAndOrientation(Transform _startPos, CombatOrientation _combatOrientation)
    {
        transform.position = startingPosition.transform.position;

        if (monsterReference.aiType == Monster.AIType.Ally)
        {
            sr.flipX = false;
        }
        else if (monsterReference.aiType == Monster.AIType.Enemy)
        {
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

    // Variables for mouse events
    public float delayTime = 0.01f;
    public float currentTime = 0.0f;
    public bool windowShowing = false;

    // This function passes in the new target to the combatManager and displays the stat screen window of the currently hovered monster
    private void OnMouseEnter()
    {
        combatManagerScript.CycleTargets(gameObject);
        if (currentTime < delayTime)
        {
            return;
        }
        DisplayStatScreenWindow(true);
    }

    // This function selects the hovered monster for combat
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

        // Show detailed monster info window
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            combatManagerScript.uiManager.DetailedMonsterStatsWindow.SetActive(true);
            combatManagerScript.uiManager.DetailedMonsterStatsWindow.GetComponent<MonsterStatScreenScript>().DisplayMonsterStatScreenStats(monsterReference);
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

    // This function hides the stat screen window of the last hovered monster
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
                ($"Elements: {monster.monsterElement.element}/{monster.monsterSubElement.element}" +
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

    // This function returns a negative or positive sign for text applications
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

    // This function returns a bonus damage source based on the enum StatEnumToChange
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
