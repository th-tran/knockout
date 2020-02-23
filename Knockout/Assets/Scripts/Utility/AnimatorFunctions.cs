using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorFunctions : MonoBehaviour
{
    [SerializeField] AudioClip[] sounds;
    [SerializeField] AudioSource audioSource;

    // Use this for initialization
    void Start ()
    {
        if (audioSource == null)
        {
            audioSource = Player.Instance.audioSource;
        }
    }

    void PlaySound(AudioClip sound)
    {
        SoundManager.Instance.audioSource.PlayOneShot(sound);
    }
}
