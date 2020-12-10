using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndVO : MonoBehaviour
{
    public VOScript voScript;
    public float WaitTime = 10.0f;
    float timer = 0.0f;

    private void Start()
    {
        voScript.End();
    }

    private void Update()
    {
        if (voScript.NotComplete() && !voScript.audioSource.isPlaying)
        {
            if (timer < WaitTime)
                timer += Time.deltaTime;
            else
            {
                voScript.Play(VOScript.PlayType.NextLine);
                timer = 0;
            }
        }
    }
}
