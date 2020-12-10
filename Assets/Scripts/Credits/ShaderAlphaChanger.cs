using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderAlphaChanger : MonoBehaviour
{
    public FancyCreds dad;
    public SpriteRenderer dadRenderer, thisRenderer;
    public Vector4 start = Vector4.one, finish = Vector4.zero, hold;
    public Material thisMat;
    public Material thatMat;
    public float timer, timeToFinishAnim, timeToFinishHalf;
    public bool isFirstHalf = true;
    // Start is called before the first frame update
    void Start()
    {
        print("Step2");
        timeToFinishHalf = timeToFinishAnim / 2;
        thatMat.SetColor("_Color", finish);
        thatMat.SetColor("_Color2", finish);
        thisMat.SetColor("_Color", start);
        thisMat.SetColor("_Color2", start);
    }

    // Update is called once per frame
    void Update()
    {
        print(isFirstHalf);
        if (timer >= 1 && isFirstHalf)
        {
            timer = 0;
            isFirstHalf = false;
        } else if (timer >= 1 && !isFirstHalf)
        {
            dad.FadeIsFin = true;
        }

        timer += Time.deltaTime / timeToFinishHalf;

        if (isFirstHalf && !dad.FadeIsFin)
        {
            hold = Vector4.Lerp(start, finish, timer);
            thisMat.SetColor("_Color", hold);
            hold = Vector4.one - hold;
            thatMat.SetColor("_Color", hold);
        } else if(!isFirstHalf && !dad.FadeIsFin)
        {
            hold = Vector4.Lerp(start, finish, timer);
            thisMat.SetColor("_Color2", hold);
            hold = Vector4.one - hold;
            thatMat.SetColor("_Color2", hold);
        }
    }
}
