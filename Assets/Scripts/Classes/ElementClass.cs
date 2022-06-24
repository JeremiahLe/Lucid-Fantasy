using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "New Element", menuName = "Elements")]
public class ElementClass : ScriptableObject
{
    // Element Type
    public enum MonsterElement
    {
        Fire, Water, Earth, Wind,
        Shadow, Neutral, None, Light, Time,
        Elixir, Electric, Stone, Sound
    };
    public MonsterElement element;

    public List<MonsterElement> listOfWeaknesses;
    public List<MonsterElement> listOfResistances;
}
