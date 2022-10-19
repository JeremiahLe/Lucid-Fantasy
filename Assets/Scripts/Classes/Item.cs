using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

[Serializable]
[CreateAssetMenu(fileName = "New Item", menuName = "Items")]
public class Item : ScriptableObject
{
    public enum ItemType { Consumable, AscensionMaterial }
    public ItemType itemType;

    public string itemName;

    [TextArea]
    public string itemDescription;

    public Sprite baseSprite;

    [EnableIf("itemType", ItemType.Consumable)]
    public AttackEffect.StatToChange consumableStatToChange;

    [EnableIf("itemType", ItemType.Consumable)]
    public float consumableAmount;

    [EnableIf("itemType", ItemType.Consumable)]
    public bool consumableFlatAmount;

    public Modifier.ModifierRarity itemRarity;
}
