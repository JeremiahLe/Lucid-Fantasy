using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectManager : MonoBehaviour
{
    public AudioSource soundEffectAudioSource;
    public List<AudioClip> soundEffectQueue;

    [Header("General SFX")]
    public AudioClip LevelUpSound;

    [Header("UI SFX")]
    public AudioClip UISelectSFX1;

    [Header("Combat SFX")]
    public AudioClip CritSound;
    public AudioClip HitSound;
    public AudioClip MissSound;
    public AudioClip ResistSound;
    public AudioClip debuffSound;
    public AudioClip buffSound;

    //// This function adds the passed in sound effect to a queue of sound effects to play
    //public void AddSoundEffectToQueue(AudioClip clip)
    //{
    //    soundEffectQueue.Add(clip);
    //}

    //// This helper function begins the playSoundEffects coroutine()
    //public void BeginSoundEffectQueue()
    //{
    //    StartCoroutine(PlaySoundEffects());
    //}

    //// This plays every sound effect in the queue
    //IEnumerator PlaySoundEffects()
    //{
    //    //Debug.Log("I got called");
    //    foreach (AudioClip clip in soundEffectQueue.ToArray())
    //    {
    //        soundEffectAudioSource.PlayOneShot(clip);
    //        yield return new WaitForSeconds(.15f);
    //        soundEffectQueue.Remove(clip);
    //    }
    //}

    public void PlaySoundEffectLevelUp()
    {
        soundEffectAudioSource.PlayOneShot(LevelUpSound);
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        soundEffectAudioSource.PlayOneShot(clip);
    }

    public void PlaySoundEffectBasicUISFX()
    {
        soundEffectAudioSource.PlayOneShot(UISelectSFX1);
    }
}
