using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Win : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("EndScene");
    }
    private void OnTriggerEnter(Collider other)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("EndScene");
    }
}
