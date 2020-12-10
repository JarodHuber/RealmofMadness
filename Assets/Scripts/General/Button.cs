using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Button : MonoBehaviour
{
    public int SceneToChangeTo;

    public void ChangeScene(AudioSource song = null)
    {
        if (SceneToChangeTo == 2)
        {
            PlayerPrefs.SetString("Seed", "1r0r0r0r0r0r0r0r19r32r32r32");
            PlayerPrefs.SetString("voSeed", "0r0r0r0r0r0r0");
            PlayerPrefs.SetString("voFailSeed", "1r1r1r1r1r1r1r1r1r1");
        }
        else if (SceneToChangeTo == 1)
            PlayerPrefs.SetFloat("startSongTime", song.time);

        SceneManager.LoadScene(SceneToChangeTo);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void difficultyAugement(Image image)
    {
        if (PuzzleGeneration.MaxDifficulty == 10)
        {
            PuzzleGeneration.MaxDifficulty = 50;
            image.color = new Color(0.5686275f, 0.8588236f, 0.7960785f);
            PlayerPrefs.SetInt("Debug/Presentation", 0);
        }
        else
        {
            PuzzleGeneration.MaxDifficulty = 10;
            image.color = new Color(0.8588235f, 0.6066108f, 0.5686275f);
            PlayerPrefs.SetInt("Debug/Presentation", 1);
        }
    }
}
