using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[CreateAssetMenu(fileName = "New Adventure", menuName = "Adventures")]
public class Adventure : ScriptableObject
{
    [Header("Adventure Init")]
    public string adventureName;

    public enum AdventureDifficulty { Easy, Medium, Hard }
    public AdventureDifficulty adventureDifficulty;

    public string adventureDescription;

    public Sprite adventureIcon;

    public Monster adventureBoss;

}
