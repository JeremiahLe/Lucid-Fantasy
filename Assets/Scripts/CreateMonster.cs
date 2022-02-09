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

    [SerializeField] private Transform startingPosition;
    private enum CombatOrientation { Left, Right };
    [SerializeField] private CombatOrientation combatOrientation;


    private void Start()
    {
        nameText.text = monster.name + ($" Lvl: {monster.level}");
        healthText.text = ($"HP: {monster.health.ToString()}/{monster.maxHealth.ToString()}");
        sr.sprite = monster.baseSprite;

        SetPositionAndOrientation(startingPosition, combatOrientation);
    }

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
