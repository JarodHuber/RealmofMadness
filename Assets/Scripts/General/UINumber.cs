using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINumber : MonoBehaviour
{
    [HideInInspector]
    public int number = 0;

    public Image numberOnes, numberTens;
    public Vector2 centerPos, rightPos;
    public List<Sprite> digits;

    public void Number(int num)
    {
        number = num;
        int numTens = num / 10;
        int numOnes = num % 10;

        if(numTens == 0)
        {
            numberTens.enabled = false;
            numberOnes.rectTransform.localPosition = centerPos;
            numberOnes.sprite = digits[numOnes];
        }
        else
        {
            numberTens.enabled = true;
            numberOnes.rectTransform.localPosition = rightPos;
            numberOnes.sprite = digits[numOnes];

            numberTens.sprite = digits[numTens];
        }
    }
}
