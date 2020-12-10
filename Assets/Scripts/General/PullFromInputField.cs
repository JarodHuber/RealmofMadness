using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PullFromInputField : MonoBehaviour
{
    InputField input;

    void Start()
    {
        input = GetComponent<InputField>();
        var se = new InputField.SubmitEvent();
        se.AddListener(SubmitName);
        input.onEndEdit = se;
    }

    private void SubmitName(string arg0)
    {
        if (isValid(arg0))
        {
            PlayerPrefs.SetString("Seed", arg0);
            PlayerPrefs.SetString("voSeed", "0r1r1r1r1r1r1");
            PlayerPrefs.SetString("voFailSeed", "1r1r1r1r1r1r1r1r1r1");
            PlayerPrefs.SetInt("puzzleSolved", 1);
            SceneManager.LoadScene("MainScene");
        }
        else
        {
            input.text = "";
        }
    }

    bool isValid(string arg0)
    {
        if (arg0.Contains("r"))
        {
            if(arg0.Split('r').Length == 12)
            {
                for(int counter = 0; counter < arg0.Split('r').Length; counter++)
                {
                    try
                    {
                        int.Parse(arg0.Split('r')[counter]);
                    }
                    catch
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        return false;
    }
}
