using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using System.Linq;
using System.Threading.Tasks;

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
    [SerializeField] private Image monsterRowFrontIcon;
    [SerializeField] private Image monsterRowBackIcon;
    public Vector3 cameraOffset;

    public Color32 healColor;
    public Color32 buffColor;
    public Color32 debuffColor;

    [Title("Status Effect Scrollbar")]
    [SerializeField] public GameObject statusEffectHolder;
    [SerializeField] public GameObject statusEffectIcon;
    [SerializeField] public GameObject InteractableToolTipWindow;
    [SerializeField] public Image statusEffectUISprite;

    [Title("MiniStatWindow")]
    [SerializeField] public GameObject MiniStatWindow;
    [SerializeField] public TextMeshProUGUI MiniStatWindowBasicStatText;
    [SerializeField] public TextMeshProUGUI MiniStatWindowAdvancedStatText;

    [SerializeField] public GameObject MonsterCombatPredictionWindow;
    [SerializeField] public TextMeshProUGUI MonsterCombatPredictionWindowText;

    [Title("Level Up Results Window")]
    public GameObject LevelUpResultsWindow;
    public ParticleSystem LevelUp_VFX;
    public TextMeshProUGUI LevelUpResultsWindowPrimaryStatsText;
    public TextMeshProUGUI LevelUpResultsWindowSecondaryStatsText;
    public GameObject LevelUpContinueButton;

    [Title("Popups")]
    [SerializeField] public Transform popupPosTransform;
    [SerializeField] private GameObject monsterStatusTextObjectCanvas;
    [SerializeField] public GameObject monsterTargeterUIGameObject;

    [Title("Healthbar")]
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
    public Vector3 startingPos;
    public enum CombatOrientation { Left, Right };
    public CombatOrientation combatOrientation;

    [Title("Combat Stats To Display")]
    [DisplayWithoutEdit] private int monsterLevel;
    [SerializeReference] public int monsterSpeed;

    [DisplayWithoutEdit] public float monsterPhysicalAttack;
    [DisplayWithoutEdit] public float monsterMagicAttack;
    [DisplayWithoutEdit] public float monsterPhysicalDefense;
    [DisplayWithoutEdit] public float monsterMagicDefense;

    [DisplayWithoutEdit] public float monsterCritChance;
    [DisplayWithoutEdit] public float monsterEvasion;

    [Title("Combat Functions & Status")]
    public enum MonsterRowPosition { CenterRow, BackRow, FrontRow };
    public MonsterRowPosition monsterRowPosition;

    [DisplayWithoutEdit] public float monsterDamageTakenThisRound;
    [DisplayWithoutEdit] public bool monsterActionAvailable = true;
    [DisplayWithoutEdit] public bool monsterRecievedStatBoostThisRound = false;
    [DisplayWithoutEdit] public bool monsterCriticallyStrikedThisRound = false;

    public bool monsterIsPoisoned = false;
    public bool monsterIsBurning = false;
    public bool monsterIsDazed = false;
    public bool monsterIsCrippled = false;
    public bool monsterIsWeakened = false;
    public bool monsterIsStunned = false;

    [Title("Monster Basic Immunities")]
    public bool monsterImmuneToDebuffs = false;
    //public bool monsterImmuneToBuffs = false;
    public bool monsterImmuneToDamage = false;

    // Basic Stat Immunities
    public List<AttackEffect.StatEnumToChange> listOfStatImmunities;

    // Basic Element Immunities
    public List<ElementClass> listOfElementImmunities;

    // Specific Status Immunities
    public List<Modifier.StatusEffectType> listOfStatusImmunities;

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
        InitiateCreateMonsterObjectStats();
        InitiateHealthBars();
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
        if (combatManagerScript.adventureMode || combatManagerScript.testAdventureMode)
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
            monster.cachedCritDamage = monsterReference.critDamage;

            monster.cachedBonusAccuracy = monsterReference.bonusAccuracy;
            monsterRowPosition = monster.cachedMonsterRowPosition;

            //monster.maxHealth = monster.health; not needed?
        }
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
        monsterSpeed = (int)monsterReference.speed;

        nameText.text = ($"{monster.name} Lvl: {monsterReference.level}");
        healthText.text = ($"{monsterReference.health.ToString()}/{monster.maxHealth.ToString()}");
        sr.sprite = monster.baseSprite;
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
        InteractableToolTipWindow = combatManagerScript.uiManager.InteractableToolTipWindow;

        // Create instances of the monster's attacks
        if (!combatManagerScript.adventureMode) // accuracy is alterable now
        {
            Monster tempMonster = Instantiate(monsterReference);
            monsterReference.ListOfMonsterAttacks.Clear();
            foreach (MonsterAttack attack in tempMonster.ListOfMonsterAttacks)
            {
                MonsterAttack attackInstance = Instantiate(attack);
                monsterReference.ListOfMonsterAttacks.Add(attackInstance);
            }
        }
        else
        {
            // Reset cooldowns at battle start in adventure mode
            foreach (MonsterAttack attack in monster.ListOfMonsterAttacks)
            {
                if (attack.attackHasCooldown)
                {
                    attack.attackOnCooldown = false;
                    attack.attackCurrentCooldown = attack.attackBaseCooldown;
                }
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

    // This function is called to update the monster's current row position (back row or front row) to adjust it stat bonuses
    public void UpdateMonsterRowPosition(MonsterRowPosition newRowPosition)
    {
        monsterRowPosition = newRowPosition;

        // Update visual position in battle
        SetPositionAndOrientation(startingPosition, combatOrientation, newRowPosition);
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
        monsterReference.monsterCurrentExp += Mathf.RoundToInt(expGained * combatManagerScript.adventureManager.bonusExp);

        // Show level up animation
        CreateStatusEffectPopup($"+{expGained} Exp");

        if (monsterReference.monsterCurrentExp >= monsterReference.monsterExpToNextLevel)
        {
            //LevelUp(true);
        }
    }

    // This function level ups the monster in combat
    public void LevelUp()
    {
        // Show level up animation
        LevelUp_VFX.Play();
        CreateStatusEffectPopup("Level Up!!!");
        monsterAttackManager.soundEffectManager.PlaySoundEffect(monsterAttackManager.LevelUpSound);

        List<float> ListOfMonstersLevelUpPrimaryStats = new List<float>();
        List<float> ListOfMonstersLevelUpSecondaryStats = new List<float>();

        // Level up, heal, and update states
        monsterReference.level += 1;
        monsterReference.monsterCurrentExp = Mathf.Abs(monsterReference.monsterExpToNextLevel - monsterReference.monsterCurrentExp);
        monsterReference.monsterExpToNextLevel = Mathf.RoundToInt(monsterReference.monsterExpToNextLevel * 1.15f);

        monster.previouslyCachedMaxHealth = monster.maxHealth;
        monsterReference.maxHealth = monsterReference.maxHealth + Random.Range(monsterReference.healthScaler, monsterReference.healthScaler * 2);
        monsterReference.health = monsterReference.maxHealth;

        // Basic int to add new stats to cached monster data
        int newStatToCache = 0;

        // Physical Attack
        monster.previouslyCachedPhysicalAttack = monster.cachedPhysicalAttack;
        newStatToCache = Mathf.RoundToInt((monsterReference.cachedPhysicalAttack) + Random.Range(1, monsterReference.physicalAttackScaler + 1));
        monsterReference.physicalAttack += newStatToCache - monsterReference.cachedPhysicalAttack;
        monster.cachedPhysicalAttack = newStatToCache;

        // Magic Attack
        monster.previouslyCachedMagicAttack = monster.cachedMagicAttack;
        newStatToCache = Mathf.RoundToInt((monsterReference.cachedMagicAttack) + Random.Range(1, monsterReference.magicAttackScaler + 1));
        monsterReference.magicAttack += newStatToCache - monsterReference.cachedMagicAttack;
        monster.cachedMagicAttack = newStatToCache;

        // Physical Defense
        monster.previouslyCachedPhysicalDefense = monster.cachedPhysicalDefense;
        newStatToCache = Mathf.RoundToInt((monsterReference.cachedPhysicalDefense) + Random.Range(1, monsterReference.physicalDefenseScaler + 1));
        monsterReference.physicalDefense += newStatToCache - monsterReference.cachedPhysicalDefense;
        monster.cachedPhysicalDefense = newStatToCache;

        // Magic Defense
        monster.previouslyCachedMagicDefense = monster.cachedMagicDefense;
        newStatToCache = Mathf.RoundToInt((monsterReference.cachedMagicDefense) + Random.Range(1, monsterReference.magicDefenseScaler + 1));
        monsterReference.magicDefense += newStatToCache - monsterReference.cachedMagicDefense;
        monster.cachedMagicDefense = newStatToCache;

        // Speed
        monster.previouslyCachedSpeed = monster.cachedSpeed;
        newStatToCache = Mathf.RoundToInt((monsterReference.cachedSpeed) + Random.Range(1, monsterReference.speedScaler + 1));
        monsterReference.speed += newStatToCache - monsterReference.cachedSpeed;
        monster.cachedSpeed = newStatToCache;

        // Finally, update in-combat stats
        UpdateStats(true, null, false);
        combatManagerScript.SortMonsterBattleSequence();
        nameText.text = monster.name + ($" Lvl: {monsterReference.level}");
        InitiateHealthBars();

        // Show Level Up Results Window
        //Invoke("ShowLevelUpResultsWindow", 0.5f);
        StartCoroutine("ShowLevelUpResultsWindow");
    }

    // This function shows the monster's level up results window
    public IEnumerator ShowLevelUpResultsWindow()
    {
        yield return new WaitForSeconds(0.5f);

        // Create temp lists of the monster's previously cached stats
        List<float> ListOfMonstersLevelUpPrimaryStats = new List<float>();
        List<float> ListOfMonstersLevelUpSecondaryStats = new List<float>();

        // Create temp lists of the monster's new stats
        List<float> ListOfMonstersLevelUpNewPrimaryStats = new List<float>();
        List<float> ListOfMonstersLevelUpNewSecondaryStats = new List<float>();

        // Add items to both lists
        ListOfMonstersLevelUpPrimaryStats.Add(monster.previouslyCachedMaxHealth);
        ListOfMonstersLevelUpPrimaryStats.Add(monster.previouslyCachedPhysicalAttack);
        ListOfMonstersLevelUpPrimaryStats.Add(monster.previouslyCachedMagicAttack);
        ListOfMonstersLevelUpPrimaryStats.Add(monster.previouslyCachedPhysicalDefense);
        ListOfMonstersLevelUpPrimaryStats.Add(monster.previouslyCachedMagicDefense);

        ListOfMonstersLevelUpNewPrimaryStats.Add(monster.maxHealth);
        ListOfMonstersLevelUpNewPrimaryStats.Add(monster.cachedPhysicalAttack);
        ListOfMonstersLevelUpNewPrimaryStats.Add(monster.cachedMagicAttack);
        ListOfMonstersLevelUpNewPrimaryStats.Add(monster.cachedPhysicalDefense);
        ListOfMonstersLevelUpNewPrimaryStats.Add(monster.cachedMagicDefense);

        ListOfMonstersLevelUpSecondaryStats.Add(monster.previouslyCachedSpeed);
        ListOfMonstersLevelUpSecondaryStats.Add(monster.previouslyCachedEvasion);
        ListOfMonstersLevelUpSecondaryStats.Add(monster.previouslyCachedCritChance);
        ListOfMonstersLevelUpSecondaryStats.Add(monster.previouslyCachedBonusAccuracy);

        ListOfMonstersLevelUpNewSecondaryStats.Add(monster.cachedSpeed);
        ListOfMonstersLevelUpNewSecondaryStats.Add(monster.cachedEvasion);
        ListOfMonstersLevelUpNewSecondaryStats.Add(monster.cachedCritChance);
        ListOfMonstersLevelUpNewSecondaryStats.Add(monster.cachedBonusAccuracy);

        // Reset text and show level up results window
        LevelUpResultsWindowPrimaryStatsText.text = ("");
        LevelUpResultsWindowSecondaryStatsText.text = ("");
        LevelUpResultsWindow.SetActive(true);

        // Iterate through each stat and show previous and new stats
        int i = 0;
        foreach (float stat in ListOfMonstersLevelUpPrimaryStats)
        {
            LevelUpResultsWindowPrimaryStatsText.text += ($"{ListOfMonstersLevelUpNewPrimaryStats[i]} (+{ListOfMonstersLevelUpNewPrimaryStats[i] - stat})\n");
            i++;
            monsterAttackManager.soundEffectManager.PlaySoundEffect(monsterAttackManager.soundEffectManager.UISelectSFX1);
            yield return new WaitForSeconds(0.2f);
        }

        i = 0;
        foreach (float stat in ListOfMonstersLevelUpSecondaryStats)
        {
            if (stat == 0)
            {
                LevelUpResultsWindowSecondaryStatsText.text += ($"{ListOfMonstersLevelUpNewSecondaryStats[i]} (+0)\n");
            }
            else
            {
                LevelUpResultsWindowSecondaryStatsText.text += ($"{ListOfMonstersLevelUpNewSecondaryStats[i]} (+{ListOfMonstersLevelUpNewSecondaryStats[i] - stat})\n");
            }
            i++;
            monsterAttackManager.soundEffectManager.PlaySoundEffect(monsterAttackManager.soundEffectManager.UISelectSFX1);
            yield return new WaitForSeconds(0.2f);
        }

        // Finally, show continue button once every stat has been displayed
        LevelUpContinueButton.SetActive(true);
    }

    // This function is called after the player accepts the LevelUpResultsWindow
    public async void HideLevelUpResultsWindow()
    {
        LevelUpResultsWindow.SetActive(false);

        // Check if exp is still overcapped
        if (monsterReference.monsterCurrentExp >= monsterReference.monsterExpToNextLevel)
        {
            LevelUp();
        }
        else
        {
            if (combatManagerScript.monsterAttackManager.currentMonsterAttack.monsterAttackTargetCount == MonsterAttack.MonsterAttackTargetCount.SingleTarget)
            {

                await combatManagerScript.monsterAttackManager.TriggerAbilityEffects(monster, AttackEffect.EffectTime.PostAttack);

                await combatManagerScript.monsterAttackManager.TriggerPostAttackEffects(monster, gameObject);

                combatManagerScript.Invoke("NextMonsterTurn", 0.25f);
            }
            else
            {
                combatManagerScript.Invoke("NextMonsterTurn", 0.25f);
            }
        }
    }

    // This function level ups the monster
    public void LevelUpOutsideOfCombat()
    {
        // Level up, heal, and update states
        monsterReference.level += 1;
        monsterReference.monsterCurrentExp = Mathf.Abs(monsterReference.monsterExpToNextLevel - monsterReference.monsterCurrentExp);

        monsterReference.monsterExpToNextLevel = Mathf.RoundToInt(monsterReference.monsterExpToNextLevel * 1.15f);

        monsterReference.maxHealth = monsterReference.maxHealth + Random.Range(monsterReference.healthScaler, monsterReference.healthScaler * 2);
        monsterReference.health = monsterReference.maxHealth;

        // Basic int to add new stats to cached monster data
        int newStatToCache = 0;

        // Physical Attack
        newStatToCache = Mathf.RoundToInt((monsterReference.cachedPhysicalAttack) + Random.Range(1, monsterReference.physicalAttackScaler + 1));
        monsterReference.physicalAttack += newStatToCache - monsterReference.cachedPhysicalAttack;
        monster.cachedPhysicalAttack = newStatToCache;

        // Magic Attack
        newStatToCache = Mathf.RoundToInt((monsterReference.cachedMagicAttack) + Random.Range(1, monsterReference.magicAttackScaler + 1));
        monsterReference.magicAttack += newStatToCache - monsterReference.cachedMagicAttack;
        monster.cachedMagicAttack = newStatToCache;

        // Physical Defense
        newStatToCache = Mathf.RoundToInt((monsterReference.cachedPhysicalDefense) + Random.Range(1, monsterReference.physicalDefenseScaler + 1));
        monsterReference.physicalDefense += newStatToCache - monsterReference.cachedPhysicalDefense;
        monster.cachedPhysicalDefense = newStatToCache;

        // Magic Defense
        newStatToCache = Mathf.RoundToInt((monsterReference.cachedMagicDefense) + Random.Range(1, monsterReference.magicDefenseScaler + 1));
        monsterReference.magicDefense += newStatToCache - monsterReference.cachedMagicDefense;
        monster.cachedMagicDefense = newStatToCache;

        // Speed
        newStatToCache = Mathf.RoundToInt((monsterReference.cachedSpeed) + Random.Range(1, monsterReference.speedScaler + 1));
        monsterReference.speed += newStatToCache - monsterReference.cachedSpeed;
        monster.cachedSpeed = newStatToCache;
    }

    // This function grants exp and if the monster levels up, it returns true and shows the level up results window
    public bool GrantExpAndCheckLevelup(int expGained)
    {
        // Grant exp
        if (expGained > 0)
        {
            monsterReference.monsterCurrentExp += Mathf.RoundToInt(expGained * combatManagerScript.adventureManager.bonusExp);
            CreateStatusEffectPopup($"+{expGained} Exp");
        }

        // If the exp was enough to level up, call LevelUp()
        if (monsterReference.monsterCurrentExp >= monsterReference.monsterExpToNextLevel)
        {
            Invoke("LevelUp", 1f);
            return true;
        }

        // Didn't level up, return false
        return false;
    }

    // This function checks stat caps
    public void CheckStatsCap()
    {
        if (monsterReference.health > monsterReference.maxHealth)
        {
            monsterReference.health = monsterReference.maxHealth;
        }

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
        // If the attack should be off cooldown, reset its CD
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

            // Reset any attack effects that only trigger once per use per round
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
        StartCoroutine(CheckModifiers());
    }

    // This function checks modifiers, permanent or tempoerary
    public IEnumerator CheckModifiers()
    {
        foreach (Modifier modifier in monsterReference.ListOfModifiers.ToArray())
        {
            // Check statuses
            if (modifier.statusEffect)
            {
                // First check if monster is immune to debuffs, if so, break out.
                if (monsterImmuneToDebuffs)
                {
                    // Send immune message to combat log
                    combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} is immune to status effects and debuffs!");
                    CreateStatusEffectPopup("Immune!");

                    // Reduce the duration of temporary modifiers
                    if (modifier.modifierDurationType == Modifier.ModifierDurationType.Temporary)
                    {
                        modifier.modifierCurrentDuration -= 1;
                        if (modifier.modifierCurrentDuration == 0)
                        {
                            modifier.ResetModifiedStat(monsterReference, gameObject);
                            UpdateStats(false, null, false);
                            monsterReference.ListOfModifiers.Remove(modifier);
                        }

                        if (modifier.statusEffectIconGameObject.TryGetComponent(out StatusEffectIcon statusEffectIcon) != false)
                        {
                            modifier.statusEffectIconGameObject.GetComponent<StatusEffectIcon>().modifierDurationText.text = ($"{modifier.modifierCurrentDuration}");
                        }
                        else
                        {
                            continue;
                        }

                        continue;
                    }
                }
                else
                {
                    switch (modifier.statusEffectType)
                    {
                        case (Modifier.StatusEffectType.Poisoned):
                            monsterIsPoisoned = true;
                            statusEffectUISprite.sprite = monsterAttackManager.poisonedUISprite;

                            // Check if immune to damage
                            if (monsterImmuneToDamage)
                            {
                                // Send immune message to combat log
                                combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} is immune to damage!");
                                CreateStatusEffectPopup("Immune!");
                                continue;
                            }

                            int poisonDamage = Mathf.RoundToInt(modifier.modifierAmount * monsterReference.maxHealth);

                            monsterAnimator.SetBool("hitAnimationPlaying", true);
                            monsterAttackManager.soundEffectManager.AddSoundEffectToQueue(monsterAttackManager.HitSound);
                            monsterAttackManager.soundEffectManager.BeginSoundEffectQueue();

                            monsterReference.health -= poisonDamage;
                            ShowDamageOrStatusEffectPopup(poisonDamage, "Damage");
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

                            // Check if immune to damage
                            if (monsterImmuneToDamage)
                            {
                                // Send immune message to combat log
                                combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} is immune to damage!");
                                CreateStatusEffectPopup("Immune!");
                                continue;
                            }

                            int burningDamage = Mathf.RoundToInt(modifier.modifierAmount * monsterReference.health);
                            if (burningDamage < 1)
                            {
                                burningDamage = 1; // Fix zero damage burn
                            }

                            monsterAnimator.SetBool("hitAnimationPlaying", true);
                            monsterAttackManager.soundEffectManager.AddSoundEffectToQueue(monsterAttackManager.HitSound);
                            monsterAttackManager.soundEffectManager.BeginSoundEffectQueue();

                            monsterReference.health -= burningDamage;
                            ShowDamageOrStatusEffectPopup(burningDamage, "Damage");
                            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} is Burning and takes {burningDamage} damage!", monsterReference.aiType);
                            UpdateStats(true, modifier.modifierOwnerGameObject, true);

                            if (modifier.modifierOwnerGameObject != null)
                            {
                                modifier.modifierOwner.cachedDamageDone += burningDamage;
                            }
                            break;
                    }
                }
            }

            // Reduce the duration of temporary modifiers
            if (modifier.modifierDurationType == Modifier.ModifierDurationType.Temporary)
            {
                modifier.modifierCurrentDuration -= 1;
                if (modifier.modifierCurrentDuration == 0)
                {
                    modifier.ResetModifiedStat(monsterReference, gameObject);
                    UpdateStats(false, null, false);
                    monsterReference.ListOfModifiers.Remove(modifier);
                }

                if (modifier.statusEffectIconGameObject.TryGetComponent(out StatusEffectIcon statusEffectIcon) != false)
                {
                    modifier.statusEffectIconGameObject.GetComponent<StatusEffectIcon>().modifierDurationText.text = ($"{modifier.modifierCurrentDuration}");
                }
                else
                {
                    continue;
                }
            }

            // adjust time
            float timeToDelay = 0.5f;

            if (!modifier.statusEffect)
                timeToDelay = 0.01f;

            yield return new WaitForSeconds(timeToDelay);
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
                // Create popup
                CreateStatusEffectPopup("Debuff and Status Immunity!");
                break;

            //case (AttackEffect.StatEnumToChange.Buffs):
            //    monsterImmuneToBuffs = true;
            //    // Create popup
            //    CreateStatusEffectPopup("Debuff and Status Immunity!");
            //    break;

            case (AttackEffect.StatEnumToChange.Damage):
                monsterImmuneToDamage = true;
                // Create popup
                CreateStatusEffectPopup("Damage Immunity!");
                break;

            case (AttackEffect.StatEnumToChange.BothOffensiveStats):
                monsterReference.physicalAttack += (int)modifier.modifierAmount;
                monsterReference.magicAttack += (int)modifier.modifierAmount;
                break;

            case (AttackEffect.StatEnumToChange.Health):
                monsterReference.health += (int)modifier.modifierAmount;
                break;

            case (AttackEffect.StatEnumToChange.Accuracy):
                monsterReference.bonusAccuracy += (int)modifier.modifierAmount;
                break;

            case (AttackEffect.StatEnumToChange.HighestAttackStat):
                if (MonsterAttackManager.ReturnMonsterHighestAttackStat(monsterReference) == MonsterAttack.MonsterAttackDamageType.Magical)
                {
                    modifier.statModified = AttackEffect.StatEnumToChange.MagicAttack;
                    monsterReference.magicAttack += (int)modifier.modifierAmount;
                }
                else if (MonsterAttackManager.ReturnMonsterHighestAttackStat(monsterReference) == MonsterAttack.MonsterAttackDamageType.Physical)
                {
                    modifier.statModified = AttackEffect.StatEnumToChange.PhysicalAttack;
                    monsterReference.physicalAttack += (int)modifier.modifierAmount;
                }
                break;

            default:
                Debug.Log("Missing stat to modify to modifier?", this);
                break;
        }

        AddStatusIcon(modifier, statToModify, modifier.modifierCurrentDuration);
    }

    // This function modifies stats by modifier value
    public void ModifyStats(AttackEffect.StatEnumToChange statToModify, Modifier modifier, bool specialModifier)
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
                // Create popup
                CreateStatusEffectPopup("Debuff and Status Immunity!");
                break;

            //case (AttackEffect.StatEnumToChange.Buffs):
            //    monsterImmuneToBuffs = true;
            //    break;

            case (AttackEffect.StatEnumToChange.Damage):
                monsterImmuneToDamage = true;
                // Create popup
                CreateStatusEffectPopup("Damage Immunity!");
                break;

            case (AttackEffect.StatEnumToChange.BothOffensiveStats):
                monsterReference.physicalAttack += (int)modifier.modifierAmount;
                monsterReference.magicAttack += (int)modifier.modifierAmount;
                break;

            case (AttackEffect.StatEnumToChange.Health):
                monsterReference.health += (int)modifier.modifierAmount;
                break;

            case (AttackEffect.StatEnumToChange.Accuracy):
                monsterReference.bonusAccuracy += (int)modifier.modifierAmount;
                break;

            default:
                Debug.Log("Missing stat to modify to modifier?", this);
                break;
        }

        AddSpecialStatusIcon(modifier);
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

            //case (AttackEffect.StatEnumToChange.Buffs):
            //    monsterImmuneToBuffs = true;
            //    break;

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
        combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name}'s {statToModify} was increased by {modifier.modifierName} (+{modifier.modifierAmount})!", monsterReference.aiType);
        AddStatusIcon(modifier, statToModify, modifier.modifierCurrentDuration);
    }

    // This function adds the modifer's icon to the monster's HUD
    public void AddStatusIcon(Modifier modifier, AttackEffect.StatEnumToChange statEnumToChange, int effectDuration)
    {
        //List<Modifier> modList = monsterReference.ListOfModifiers.Where(mod => mod.statModified == modifier.statModified && mod.modifierSource == modifier.modifierSource).ToList();

        //if (modList.Count == 1)
        //{
        if (!modifier.adventureEquipment && statEnumToChange != AttackEffect.StatEnumToChange.Health)
        {
            GameObject statusIcon = Instantiate(statusEffectIcon, statusEffectHolder.transform);
            modifier.statusEffectIconGameObject = statusIcon;
            StatusEffectIcon newStatusEffectIcon = statusIcon.AddComponent<StatusEffectIcon>();
            newStatusEffectIcon.modifier = modifier;

            // Initiate Interactable components
            newStatusEffectIcon.modifier.modifierName = modifier.modifierSource;
            newStatusEffectIcon.modifier.modifierDescription = ($"{ReturnSign(modifier.statChangeType)}{modifier.modifierAmount} {modifier.statModified.ToString()}");

            newStatusEffectIcon.InitiateStatusEffectIcon(this);
        }
        else if (modifier.statusEffect)
        {
            GameObject statusIcon = Instantiate(statusEffectIcon, statusEffectHolder.transform);
            modifier.statusEffectIconGameObject = statusIcon;
            StatusEffectIcon newStatusEffectIcon = statusIcon.AddComponent<StatusEffectIcon>();
            newStatusEffectIcon.modifier = modifier;

            // Initiate Interactable components
            newStatusEffectIcon.modifier.modifierName = modifier.modifierSource;
            newStatusEffectIcon.modifier.modifierDescription = ($"{modifier.statusEffectType.ToString()}, {modifier.modifierAmount}% {modifier.statModified.ToString()}");

            newStatusEffectIcon.InitiateStatusEffectIcon(this);
        }
        //}
        //else
        //{
        //    StatusEffectIcon statusEffectIcon = modList.First().statusEffectIconGameObject.GetComponent<StatusEffectIcon>();
        //    statusEffectIcon.currentModifierStack += 1;
        //    statusEffectIcon.modifierDurationText.text = ($"x{statusEffectIcon.currentModifierStack}");
        //}

        //modList.Clear();
    }

    // This function adds the modifer's icon to the monster's HUD
    public void AddSpecialStatusIcon(Modifier modifier)
    {
        List<Modifier> modList = monsterReference.ListOfModifiers.Where(mod => mod.modifierName == modifier.modifierName).ToList();

        //Debug.Log($"Modlist count: {modList.Count}");

        if (modList.Count <= 1)
        {
            GameObject statusIcon = Instantiate(statusEffectIcon, statusEffectHolder.transform);
            modifier.statusEffectIconGameObject = statusIcon;
            StatusEffectIcon newStatusEffectIcon = statusIcon.AddComponent<StatusEffectIcon>();
            newStatusEffectIcon.modifier = modifier;
            newStatusEffectIcon.InitiateSpecialEffectIcon(this);
        }
        else
        {
            if (modifier.statusEffectIconGameObject == null)
            {
                GameObject statusIcon = Instantiate(statusEffectIcon, statusEffectHolder.transform);
                modifier.statusEffectIconGameObject = statusIcon;
                StatusEffectIcon newStatusEffectIcon = statusIcon.AddComponent<StatusEffectIcon>();
                newStatusEffectIcon.modifier = modifier;
                newStatusEffectIcon.InitiateSpecialEffectIcon(this);
            }
            //StatusEffectIcon statusEffectIconScript = modList.First().statusEffectIconGameObject.GetComponent<StatusEffectIcon>();
            //statusEffectIconScript.currentModifierStack += 1;
            //statusEffectIconScript.modifierDurationText.text = ($"x{statusEffectIconScript.currentModifierStack}");
        }

        modList.Clear();
    }

    // This function returns a sprite icon based on what stat is changed
    public Sprite ReturnStatusEffectSprite(Modifier modifier)
    {
        if (modifier.statusEffect)
        {
            switch (modifier.statusEffectType)
            {
                case (Modifier.StatusEffectType.Burning):
                    return combatManagerScript.uiManager.burningUISprite;

                case (Modifier.StatusEffectType.Poisoned):
                    return combatManagerScript.uiManager.poisonedUISprite;

                case (Modifier.StatusEffectType.Dazed):
                    return combatManagerScript.uiManager.dazedUISprite;

                case (Modifier.StatusEffectType.Stunned):
                    return combatManagerScript.uiManager.stunnedUISprite;

                case (Modifier.StatusEffectType.Crippled):
                    return combatManagerScript.uiManager.crippledUISprite;

                case (Modifier.StatusEffectType.Weakened):
                    return combatManagerScript.uiManager.weakenedUISprite;

                default:
                    return combatManagerScript.monsterAttackManager.poisonedUISprite;
            }
        }

        switch (modifier.statModified)
        {
            case (AttackEffect.StatEnumToChange.Speed):
                return combatManagerScript.uiManager.speedSprite;

            case (AttackEffect.StatEnumToChange.PhysicalAttack):
                return combatManagerScript.uiManager.physicalAttackSprite;

            case (AttackEffect.StatEnumToChange.PhysicalDefense):
                return combatManagerScript.uiManager.physicalDefenseSprite;

            case (AttackEffect.StatEnumToChange.MagicAttack):
                return combatManagerScript.uiManager.magicAttackSprite;

            case (AttackEffect.StatEnumToChange.MagicDefense):
                return combatManagerScript.uiManager.magicDefenseSprite;

            case (AttackEffect.StatEnumToChange.Evasion):
                return combatManagerScript.uiManager.evasionSprite;

            case (AttackEffect.StatEnumToChange.CritChance):
                return combatManagerScript.uiManager.critChanceSprite;

            case (AttackEffect.StatEnumToChange.CritDamage):
                return combatManagerScript.uiManager.critDamageSprite;

            case (AttackEffect.StatEnumToChange.Damage):
                return combatManagerScript.uiManager.damageImmuneUISprite;

            case (AttackEffect.StatEnumToChange.Debuffs):
                return combatManagerScript.uiManager.debuffsImmuneUISprite;

            case (AttackEffect.StatEnumToChange.Accuracy):
                return combatManagerScript.uiManager.accuracySprite;

            default:
                return combatManagerScript.monsterAttackManager.poisonedUISprite;
        }
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

            case (Modifier.StatusEffectType.Stunned):
                monsterIsStunned = true;
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
            // Remove outside components
            Destroy(monsterTargeterUIGameObject);
            transform.DetachChildren();

            // remove statuses to prevent two status death call bug
            monsterIsPoisoned = false;
            monsterIsBurning = false;

            //// Check for status or ability kills
            //if (killedExternally && combatManagerScript.adventureMode && aiType != Monster.AIType.Ally)
            //{
            //    if (externalKillerGameObject == null)
            //    {
            //        externalKillerGameObject = combatManagerScript.GetRandomTarget(combatManagerScript.ListOfAllys); // random ally
            //    }
            //    externalKillerGameObject.GetComponent<CreateMonster>().monsterReference.monsterKills += 1;
            //    externalKillerGameObject.GetComponent<CreateMonster>().GrantExp(11 * monsterReference.level);
            //}

            combatManagerScript.RemoveMonsterFromList(gameObject, monsterReference.aiType);
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} has been defeated!", monsterReference.aiType);
        }
    }

    // This function sets monster sprite orientation at battle start
    public void SetPositionAndOrientation(Transform _startPos, CombatOrientation _combatOrientation, MonsterRowPosition _monsterRowPosition)
    {
        monsterRowPosition = _monsterRowPosition;

        switch (_monsterRowPosition)
        {
            case (MonsterRowPosition.BackRow):
                if (aiType == Monster.AIType.Ally)
                {
                    transform.position = new Vector3(startingPosition.transform.position.x - 1.75f, startingPosition.transform.position.y, startingPosition.transform.position.z);
                    monsterRowFrontIcon.enabled = false;
                    monsterRowBackIcon.enabled = true;
                }
                else
                {
                    transform.position = new Vector3(startingPosition.transform.position.x + 1.75f, startingPosition.transform.position.y, startingPosition.transform.position.z);
                    monsterRowFrontIcon.enabled = true;
                    monsterRowBackIcon.enabled = false;
                }
                break;

            case (MonsterRowPosition.FrontRow):
                if (aiType == Monster.AIType.Ally)
                {
                    transform.position = new Vector3(startingPosition.transform.position.x + 1.75f, startingPosition.transform.position.y, startingPosition.transform.position.z);
                    monsterRowFrontIcon.enabled = true;
                    monsterRowBackIcon.enabled = false;
                }
                else
                {
                    transform.position = new Vector3(startingPosition.transform.position.x - 1.75f, startingPosition.transform.position.y, startingPosition.transform.position.z);
                    monsterRowFrontIcon.enabled = false;
                    monsterRowBackIcon.enabled = true;
                }
                break;

            case (MonsterRowPosition.CenterRow):

                monsterRowFrontIcon.enabled = false;
                monsterRowBackIcon.enabled = false;
                break;
        }

        if (monsterReference.aiType == Monster.AIType.Ally)
        {
            sr.flipX = false;
        }
        else if (monsterReference.aiType == Monster.AIType.Enemy)
        {
            sr.flipX = true;
        }

    }

    // This function sets monster sprite orientation
    public void SetPositionAndOrientation(Transform _startPos, CombatOrientation _combatOrientation, MonsterRowPosition _monsterRowPosition, MonsterRowPosition previousRowPosition)
    {
        monsterRowPosition = _monsterRowPosition;

        switch (_monsterRowPosition)
        {
            case (MonsterRowPosition.BackRow):
                monsterRowFrontIcon.enabled = false;
                monsterRowBackIcon.enabled = true;
                if (previousRowPosition == MonsterRowPosition.CenterRow)
                {
                    StartCoroutine(MoveTowardPoint(-1.75f, 1.75f));
                }
                else
                if (previousRowPosition == MonsterRowPosition.FrontRow)
                {
                    StartCoroutine(MoveTowardPoint(-3.5f, 3f));
                }
                break;

            case (MonsterRowPosition.FrontRow):
                monsterRowFrontIcon.enabled = true;
                monsterRowBackIcon.enabled = false;
                if (previousRowPosition == MonsterRowPosition.BackRow)
                {
                    StartCoroutine(MoveTowardPoint(3.5f, 3f));
                }
                else
                if (previousRowPosition == MonsterRowPosition.CenterRow)
                {
                    StartCoroutine(MoveTowardPoint(1.75f, 1.75f));
                }
                break;

            case (MonsterRowPosition.CenterRow):
                monsterRowFrontIcon.enabled = false;
                monsterRowBackIcon.enabled = false;
                if (previousRowPosition == MonsterRowPosition.BackRow)
                {
                    StartCoroutine(MoveTowardPoint(1.75f, 1.75f));
                }
                else
                if (previousRowPosition == MonsterRowPosition.FrontRow)
                {
                    StartCoroutine(MoveTowardPoint(-1.75f, 1.75f));
                }
                break;
        }

        if (monsterReference.aiType == Monster.AIType.Ally)
        {
            sr.flipX = false;
        }
        else if (monsterReference.aiType == Monster.AIType.Enemy)
        {
            sr.flipX = true;
        }
    }

    // This function is called when the monster changes rows
    IEnumerator MoveTowardPoint(float distance, float speed)
    {
        // Fix enemy orientation
        if (monsterReference.aiType == Monster.AIType.Enemy)
        {
            distance *= -1;

            if (monsterRowPosition == MonsterRowPosition.BackRow)
            {
                monsterRowFrontIcon.enabled = true;
                monsterRowBackIcon.enabled = false;
            }
            else if (monsterRowPosition == MonsterRowPosition.FrontRow)
            {
                monsterRowFrontIcon.enabled = false;
                monsterRowBackIcon.enabled = true;
            }

        }

        Vector3 newPosition;
        newPosition = new Vector3(startingPosition.transform.position.x + distance, startingPosition.transform.position.y, startingPosition.transform.position.z);

        while (transform.position != newPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, newPosition, speed * Time.deltaTime);
            yield return null;
        }

        combatManagerScript.uiManager.InitiateMonsterTurnIndicator(combatManagerScript.CurrentMonsterTurn);

        // Reset HUD after
        if (combatManagerScript.monsterTurn == CombatManagerScript.MonsterTurn.AllyTurn)
        {
            combatManagerScript.buttonManagerScript.ResetHUD();
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
            SetPositionAndOrientation(startingPosition, combatOrientation, monsterRowPosition);
            monsterReference.aiLevel = aiLevel;
        }
        else if (monsterReference.aiType == Monster.AIType.Ally)
        {
            nameText.color = Color.white;
            combatOrientation = CombatOrientation.Left;
            SetPositionAndOrientation(startingPosition, combatOrientation, monsterRowPosition);
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
    public void DebuffAnimationEnd()
    {
        monsterAnimator.SetBool("debuffAnimationPlaying", false);
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
            combatManagerScript.uiManager.DetailedMonsterStatsWindow.GetComponent<InventoryManager>().currentMonsterEquipment = monster;
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
            MiniStatWindow.SetActive(true);
            MiniStatWindowBasicStatText.text =
                ($"{monsterReference.health}/{monsterReference.maxHealth}" +
                $"\n{monsterReference.physicalAttack} ({ReturnSign(monsterReference.physicalAttack, monsterReference.cachedPhysicalAttack)}{monsterReference.physicalAttack - monsterReference.cachedPhysicalAttack})" +
                $"\n{monsterReference.magicAttack} ({ReturnSign(monsterReference.magicAttack, monsterReference.cachedMagicAttack)}{monsterReference.magicAttack - monsterReference.cachedMagicAttack})" +
                $"\n{monsterReference.physicalDefense} ({ReturnSign(monsterReference.physicalDefense, monsterReference.cachedPhysicalDefense)}{monsterReference.physicalDefense - monsterReference.cachedPhysicalDefense})" +
                $"\n{monsterReference.magicDefense} ({ReturnSign(monsterReference.magicDefense, monsterReference.cachedMagicDefense)}{monsterReference.magicDefense - monsterReference.cachedMagicDefense})");
            MiniStatWindowAdvancedStatText.text =
                ($"{monsterReference.speed} ({ReturnSign(monsterReference.speed, monsterReference.cachedSpeed)}{monsterReference.speed - monsterReference.cachedSpeed})" +
                $"\n{monsterReference.evasion} ({ReturnSign(monsterReference.evasion, monsterReference.cachedEvasion)}{monsterReference.evasion - monsterReference.cachedEvasion})" +
                $"\n{monsterReference.critChance} (+{monsterReference.critChance - monsterReference.cachedCritChance})" +
                $"\n{monsterReference.bonusAccuracy} ({ReturnSign(monsterReference.bonusAccuracy, monsterReference.cachedBonusAccuracy)}{monsterReference.bonusAccuracy - monsterReference.cachedBonusAccuracy})");
        }
        else
        if (!showWindow)
        {
            MiniStatWindow.SetActive(false);
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

    // Override function that returns a sign based on a modifier's stat change type
    public string ReturnSign(AttackEffect.StatChangeType statChangeType)
    {
        if (statChangeType == AttackEffect.StatChangeType.Buff)
        {
            return "+";
        }
        else
        {
            return "";
        }
    }

    // This function returns a negative or positive sign for text applications
    public string ReturnStatusEffectDescription(Modifier.StatusEffectType statusEffectType)
    {
        switch (statusEffectType)
        {
            case (Modifier.StatusEffectType.Burning):
                return ($"current health damage per round.");

            case (Modifier.StatusEffectType.Crippled):
                return ($"50% reduced Speed and -10 Accuracy.");

            case (Modifier.StatusEffectType.Dazed):
                return ($"50% chance to select a different move and target.");

            case (Modifier.StatusEffectType.Poisoned):
                return ($"max health damage per round.");

            case (Modifier.StatusEffectType.Stunned):
                return ($"Cannot attack or act.");

            case (Modifier.StatusEffectType.Weakened):
                return ($"50% reduced Physical and Magic Attack.");

            default:
                return "";
        }
        
    }

    // This function returns a bonus damage source based on the enum StatEnumToChange
    public float GetStatToChange(AttackEffect.StatEnumToChange statEnumToChange, Monster monsterRef)
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
    public void ShowDamageOrStatusEffectPopup(float damage, string damageOrHeal)
    {   
        // if damage is greater than 0, that means it was a heal, show healing color
        if (damageOrHeal == "Heal")
        {
            monsterStatusTextObject.SetActive(true);
            monsterStatusText.color = Color.green;
            monsterStatusText.text = ($"+{damage}");
        }
        else
        {
            monsterStatusTextObject.SetActive(true);
            monsterStatusText.color = Color.red;
            monsterStatusText.text = ($"-{damage}");
        }
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
        effectPopup.GetComponentInChildren<Animator>().speed = 1.25f;

        if (!isBuff)
        {
            effectPopup.GetComponentInChildren<TextMeshProUGUI>().color = debuffColor;
            effectPopup.GetComponentInChildren<TextMeshProUGUI>().text = ($"{stat.ToString()} down!");
        }
        else
        {
            effectPopup.GetComponentInChildren<TextMeshProUGUI>().color = buffColor;
            effectPopup.GetComponentInChildren<TextMeshProUGUI>().text = ($"{stat.ToString()} up!");
        }
    }

    // This function is called by monster attack manager to create a buff/debuff popup
    public void CreateStatusEffectPopup(AttackEffect.StatEnumToChange stat, bool isBuff, float amountChanged)
    {
        GameObject effectPopup = Instantiate(monsterStatusTextObjectCanvas, popupPosTransform);
        effectPopup.GetComponentInChildren<PopupScript>().instantiated = true;
        effectPopup.GetComponentInChildren<PopupScript>().parentObj = effectPopup;
        effectPopup.GetComponentInChildren<Animator>().speed = 1.25f;

        if (!isBuff)
        {
            effectPopup.GetComponentInChildren<TextMeshProUGUI>().color = debuffColor;
            effectPopup.GetComponentInChildren<TextMeshProUGUI>().text = ($"{stat.ToString()} down!");
        }
        else
        {
            effectPopup.GetComponentInChildren<TextMeshProUGUI>().color = buffColor;
            effectPopup.GetComponentInChildren<TextMeshProUGUI>().text = ($"{stat.ToString()} up!");
        }
    }

    // This function is called by monster attack manager to create a status effect popup
    public void CreateStatusEffectPopup(string condition)
    {
        GameObject effectPopup = Instantiate(monsterStatusTextObjectCanvas, popupPosTransform);
        monsterAttackManager.soundEffectManager.PlaySoundEffect(monsterAttackManager.buffSound);
        effectPopup.GetComponentInChildren<PopupScript>().instantiated = true;
        effectPopup.GetComponentInChildren<PopupScript>().parentObj = effectPopup;
        effectPopup.GetComponentInChildren<TextMeshProUGUI>().text = ($"{condition}!");
    }
}
