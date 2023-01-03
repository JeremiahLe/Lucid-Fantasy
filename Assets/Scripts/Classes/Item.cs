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

    public Modifier.ModifierRarity itemRarity;

    [EnableIf("itemType", ItemType.Consumable)]
    public List<AttackEffect> listOfItemEffects;

    [EnableIf("itemType", ItemType.Consumable)]
    public List<IAbilityTrigger> listOfItemEffectTriggers;
}
