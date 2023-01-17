using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using System.Linq;
using System.Threading.Tasks;
using System;
using Random = UnityEngine.Random;

public class CreateMonster : MonoBehaviour
{
    [Title("Monster ScriptableObject and Reference Clone")]
    [Required] public Monster monster;
    public Monster monsterReference;

    [Title("UI Elements")]
    [SerializeField] private GameObject monsterCanvas;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Image monsterOffensiveStanceIcon;
    [SerializeField] private Image monsterDefensiveStanceIcon;
    public Vector3 cameraOffset;

    public Color32 healColor;
    public Color32 buffColor;
    public Color32 debuffColor;

    public GameObject monsterSPIcon;
    public RectTransform startingMonsterSPIconTransform;
    public List<GameObject> ListOfSPIcons;

    [Title("Status Effect Scrollbar")]
    [SerializeField] public GameObject statusEffectHolder;
    [SerializeField] public GameObject statusEffectIcon;
    [SerializeField] public GameObject InteractableToolTipWindow;
    [SerializeField] public Image statusEffectUISprite;

    [Title("MiniStatWindow")]
    [SerializeField] public GameObject MiniStatWindow;
    [SerializeField] public TextMeshProUGUI MiniStatWindowBasicStatText;
    [SerializeField] public TextMeshProUGUI MiniStatWindowAdvancedStatText;

    //[SerializeField] public GameObject MonsterCombatPredictionWindow;
    //[SerializeField] public TextMeshProUGUI MonsterCombatPredictionWindowText;

    [Title("Level Up Results Window")]
    public GameObject LevelUpResultsWindow;
    public ParticleSystem LevelUp_VFX;
    public TextMeshProUGUI LevelUpResultsWindowPrimaryStatsText;
    public TextMeshProUGUI LevelUpResultsWindowSecondaryStatsText;
    public GameObject LevelUpContinueButton;

    [Title("Popups")]
    [SerializeField] public Transform popupPosTransform;
    [SerializeField] public Transform popupPosTransformHigher;
    [SerializeField] private GameObject monsterStatusTextObjectCanvas;

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
    public enum MonsterStance { Neutral, Defensive, Aggressive };
    public MonsterStance monsterStance;

    [DisplayWithoutEdit] public float monsterDamageTakenThisRound;
    [DisplayWithoutEdit] public bool monsterActionAvailable = true;
    [DisplayWithoutEdit] public bool monsterRecievedStatBoostThisRound = false;
    [DisplayWithoutEdit] public bool monsterCriticallyStrikedThisRound = false;
    [DisplayWithoutEdit] public bool monsterCannotMissAttacks = false;

    public GameObject monsterEnragedTarget;

    [Title("Monster Basic Immunities")]
    public bool monsterImmuneToDebuffs = false;
    public bool monsterImmuneToDamage = false;

    public List<Modifier.StatusEffectType> listofCurrentStatusEffects;

    public List<AttackEffect.StatToChange> listOfStatImmunities;

    public List<ElementClass> listOfElementImmunities;

    public List<Modifier.StatusEffectType> listOfStatusImmunities;

    [Title("Components")]
    [Required] public GameObject combatManagerObject;
    public Animator monsterAnimator;
    public CombatManagerScript combatManagerScript;
    public MonsterAttackManager monsterAttackManager;

    // Start
    private void Start()
    {
        InitiateStats();

        InitiateCreateMonsterObjectStats();

        InitiateHealthBars();

        InitializeSPBar();

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
            monsterStance = monster.cachedMonsterRowPosition;
            monster.cachedMonsterRowPosition = monsterStance;

            monsterReference.currentSP = monsterReference.initialSP;
            monsterReference.cachedHealthAtBattleStart = monsterReference.health;

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
        healthText.text = ($"{monsterReference.health}/{monster.maxHealth}");
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

    public void InitializeSPBar()
    {
        ListOfSPIcons.Clear();

        for (int i = 0; i < monsterReference.maxSP; i++)
        {
            var icon = Instantiate(monsterSPIcon, monsterCanvas.transform);
            icon.transform.localPosition = new Vector3(icon.transform.localPosition.x + (i * 1.1f), icon.transform.localPosition.y + 0.90f, icon.transform.localPosition.z); //0.95f
            ListOfSPIcons.Add(icon);
        }

        if (monsterReference.maxSP >= 4)
        {
            int i = 0;
            foreach(GameObject icon in ListOfSPIcons)
            {
                icon.transform.localPosition += new Vector3(icon.transform.localPosition.x - (i * 1.59f), 0, 0);
                i++;
            }
        }

        UpdateSPBar();
    }

    public void UpdateSPBar()
    {
        for (int i = 0; i < monsterReference.maxSP; i++)
        {
            ListOfSPIcons[i].GetComponent<Image>().color = Color.black;
        }

        for (int i = 0; i < monsterReference.currentSP; i++)
        {
            ListOfSPIcons[i].GetComponent<Image>().color = Color.white;
        }
    }

    // This function initializes a gameObjects components
    public async Task<int> InitializeComponents()
    {
        monsterAnimator = GetComponent<Animator>();

        monsterAttackManager = combatManagerObject.GetComponent<MonsterAttackManager>();

        InteractableToolTipWindow = combatManagerScript.uiManager.InteractableToolTipWindow;

        // Create instances of the monster's attacks
        if (combatManagerScript.adventureMode)
        {
            monsterReference.ListOfCachedMonsterAttacks.Clear();

            foreach (MonsterAttack initialAttack in monsterReference.ListOfMonsterAttacks)
            {
                MonsterAttack newAttack = Instantiate(initialAttack);

                newAttack.name = newAttack.monsterAttackName + " (Non Alterable Clone)";

                monsterReference.ListOfCachedMonsterAttacks.Add(newAttack);
            }

            monsterReference.ListOfMonsterAttacks.Clear();

            foreach (MonsterAttack cachedAttack in monsterReference.ListOfCachedMonsterAttacks)
            {
                MonsterAttack newAttack = Instantiate(cachedAttack);

                newAttack.name = newAttack.monsterAttackName + " (Alterable Clone)";

                monsterReference.ListOfMonsterAttacks.Add(newAttack);
            }
        }

        await Task.Delay(1);

        return 1;
    }

    // Update current health bar and call damaged healthbar fade
    public void UpdateHealthBar()
    {
        // Should also be called when healed
        HealthbarSlider.value = monsterReference.health;
        HealthbarSliderFill.color = new Color(Mathf.Clamp((1 - monsterReference.health / monsterReference.maxHealth), 0, .75f), Mathf.Clamp((monsterReference.health / monsterReference.maxHealth), 0, .75f), 0, 1f);

        // Fade out damaged healthbar
        Invoke(nameof(FadeHealthBarDamageTaken), .5f);
    }

    // Fades the damaged health bar
    public void FadeHealthBarDamageTaken()
    {
        HealthbarSliderFillDamagedFade.CrossFadeAlpha(0, .75f, false);
        Invoke(nameof(AdjustHealthBarDamageTaken), .75f);
    }

    // Adjusts the damaged health bar to match the current health bar after its faded
    public void AdjustHealthBarDamageTaken()
    {
        HealthbarSliderDamaged.value = monsterReference.health;
        HealthbarSliderFillDamagedFade.CrossFadeAlpha(1, .1f, false);
    }


    // This function should be called when stats get updated
    public async Task<int> UpdateStats(bool damageTaken, GameObject damageSourceGameObject, bool externalDamageTaken, float calculatedDamage)
    {
        // Initial check of stats are out of bounds
        CheckStatsCap();

        // Update healthbar and text
        healthText.text = ($"{monsterReference.health}/{monster.maxHealth}"); // \nSpeed: { monsterReference.speed.ToString()}

        // Only check monster is alive if damage was taken - Fixes dual death call bug
        if (damageTaken && gameObject.GetComponent<BoxCollider2D>().enabled)
        {
            if (calculatedDamage > 0)
                HitMonster(calculatedDamage);

            CheckHealth(externalDamageTaken, damageSourceGameObject);

            UpdateHealthBar();
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

        return 1;
    }

    // This function should be called when stats get updated
    public async Task<int> UpdateStats(bool damageTaken, GameObject damageSourceGameObject, bool externalDamageTaken, float calculatedDamage, bool dontShowDamagePopup)
    {
        // Initial check of stats are out of bounds
        CheckStatsCap();

        // Update healthbar and text
        healthText.text = ($"{monsterReference.health}/{monster.maxHealth}");

        // Only check monster is alive if damage was taken - Fixes dual death call bug
        if (damageTaken && gameObject.GetComponent<BoxCollider2D>().enabled)
        {
            if (calculatedDamage > 0 && !dontShowDamagePopup)
                HitMonster(calculatedDamage);

            CheckHealth(externalDamageTaken, damageSourceGameObject);

            UpdateHealthBar();
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

        return 1;
    }

    // This function is called to grant a monster exp upon kill or combat win in adventure mode
    public void GrantExp(int expGained)
    {
        if (expGained <= 0)
            return;

        if (aiType == Monster.AIType.Enemy)
            return;

        expGained += Mathf.RoundToInt(expGained * (combatManagerScript.adventureManager.bonusExp / 100));

        // Gain exp
        monsterReference.monsterCurrentExp += Mathf.RoundToInt(expGained);

        // Show level up animation
        CreateStatusEffectPopup($"+{expGained} Exp", AttackEffect.StatChangeType.Buff);
    }

    // This function level ups the monster in combat
    public void LevelUp()
    {
        // Show level up animation
        LevelUp_VFX.Play();
        CreateStatusEffectPopup("Level Up!!!", AttackEffect.StatChangeType.Buff);
        monsterAttackManager.soundEffectManager.PlaySoundEffect(monsterAttackManager.LevelUpSound);

        List<float> ListOfMonstersLevelUpPrimaryStats = new List<float>();
        List<float> ListOfMonstersLevelUpSecondaryStats = new List<float>();

        // Level up, heal, and update states
        monsterReference.level += 1;
        monsterReference.monsterCurrentExp = Mathf.Abs(monsterReference.monsterExpToNextLevel - monsterReference.monsterCurrentExp);
        monsterReference.monsterExpToNextLevel = Mathf.RoundToInt(monsterReference.monsterExpToNextLevel * 1.15f);

        monster.previouslyCachedMaxHealth = monster.maxHealth;
        monsterReference.maxHealth = monsterReference.maxHealth + Random.Range(monsterReference.healthScaler, monsterReference.healthScaler * 2);
        //monsterReference.health = monsterReference.maxHealth;

        // Basic int to add new stats to cached monster data
        int newStatToCache = 0;

        // Physical Attack
        monster.previouslyCachedPhysicalAttack = monster.cachedPhysicalAttack;
        newStatToCache = Mathf.RoundToInt((monsterReference.cachedPhysicalAttack) + Random.Range(0, monsterReference.physicalAttackScaler + 1));
        monsterReference.physicalAttack += newStatToCache - monsterReference.cachedPhysicalAttack;
        monster.cachedPhysicalAttack = newStatToCache;

        // Magic Attack
        monster.previouslyCachedMagicAttack = monster.cachedMagicAttack;
        newStatToCache = Mathf.RoundToInt((monsterReference.cachedMagicAttack) + Random.Range(0, monsterReference.magicAttackScaler + 1));
        monsterReference.magicAttack += newStatToCache - monsterReference.cachedMagicAttack;
        monster.cachedMagicAttack = newStatToCache;

        // Physical Defense
        monster.previouslyCachedPhysicalDefense = monster.cachedPhysicalDefense;
        newStatToCache = Mathf.RoundToInt((monsterReference.cachedPhysicalDefense) + Random.Range(0, monsterReference.physicalDefenseScaler + 1));
        monsterReference.physicalDefense += newStatToCache - monsterReference.cachedPhysicalDefense;
        monster.cachedPhysicalDefense = newStatToCache;

        // Magic Defense
        monster.previouslyCachedMagicDefense = monster.cachedMagicDefense;
        newStatToCache = Mathf.RoundToInt((monsterReference.cachedMagicDefense) + Random.Range(0, monsterReference.magicDefenseScaler + 1));
        monsterReference.magicDefense += newStatToCache - monsterReference.cachedMagicDefense;
        monster.cachedMagicDefense = newStatToCache;

        // Speed
        monster.previouslyCachedSpeed = monster.cachedSpeed;
        newStatToCache = Mathf.RoundToInt((monsterReference.cachedSpeed) + Random.Range(0, monsterReference.speedScaler + 1));
        monsterReference.speed += newStatToCache - monsterReference.cachedSpeed;
        monster.cachedSpeed = newStatToCache;

        // Finally, update in-combat stats
        UpdateStats(true, null, false, 0);
        combatManagerScript.SortMonsterBattleSequence();
        nameText.text = monster.name + ($" Lvl: {monsterReference.level}");
        InitiateHealthBars();

        // Show Level Up Results Window
        //Invoke("ShowLevelUpResultsWindow", 0.5f);
        StartCoroutine(nameof(ShowLevelUpResultsWindow));
    }

    public void ModifySP(float sp)
    {
        if (sp > 0 && monsterReference.currentSP != monsterReference.maxSP)
            CreateStatusEffectPopup($"+{sp} SP!", AttackEffect.StatChangeType.Buff, true);
        else if (sp < 0)
            CreateStatusEffectPopup($"{sp} SP!", AttackEffect.StatChangeType.Debuff, true);

        monsterReference.currentSP += sp;

        if (monsterReference.currentSP > monsterReference.maxSP)
            monsterReference.currentSP = monsterReference.maxSP;

        if (monsterReference.currentSP < 0)
            monsterReference.currentSP = 0;

        UpdateSPBar();
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
    public void HideLevelUpResultsWindow()
    {
        LevelUpResultsWindow.SetActive(false);

        // Check if exp is still overcapped
        if (monsterReference.monsterCurrentExp >= monsterReference.monsterExpToNextLevel)
        {
            LevelUp();
        }
        else
        {
            combatManagerScript.Invoke(nameof(combatManagerScript.CheckMonsterLevelUps), 0.25f);
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

        if (monsterReference.critDamage > 2.5f)
        {
            monsterReference.critDamage = 2.5f;
        }
    }

    // This function is called on round start to adjust all round start variables
    public async Task<int> OnRoundStart()
    {
        await ResetRoundCombatVariables();

        if (this == null || gameObject == null)
            return 1;

        await monsterAttackManager.TriggerAbilityEffects(monsterReference, gameObject, AttackEffect.EffectTime.RoundStart);

        return 1;
    }

    public async Task<int> OnRoundEnd()
    {
        await CheckCurrentModifiers();

        if (this == null || gameObject == null)
            return 1;

        await monsterAttackManager.TriggerAbilityEffects(monsterReference, gameObject, AttackEffect.EffectTime.RoundEnd);

        return 1;
    }

    public async Task<int> CheckCurrentModifiers()
    {
        foreach (Modifier modifier in monsterReference.ListOfModifiers.ToArray())
        {
            if (this == null || gameObject == null || monsterReference.health <= 0)
                return 1;

            if (modifier.isStatusEffect)
            {
                await modifier.DealModifierStatusEffectDamage(monsterReference, gameObject);
            }

            if (this == null || gameObject == null || monsterReference.health <= 0)
                return 1;

            await modifier.CountdownModifierDuration(monsterReference, gameObject);

            await Task.Delay(75);
        }

        return 1;
    }

    // This function modifies stats by modifier value
    public async Task<int> ModifyStats(AttackEffect attackEffect, Modifier modifier)
    {
        await monsterAttackManager.TriggerAbilityEffects(monsterReference, AttackEffect.EffectTime.OnStatChange, gameObject, modifier);

        switch (attackEffect.statToChange)
        {
            case (AttackEffect.StatToChange.Evasion):
                monsterReference.evasion += modifier.modifierAmount;
                break;

            case (AttackEffect.StatToChange.Speed):
                monsterReference.speed += (int)modifier.modifierAmount;
                UpdateStats(false, null, false, 0);
                combatManagerScript.SortMonsterBattleSequence();
                break;

            case (AttackEffect.StatToChange.MagicAttack):
                monsterReference.magicAttack += (int)modifier.modifierAmount;
                break;
            case (AttackEffect.StatToChange.MagicDefense):
                monsterReference.magicDefense += (int)modifier.modifierAmount;
                break;

            case (AttackEffect.StatToChange.PhysicalAttack):
                monsterReference.physicalAttack += (int)modifier.modifierAmount;
                break;

            case (AttackEffect.StatToChange.PhysicalDefense):
                monsterReference.physicalDefense += (int)modifier.modifierAmount;
                break;

            case (AttackEffect.StatToChange.CritChance):
                monsterReference.critChance += (int)modifier.modifierAmount;
                break;

            case (AttackEffect.StatToChange.CritDamage):
                monsterReference.critDamage += modifier.modifierAmount;
                break;

            case (AttackEffect.StatToChange.Debuffs):
                monsterImmuneToDebuffs = true;
                // Create popup
                CreateStatusEffectPopup("Debuff and Status Immunity!", AttackEffect.StatChangeType.Buff);
                break;

            case (AttackEffect.StatToChange.Damage):
                monsterImmuneToDamage = true;
                // Create popup
                CreateStatusEffectPopup("Damage Immunity!", AttackEffect.StatChangeType.Buff);
                break;

            case (AttackEffect.StatToChange.BothOffensiveStats):
                monsterReference.physicalAttack += (int)modifier.modifierAmount;
                monsterReference.magicAttack += (int)modifier.modifierAmount;
                break;

            case (AttackEffect.StatToChange.Health):
                if (modifier.statusEffectType == Modifier.StatusEffectType.None && attackEffect.effectDamageType != MonsterAttack.MonsterAttackDamageType.None && attackEffect.statChangeType == AttackEffect.StatChangeType.Debuff)
                {
                    Debug.Log($"Calling damage on {monsterReference.name} from {attackEffect} by {modifier.modifierOwner.name}!");
                    await monsterAttackManager.Damage(monsterReference, gameObject, attackEffect, modifier.modifierOwner, modifier.modifierOwnerGameObject, modifier);
                    return 1;
                }
                else
                {
                    monsterReference.health += (int)modifier.modifierAmount;
                }
                break;

            case (AttackEffect.StatToChange.Accuracy):
                monsterReference.bonusAccuracy += (int)modifier.modifierAmount;
                break;

            case (AttackEffect.StatToChange.HighestAttackStat):
                if (MonsterAttackManager.ReturnMonsterHighestAttackStat(monsterReference) == MonsterAttack.MonsterAttackDamageType.Magical)
                {
                    modifier.statModified = AttackEffect.StatToChange.MagicAttack;
                    monsterReference.magicAttack += (int)modifier.modifierAmount;
                }
                else if (MonsterAttackManager.ReturnMonsterHighestAttackStat(monsterReference) == MonsterAttack.MonsterAttackDamageType.Physical)
                {
                    modifier.statModified = AttackEffect.StatToChange.PhysicalAttack;
                    monsterReference.physicalAttack += (int)modifier.modifierAmount;
                }
                break;

            case (AttackEffect.StatToChange.Immunity):
                CreateStatusEffectPopup($"{attackEffect.immunityType} Immunity!", AttackEffect.StatChangeType.Buff);
                break;

            default:
                Debug.Log("Missing stat to modify to modifier?", this);
                break;
        }

        if (modifier.statusEffectType == Modifier.StatusEffectType.None)
            attackEffect.CallStatAdjustment(monsterReference, this, monsterAttackManager, attackEffect, modifier);

        if (modifier.statChangeType == AttackEffect.StatChangeType.Buff)
            monsterRecievedStatBoostThisRound = true;

        AddStatusIcon(modifier, attackEffect.statToChange, modifier.modifierCurrentDuration);

        return 1;
    }

    // This function adds the modifer's icon to the monster's HUD
    public void AddStatusIcon(Modifier modifier, AttackEffect.StatToChange statEnumToChange, int effectDuration)
    {
        //List<Modifier> modList = monsterReference.ListOfModifiers.Where(mod => mod.statModified == modifier.statModified && mod.modifierSource == modifier.modifierSource).ToList();

        //if (modList.Count == 1)
        //{
        if (statEnumToChange == AttackEffect.StatToChange.Immunity)
        {
            GameObject statusIcon = Instantiate(statusEffectIcon, statusEffectHolder.transform);
            modifier.statusEffectIconGameObject = statusIcon;
            StatusEffectIcon newStatusEffectIcon = statusIcon.AddComponent<StatusEffectIcon>();
            newStatusEffectIcon.modifier = modifier;

            // Initiate Interactable components
            newStatusEffectIcon.modifier.modifierName = modifier.modifierSource;

            switch (modifier.attackEffect.immunityType)
            {
                case AttackEffect.ImmunityType.Element:
                    newStatusEffectIcon.modifier.modifierDescription = ($"{modifier.attackEffect.elementImmunity.element.ToString()} Element Immunity");
                    break;

                case AttackEffect.ImmunityType.Status:
                    newStatusEffectIcon.modifier.modifierDescription = ($"{modifier.attackEffect.statusImmunity} Status Immunity");
                    break;

                case AttackEffect.ImmunityType.SpecificStatChange:
                    newStatusEffectIcon.modifier.modifierDescription = ($"{modifier.attackEffect.statImmunity} Debuff Immunity");
                    break;

                case AttackEffect.ImmunityType.Damage:
                    newStatusEffectIcon.modifier.modifierDescription = ("Damage Immunity");
                    break;

                case AttackEffect.ImmunityType.Death:
                    newStatusEffectIcon.modifier.modifierDescription = ("Death Immunity");
                    break;

                case AttackEffect.ImmunityType.Debuffs:
                    newStatusEffectIcon.modifier.modifierDescription = ("Status Effects and Debuffs Immunity");
                    break;

                default:
                    newStatusEffectIcon.modifier.modifierDescription = ("Missing Immunity?");
                    break;
            }

            newStatusEffectIcon.InitiateStatusEffectIcon(this);
            return;
        }

        if (modifier.modifierType != Modifier.ModifierType.equipmentModifier && statEnumToChange != AttackEffect.StatToChange.Health)
        {
            if (modifier.modifierDuration == 0 && modifier.statModified != AttackEffect.StatToChange.Debuffs)
                return;

            GameObject statusIcon = Instantiate(statusEffectIcon, statusEffectHolder.transform);
            modifier.statusEffectIconGameObject = statusIcon;
            StatusEffectIcon newStatusEffectIcon = statusIcon.AddComponent<StatusEffectIcon>();
            newStatusEffectIcon.modifier = modifier;

            // Initiate Interactable components
            newStatusEffectIcon.modifier.modifierName = modifier.modifierSource;
            newStatusEffectIcon.modifier.modifierDescription = ($"{ReturnSign(modifier.statChangeType)}{modifier.modifierAmount} {modifier.statModified.ToString()}");

            newStatusEffectIcon.InitiateStatusEffectIcon(this);
            return;
        }

        if (modifier.isStatusEffect)
        {
            GameObject statusIcon = Instantiate(statusEffectIcon, statusEffectHolder.transform);
            modifier.statusEffectIconGameObject = statusIcon;
            StatusEffectIcon newStatusEffectIcon = statusIcon.AddComponent<StatusEffectIcon>();
            newStatusEffectIcon.modifier = modifier;

            // Initiate Interactable components
            newStatusEffectIcon.modifier.modifierName = modifier.modifierSource;
            newStatusEffectIcon.modifier.modifierDescription = ($"{modifier.statusEffectType.ToString()}");

            switch (modifier.statusEffectType)
            {
                case (Modifier.StatusEffectType.Burning):
                    newStatusEffectIcon.modifier.modifierDescription +=
                        ($", {modifier.modifierAmount * 100f}% Current Health damage per round.");
                    break;

                case (Modifier.StatusEffectType.Poisoned):
                    newStatusEffectIcon.modifier.modifierDescription +=
                        ($", {modifier.modifierAmount * 100f}% Max Health damage per round.");
                    break;

                case (Modifier.StatusEffectType.Stunned):
                    newStatusEffectIcon.modifier.modifierDescription +=
                        ($", Cannot act for {modifier.modifierCurrentDuration} rounds.");
                    break;

                case (Modifier.StatusEffectType.Dazed):
                    newStatusEffectIcon.modifier.modifierDescription +=
                        ($", 50% chance to select a random attack and/or target.");
                    break;

                case (Modifier.StatusEffectType.Enraged):
                    monsterEnragedTarget = modifier.attackEffect.monsterAttackTrigger.monsterAttackSourceGameObject;
                    newStatusEffectIcon.modifier.modifierDescription +=
                        ($", Can only target whoever Enraged me ({monsterEnragedTarget.GetComponent<CreateMonster>().monsterReference.aiType} {monsterEnragedTarget.GetComponent<CreateMonster>().monsterReference.name}).");
                    break;

                case (Modifier.StatusEffectType.Silenced):
                    newStatusEffectIcon.modifier.modifierDescription +=
                        ($", Cannot use Status attacks.");
                    break;

                case (Modifier.StatusEffectType.Weakened):
                    newStatusEffectIcon.modifier.modifierDescription +=
                        ($", Take {modifier.modifierAmount * 100f}% more damage during combat.");
                    break;

                case (Modifier.StatusEffectType.Crippled):
                    newStatusEffectIcon.modifier.modifierDescription +=
                        ($", Cannot recieve buffs.");
                    break;

                default:
                    break;
            }


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
        if (modifier.isStatusEffect)
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

                case (Modifier.StatusEffectType.Enraged):
                    return combatManagerScript.uiManager.enragedUISprite;

                default:
                    return combatManagerScript.monsterAttackManager.poisonedUISprite;
            }
        }

        switch (modifier.statModified)
        {
            case (AttackEffect.StatToChange.Speed):
                return combatManagerScript.uiManager.speedSprite;

            case (AttackEffect.StatToChange.PhysicalAttack):
                return combatManagerScript.uiManager.physicalAttackSprite;

            case (AttackEffect.StatToChange.PhysicalDefense):
                return combatManagerScript.uiManager.physicalDefenseSprite;

            case (AttackEffect.StatToChange.MagicAttack):
                return combatManagerScript.uiManager.magicAttackSprite;

            case (AttackEffect.StatToChange.MagicDefense):
                return combatManagerScript.uiManager.magicDefenseSprite;

            case (AttackEffect.StatToChange.Evasion):
                return combatManagerScript.uiManager.evasionSprite;

            case (AttackEffect.StatToChange.CritChance):
                return combatManagerScript.uiManager.critChanceSprite;

            case (AttackEffect.StatToChange.CritDamage):
                return combatManagerScript.uiManager.critDamageSprite;

            case (AttackEffect.StatToChange.Damage):
                return combatManagerScript.uiManager.damageImmuneUISprite;

            case (AttackEffect.StatToChange.Debuffs):
                return combatManagerScript.uiManager.debuffsImmuneUISprite;

            case (AttackEffect.StatToChange.Accuracy):
                return combatManagerScript.uiManager.accuracySprite;

            case (AttackEffect.StatToChange.Immunity):
                return combatManagerScript.uiManager.debuffsImmuneUISprite;

            default:
                return combatManagerScript.monsterAttackManager.poisonedUISprite;
        }
    }

    // This function inflicts statuses
    public void InflictStatus(Modifier.StatusEffectType statusEffect)
    {
        listofCurrentStatusEffects.Add(statusEffect);

        CreateStatusEffectPopup($"{statusEffect}!", AttackEffect.StatChangeType.Debuff);
    }

    // This function refreshs per-round combat variables
    public async Task<int> ResetRoundCombatVariables()
    {
        monsterDamageTakenThisRound = 0;
        monsterActionAvailable = true;
        monsterRecievedStatBoostThisRound = false;
        monsterCriticallyStrikedThisRound = false;
        return 1;
    }

    // This function checks the monster's health
    public void CheckHealth(bool killedExternally, GameObject externalKillerGameObject)
    {
        // Called after a delay
        if (monsterReference.health <= 0)
        {
            // Remove outside components
            transform.DetachChildren();
            gameObject.GetComponent<BoxCollider2D>().enabled = false;

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

            //await combatManagerScript.monsterAttackManager.TriggerAbilityEffects(monster, AttackEffect.EffectTime.OnDeath, gameObject);

            combatManagerScript.RemoveMonsterFromList(gameObject, monsterReference.aiType);
            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monsterReference.aiType} {monsterReference.name} has been defeated!", monsterReference.aiType);
        }
    }

    public void SetMonsterStance(MonsterStance newMonsterStance)
    {
        monsterStance = newMonsterStance;

        switch (newMonsterStance)
        {
            case MonsterStance.Neutral:
                monsterOffensiveStanceIcon.enabled = false;
                monsterDefensiveStanceIcon.enabled = false;
                break;

            case MonsterStance.Defensive:
                monsterOffensiveStanceIcon.enabled = false;
                monsterDefensiveStanceIcon.enabled = true;
                break;

            case MonsterStance.Aggressive:
                monsterOffensiveStanceIcon.enabled = true;
                monsterDefensiveStanceIcon.enabled = false;
                break;
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

            SetMonsterStance(monsterStance);
            
            monsterReference.aiLevel = aiLevel;

            sr.flipX = true;

            return;
        }

        if (monsterReference.aiType == Monster.AIType.Ally)
        {
            nameText.color = Color.white;

            SetMonsterStance(monsterStance);

            monsterReference.aiLevel = Monster.AILevel.Player;

            sr.flipX = false;

            return;
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
        if (gameObject == null)      
            return;

        combatManagerScript.CycleTargets(gameObject);

        if (combatManagerScript.targeting)
        {
            combatManagerScript.uiManager.ClearTargeters();

            if (monsterAttackManager.currentMonsterAttack != null && monsterAttackManager.currentMonsterAttack.monsterAttackTargetCount == MonsterAttack.MonsterAttackTargetCount.AllTargets)
            {
                if (gameObject.GetComponent<CreateMonster>().monsterReference.aiType == Monster.AIType.Enemy)
                {
                    foreach (GameObject monster in combatManagerScript.ListOfEnemies)
                    {
                        combatManagerScript.uiManager.InstantiateTargeterToMonsterPosition(monster);
                    }
                }
                else
                {
                    foreach (GameObject monster in combatManagerScript.ListOfAllys)
                    {
                        combatManagerScript.uiManager.InstantiateTargeterToMonsterPosition(monster);
                    }
                }

                return;
            }

            combatManagerScript.uiManager.InstantiateTargeterToMonsterPosition(gameObject);
        }

        if (currentTime < delayTime)
            return;

        DisplayStatScreenWindow(true);
    }

    // This function selects the hovered monster for combat
    private void OnMouseOver()
    {
        if (gameObject == null)
            return;

        if (combatManagerScript.CurrentMonsterTurn == null)
            return;

        // Confirm Target
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            CreateMonster monsterComponent = combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>();
            Monster monsterReference = monsterComponent.monsterReference;

            if (combatManagerScript.itemPending)
            {
                combatManagerScript.UseItem(gameObject);
                return;
            }

            if (!combatManagerScript.targeting)
                return;

            if (combatManagerScript.monsterTurn != CombatManagerScript.MonsterTurn.AllyTurn)
                return;

            if (monsterAttackManager.currentMonsterAttack == null)
                return;

            if (monsterAttackManager.currentMonsterAttack.monsterAttackSPCost > monsterReference.currentSP)
            {
                combatManagerScript.uiManager.EditCombatMessage("Not enough SP!");
                return;
            }

            if (monsterAttackManager.ListOfCurrentlyTargetedMonsters.Count >= monsterAttackManager.currentMonsterAttack.monsterAttackTargetCountNumber)
                return;

            if (monsterComponent.listofCurrentStatusEffects.Contains(Modifier.StatusEffectType.Enraged) && combatManagerScript.CurrentTargetedMonster != monsterComponent.monsterEnragedTarget)
            {
                monsterAttackManager.uiManager.EditCombatMessage($"{monsterReference.aiType} {monsterReference.name} is {Modifier.StatusEffectType.Enraged} " +
                $"and can only target {monsterComponent.monsterEnragedTarget.GetComponent<CreateMonster>().monsterReference.aiType} " +
                $"{monsterComponent.monsterEnragedTarget.GetComponent<CreateMonster>().monsterReference.name}!");
                return;
            }

            // Check if the current attack is a multi-target attack and has the correct amount of targets
            if (monsterAttackManager.currentMonsterAttack.monsterAttackTargetCount == MonsterAttack.MonsterAttackTargetCount.MultiTarget)
            {
                if (monsterAttackManager.ListOfCurrentlyTargetedMonsters.Count < monsterAttackManager.currentMonsterAttack.monsterAttackTargetCountNumber)
                {
                    monsterAttackManager.ListOfCurrentlyTargetedMonsters.Add(combatManagerScript.CurrentTargetedMonster);
                    combatManagerScript.uiManager.InstantiateTargeterToMonsterPosition(gameObject);

                    if (monsterAttackManager.ListOfCurrentlyTargetedMonsters.Count != monsterAttackManager.currentMonsterAttack.monsterAttackTargetCountNumber)
                        monsterAttackManager.HUDanimationManager.MonsterCurrentTurnText.text =
                            ($"Select {monsterAttackManager.currentMonsterAttack.monsterAttackTargetCountNumber - monsterAttackManager.ListOfCurrentlyTargetedMonsters.Count} target(s)...");

                    if (monsterAttackManager.ListOfCurrentlyTargetedMonsters.Count != monsterAttackManager.currentMonsterAttack.monsterAttackTargetCountNumber)
                        return;
                }
            }

            monsterAttackManager.UseMonsterAttack();
        }

        // Show detailed monster info window
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            combatManagerScript.uiManager.DetailedMonsterStatsWindow.SetActive(true);
            combatManagerScript.uiManager.DetailedMonsterStatsWindow.GetComponent<MonsterStatScreenScript>().DisplayMonsterStatScreenStats(monsterReference);
            combatManagerScript.uiManager.DetailedMonsterStatsWindow.GetComponent<InventoryManager>().currentMonster = monster;
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
        if (gameObject == null)
            return;

        windowShowing = false;
        currentTime = 0.0f;
        combatManagerScript.CycleTargets(gameObject);
        DisplayStatScreenWindow(false);
    }

    // This function is called when the mouse hovers the monster, bringing up the stat screen window
    public void DisplayStatScreenWindow(bool showWindow)
    {
        if (gameObject == null)
            return;

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

    public void CreateDamageEffectPopup(float damage, string damageOrHeal)
    {
        GameObject effectPopup = Instantiate(monsterStatusTextObjectCanvas, popupPosTransform);
        effectPopup.GetComponentInChildren<PopupScript>().instantiated = true;
        effectPopup.GetComponentInChildren<PopupScript>().parentObj = effectPopup;

        float amount = damage;

        if (damageOrHeal == "Heal")
        {
            effectPopup.GetComponentInChildren<TextMeshProUGUI>().color = Color.green;
            effectPopup.GetComponentInChildren<TextMeshProUGUI>().text = ($"+{amount}");
        }
        else
        {
            amount = Mathf.Abs(amount);

            if (amount == 0)
                return;

            effectPopup.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
            effectPopup.GetComponentInChildren<TextMeshProUGUI>().text = ($"-{amount}");
        }
    }

    public void CreateStatusEffectPopup(AttackEffect.StatToChange stat, AttackEffect.StatChangeType statChangeType)
    {
        GameObject effectPopup = Instantiate(monsterStatusTextObjectCanvas, popupPosTransform);
        effectPopup.GetComponentInChildren<PopupScript>().instantiated = true;
        effectPopup.GetComponentInChildren<PopupScript>().parentObj = effectPopup;
        effectPopup.GetComponentInChildren<Animator>().speed = 1.25f;

        if (statChangeType == AttackEffect.StatChangeType.Debuff)
        {
            effectPopup.GetComponentInChildren<TextMeshProUGUI>().color = debuffColor;
            effectPopup.GetComponentInChildren<TextMeshProUGUI>().text = ($"{stat} down!");
        }
        else
        {
            effectPopup.GetComponentInChildren<TextMeshProUGUI>().color = buffColor;
            effectPopup.GetComponentInChildren<TextMeshProUGUI>().text = ($"{stat} up!");
        }
    }

    public void CreateStatusEffectPopup(string condition, AttackEffect.StatChangeType statChangeType)
    {
        GameObject effectPopup = Instantiate(monsterStatusTextObjectCanvas, popupPosTransform);
        monsterAttackManager.soundEffectManager.PlaySoundEffect(monsterAttackManager.buffSound);
        effectPopup.GetComponentInChildren<PopupScript>().instantiated = true;
        effectPopup.GetComponentInChildren<PopupScript>().parentObj = effectPopup;
        effectPopup.GetComponentInChildren<TextMeshProUGUI>().text = ($"{condition}!");

        if (statChangeType == AttackEffect.StatChangeType.Debuff)
        {
            effectPopup.GetComponentInChildren<TextMeshProUGUI>().color = debuffColor;
        }
        else if (statChangeType == AttackEffect.StatChangeType.Buff)
        {
            effectPopup.GetComponentInChildren<TextMeshProUGUI>().color = buffColor;
        }
        else
        {
            effectPopup.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

        }
    }

    public void CreateStatusEffectPopup(string condition, AttackEffect.StatChangeType statChangeType, bool useHigherTransform)
    {
        GameObject effectPopup = Instantiate(monsterStatusTextObjectCanvas, popupPosTransformHigher);
        monsterAttackManager.soundEffectManager.PlaySoundEffect(monsterAttackManager.buffSound);
        effectPopup.GetComponentInChildren<PopupScript>().instantiated = true;
        effectPopup.GetComponentInChildren<PopupScript>().parentObj = effectPopup;
        effectPopup.GetComponentInChildren<TextMeshProUGUI>().text = ($"{condition}!");

        if (statChangeType == AttackEffect.StatChangeType.Debuff)
        {
            effectPopup.GetComponentInChildren<TextMeshProUGUI>().color = debuffColor;
        }
        else if (statChangeType == AttackEffect.StatChangeType.Debuff)
        {
            effectPopup.GetComponentInChildren<TextMeshProUGUI>().color = buffColor;
        }
        else
        {
            effectPopup.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        }
    }

    public void HitMonster(float damageAmount)
    {
        monsterAnimator.SetBool("hitAnimationPlaying", true);

        monsterAttackManager.soundEffectManager.PlaySoundEffect(monsterAttackManager.HitSound);

        CreateDamageEffectPopup(damageAmount, "Damage");
    }
}
