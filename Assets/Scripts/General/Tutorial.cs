using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public VOScript voScript;
    public Image instructions;
    public Text[] objective;
    Color color;
    public float timer = 0f, opacity = 0f;
    public bool increaseOpacity = false, decreaseOpacity = false, instructionsFinished = true, 
                showObjective = false, objectiveShown = false, timing = false, secondObjective = false;
    public float holdDur = 2, waitDur = 1;

    private void Start()
    {
        if(PlayerPrefs.GetInt("failedAttempts") == 0)
        {
            objective[2].text = "Objective:\nDo what the voice says, he seems trustworthy...";
            instructionsFinished = false;
            increaseOpacity = true;
            timing = true;
        }
        else
        {
            if (PlayerPrefs.GetInt("failedAttempts") == 1)
            {
                objective[2].text = "Objective:\nThe voice lied! figure out how to get through the portal on your own";
                secondObjective = true;
            }
            instructionsFinished = true;
            increaseOpacity = false;
            timing = false;
        }
    }

    private void Update()
    {
        if (timing)
            timer += Time.deltaTime;

        if (timer < 1 && increaseOpacity && !instructionsFinished)
        {
            opacity = Mathf.Lerp(0f, 1f, timer);
        }
        else if (increaseOpacity && !instructionsFinished)
        {
            increaseOpacity = false;
            timer = 0f;
        }
        if(timer >= holdDur && !increaseOpacity && !decreaseOpacity && !instructionsFinished)
        {
            decreaseOpacity = true;
            timer = 0f;
        }
        if(timer < 1 && decreaseOpacity && !instructionsFinished)
        {
            opacity = Mathf.Lerp(1f, 0f, timer);
        }
        else if (decreaseOpacity && !instructionsFinished)
        {
            instructions.enabled = false;
            decreaseOpacity = false;
            instructionsFinished = true;
            timing = false;
            timer = 0f;
            opacity = 0;
        }

        if(instructionsFinished && !voScript.NotComplete() && (voScript.wasTutorial || secondObjective))
        {
            if((voScript.audioSource.time + 4.0f) >= voScript.audioSource.clip.length)
            {
                timing = true;
                showObjective = true;
            }
        }

        if (timer >= waitDur && !increaseOpacity && !decreaseOpacity && showObjective && !objectiveShown)
        { 
            increaseOpacity = true;
            objectiveShown = true;
            timer = 0f;
        }
        if (timer < 1 && increaseOpacity && showObjective)
        {
            opacity = Mathf.Lerp(0f, 1f, timer);
        }
        else if (increaseOpacity && showObjective)
        {
            increaseOpacity = false;
            timer = 0f;
        }
        if (timer >= holdDur && !increaseOpacity && !decreaseOpacity && showObjective && objectiveShown)
        {
            decreaseOpacity = true;
            timer = 0f;
        }
        if (timer < 1 && decreaseOpacity && showObjective)
        {
            opacity = Mathf.Lerp(1f, 0f, timer);
        }
        else if (decreaseOpacity && showObjective)
        {

            foreach (Text t in objective)
                t.enabled = false;

            decreaseOpacity = false;
            showObjective = false;
            timing = false;
            timer = 0f;
            opacity = 0;
        }


        if (!instructionsFinished)
        {
            color = instructions.color;
            color.a = opacity;
            instructions.color = color;
        }
        else
        {
            foreach(Text t in objective)
            {
                color = t.color;
                color.a = opacity;
                t.color = color;
            }
        }
    }
}
