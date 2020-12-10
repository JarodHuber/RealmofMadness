using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VOScript : MonoBehaviour
{
    public enum PlayType { Fail, Success, NextLine }

    [HideInInspector]
    public bool tutorialStart = false, wasTutorial = false, reset = false;
    [HideInInspector]
    public int lineCount;

    public AudioSource audioSource;
    [Header("Main Lines")]
    public AudioClip tutorialLine1;
    public AudioClip tutorialLine2, tutorialLineSuccess, tutorialLineTrueSuccess, ghostLine, ghostLine2, backwardsLine, enviroLine, wispLine, tagLine;
    [Header("Fail Lines")]
    public List<AudioClip> tutorialFail;
    public List<AudioClip> ghostFail, backwardsFail, enviroFail, wispFail, tagFail, generalFail;
    [Header("End Lines")]
    public List<AudioClip> endLines;
    public AudioClip endSuccess;

    List<AudioClip> LineIndex = new List<AudioClip>();
    List<List<AudioClip>> failIndex = new List<List<AudioClip>>();
    public List<AudioClip> lines = new List<AudioClip>(), failLines = new List<AudioClip>();

    bool newLine = false, soundLock = false;
    int lineMarker = 0;

    public List<int> voFailVals = new List<int>();

    private void Awake()
    {
        LineIndex.Add(tutorialLine2);
        LineIndex.Add(ghostLine);
        LineIndex.Add(backwardsLine);
        LineIndex.Add(enviroLine);
        LineIndex.Add(wispLine);
        LineIndex.Add(tagLine);

        failIndex.Add(tutorialFail);
        failIndex.Add(ghostFail);
        failIndex.Add(backwardsFail);
        failIndex.Add(enviroFail);
        failIndex.Add(wispFail);
        failIndex.Add(tagFail);
    }

    private void Update()
    {
        if (tutorialStart)
        {
            if (Input.anyKeyDown)
            {
                Play();
                wasTutorial = true;
                tutorialStart = false;
            }
        }

        if (!audioSource.isPlaying && !soundLock)
        {
            if (newLine)
            {
                if(NotComplete())
                {
                    Play(PlayType.NextLine);
                }
                newLine = false;
            }
        }
        else
        {
            newLine = true;
        }
    }

    public void OverideLine(bool a = true, bool b = false)
    {
        if (a)
            lines.Add(tutorialLine1);
        else if (b)
            lines.Add(tutorialLineTrueSuccess);
        else
            lines.Add(tutorialLineSuccess);
    }

    public void SetLines(int puzzleNum)
    {
        lines.Add(LineIndex[puzzleNum]);
    }

    public bool SetFail(int puzzleNum, int attemptNum, bool forceGeneral = false)
    {
        if (!forceGeneral)
        {
            if ((attemptNum - 1) < failIndex[puzzleNum].Count || failIndex[puzzleNum] == wispFail)
            {
                failLines = failIndex[puzzleNum];
                lineCount = failLines.Count - 1;
                return true;
            }
            else
            {
                failLines = generalFail;
                lineCount = 0;
                return false;
            }
        }
        else
        {
            failLines = generalFail;
            lineCount = 0;
            return false;
        }
    }

    public void Play(PlayType type = PlayType.Success, int failLine = 0)
    {
        if(type == PlayType.Success)
        {
            audioSource.clip = lines[0];
        }
        else if(type == PlayType.NextLine)
        {
            int x = 0;
            while (lines[x] != audioSource.clip)
                x++;
            lineMarker = x + 1;
            audioSource.clip = lines[lineMarker];
        }
        else if(type == PlayType.Fail)
        {
            if(failLines.Count != 0)
            {
                if (failLine >= failLines.Count)
                {
                    SetFail(0, 0, true);
                    if (failLine >= failLines.Count || failLine < 0)
                        failLine = 0;
                    if (voFailVals[failLine] == 0) 
                    {
                        if (voFailVals.Contains(1))
                            failLine = voFailVals.BinarySearch(1);
                        else
                            for (int i = 0; i < 10; i++)
                                voFailVals[i] = 1;
                    }

                    voFailVals[failLine] = 0;
                }
                audioSource.clip = failLines[failLine];
            }
            else
                Debug.LogError("you haven't properly set the fail line ~Jarod");
        }

        audioSource.Play();
    }

    public void CutOff(bool ghostCutoff = true)
    {
        if (ghostCutoff)
            audioSource.clip = ghostLine2;
        else
            audioSource.clip = endSuccess;

        lineMarker = lines.Count;

        audioSource.Play();
    }

    public void End()
    {
        soundLock = true;
        lines = endLines;
        Play();
    }
    public bool NotComplete()
    {
        return (lineMarker + 1) < lines.Count;
    }
}
