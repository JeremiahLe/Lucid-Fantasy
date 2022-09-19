using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AscensionImageScript : MonoBehaviour
{
    public InventoryManager inventoryManager;

    public void CallSpriteChange()
    {
        inventoryManager.ChangeMonsterSprite();
    }

    public void CallAscension()
    {
        inventoryManager.AscendMonster();
    }

    public void PlayAscensionSoundEffect()
    {
        inventoryManager.adventureManager.PlaySFX(inventoryManager.adventureManager.monsterAscendedSFX);
    }
}
