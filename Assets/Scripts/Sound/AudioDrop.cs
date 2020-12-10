using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioDrop : MonoBehaviour
{
    public AudioSource audioSource;

    private void Update()
    {
        if (!audioSource.isPlaying)
            Destroy(gameObject);
    }
}
