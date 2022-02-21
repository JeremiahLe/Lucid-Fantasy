using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreateMonster : MonoBehaviour
{
    public Monster monster;

    [Header("Display Variables")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private SpriteRenderer sr;

    [SerializeField] private Monster.AIType aiType;

    [SerializeField] private Transform startingPosition;
    private enum CombatOrientation { Left, Right };
    [SerializeField] private CombatOrientation combatOrientation;


    private void Start()
    {
        InitateStats();
        SetPositionAndOrientation(startingPosition, combatOrientation);
    }

    // This function sets monster stats on HUD at battle start
    private void InitateStats()
    {
        nameText.text = monster.name + ($" Lvl: {monster.level}");
        healthText.text = ($"HP: {monster.health.ToString()}/{monster.maxHealth.ToString()}\nSpeed: {monster.speed.ToString()}");
        sr.sprite = monster.baseSprite;
        monster.aiType = aiType;
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
}
