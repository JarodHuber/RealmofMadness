using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset : MonoBehaviour
{
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("lvlNum", 0);
        PlayerPrefs.SetInt("puzzleSolved", 1);
        PlayerPrefs.SetInt("failedAttempts", 0);
    }
}
