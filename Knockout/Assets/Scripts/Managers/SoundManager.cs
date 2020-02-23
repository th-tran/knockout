using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource audioSource;

    static SoundManager instance;

	public static SoundManager Instance
    {
        get
        {
            if (instance == null) instance = GameObject.FindObjectOfType<SoundManager>();
            return instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
}
