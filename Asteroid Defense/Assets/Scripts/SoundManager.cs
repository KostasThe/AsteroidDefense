using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public static SoundManager Instance;

    public AudioClip explosion;

    public AudioSource source;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("Multiple instances of SoundManager!");
            Destroy(this);
            return;
        }
        if (Instance == null)
            Instance = this;
    }
}
