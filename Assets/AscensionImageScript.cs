using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AscensionImageScript : MonoBehaviour
{
    public InventoryManager inventoryManager;
    public ParticleSystem ascensionVFX;

    // Change the monster's sprite halfway through ascension
    public void CallSpriteChange()
    {
        inventoryManager.ChangeMonsterSprite();
        PlayAscensionVFX();
    }

    // Trigger after the ascension ends
    public void CallAscension()
    {
        inventoryManager.AscendMonster();
    }

    // Play the ascension particle effects
    public void PlayAscensionVFX()
    {
        ascensionVFX.Play();
    }

    // Play the ascension sound effect after it's complete
    public void PlayAscensionSoundEffect()
    {
        inventoryManager.adventureManager.PlaySFX(inventoryManager.adventureManager.monsterAscendedSFX);
    }
}
