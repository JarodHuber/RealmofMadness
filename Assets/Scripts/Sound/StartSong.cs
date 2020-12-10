using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSong : MonoBehaviour
{
    public AudioSource audioSource;
    public float loopStart = 12.23f;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
            PlayerPrefs.SetFloat("startSongTime", 0);
        else
            audioSource.time = PlayerPrefs.GetFloat("startSongTime");
    }

    void Update()
    {
        if(SceneManager.GetActiveScene().name == "CreditScene")
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
                audioSource.volume -= 0.05f;
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                audioSource.volume += 0.05f;
        }

        if (!audioSource.isPlaying)
        {
            audioSource.time = loopStart;
            audioSource.Play();
        }
    }
}