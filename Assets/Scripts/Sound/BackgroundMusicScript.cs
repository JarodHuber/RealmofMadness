using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusicScript : MonoBehaviour
{
    public AudioSource audioSource;

    private void Awake()
    {
        audioSource.time = PlayerPrefs.GetFloat("songPoint");
    }
}