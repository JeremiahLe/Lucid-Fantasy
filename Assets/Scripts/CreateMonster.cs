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

    [SerializeField] private Transform startingPosition;
    private enum CombatOrientation { Left, Right };
    [SerializeField] private CombatOrientation combatOrientation;

    [Header("Additional Editable Variables")]
    [SerializeReference] private int monsterSpeed;


    private void Start()
    {
        InitateStats();
        SetAIType();
    }

    // This function sets monster stats on HUD at battle start
    private void InitateStats()
    {
        monsterReference = Instantiate(monster); // this is needed to create instances of the scriptable objects rather than editing them
        monsterReference.aiType = aiType;

        monsterSpeed = monster.speed; // this is needed to not edit the base scriptable objects
        monsterReference.speed = monsterSpeed;

        nameText.text = monster.name + ($" Lvl: {monster.level}");
        healthText.text = ($"HP: {monster.health.ToString()}/{monster.maxHealth.ToString()}\nSpeed: {monsterReference.speed.ToString()}");
        sr.sprite = monster.baseSprite;
    }

    // This function sets monster sprite orientation at battle start
    private void SetPositionAndOrientation(Transform _startPos, CombatOrientation _combatOrientation)
    {
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
        }
    }
}
