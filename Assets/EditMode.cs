using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EditMode : MonoBehaviour
{
    public SpriteRenderer sr;
    public CreateMonster monsterComponent;

    private void Awake()
    {
        if (!Application.isEditor)
            enabled = false;

        monsterComponent = GetComponent<CreateMonster>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        sr.sprite = monsterComponent.monster.baseSprite;
    }
}
