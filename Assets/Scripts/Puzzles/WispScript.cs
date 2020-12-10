using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WispScript : MonoBehaviour
{
    public PuzzleGeneration puzzleManager;
    public NavMeshAgent positionCheck;
    Light wispColor;
    public GameObject player;
    public Vector3 islandCenter;
    public int wispNum = 0;
    public bool setPos = false;
    float wispYPos = 0f;

    public Color colorZero, colorOne, colorTwo, colorThree;
    public enum wispType { Yellow, Blue, Orange, Red};
    public wispType type;

    public bool wispLock = false, lerpToNewPos = false;
    public int chance;
    public Vector3 nextPos, iniPos;
    public float timer = 0, dur = .5f;
    public bool reset = true;


    public GameObject wispSucessAudioFab, wispFailAudioFab;

    private void Start()
    {
        wispColor = GetComponent<Light>();
        if (setPos)
        {
            SetPos();
            setPos = false;
            chance = puzzleManager.methodLibrary.SkewedNum(0, 70, 1, 30);
            PlayerPrefs.SetInt("chance" + wispNum, chance);
            type = (wispType)puzzleManager.methodLibrary.SkewedNum((int)wispType.Yellow, 65, (int)wispType.Orange, 20, (int)wispType.Blue, 10, (int)wispType.Red, 5);
            PlayerPrefs.SetInt("type" + wispNum, (int)type);
            for (int counter = 0; counter < puzzleManager.wispPoses.ToArray().Length; counter++)
            {
                if (Mathf.Abs(Vector3.Distance(puzzleManager.wispPoses[counter].position, transform.position)) <= 0.5f || Mathf.Abs(Vector3.Distance(player.transform.position, transform.position)) <= 2)
                {
                    setPos = true;
                    Start();
                }
            }
        }
        else
        {
            string vector = PlayerPrefs.GetString("pos" + wispNum);
            float[] wispPoses = puzzleManager.methodLibrary.ParseVector3(vector);
            transform.position = new Vector3(wispPoses[0], wispPoses[1], wispPoses[2]);
            type = (wispType)PlayerPrefs.GetInt("chance" + wispNum);
            type = (wispType)PlayerPrefs.GetInt("type" + wispNum);
        }
        
        if (type == wispType.Yellow)
            wispColor.color = colorZero;
        else if (type == wispType.Orange)
            wispColor.color = colorOne;
        else if (type == wispType.Blue)
            wispColor.color = colorTwo;
        else if (type == wispType.Red)
        {
            wispColor.color = colorThree;
            puzzleManager.difficulty--;
        }

        puzzleManager.wispPoses.Add(this.transform);
    }

    private void Update()
    {
        if (reset)
        {
            foreach (Transform g in puzzleManager.wispPoses)
            {
                reset = false;
                if ((Mathf.Abs(Vector3.Distance(g.transform.position, transform.position)) <= 0.5f || Mathf.Abs(Vector3.Distance(player.transform.position, transform.position)) <= 2) && g != gameObject)
                {
                    reset = true;
                    puzzleManager.wispPoses.Remove(transform);
                    break;
                }
            }
        }
        if (reset)
        {
            reset = false;
            Start();
        }

        if (Vector3.Distance(player.transform.position, transform.position) <= 1)
        {
            if (type == wispType.Blue)
            {
                foreach(WispScript w in puzzleManager.wisps)
                {
                    if (w.type == wispType.Yellow || w.type == wispType.Orange)
                    {
                        //puzzle failed due to out of order wisp pickup
                        puzzleManager.blueCollected = true;
                        puzzleManager.puzzleLocked = true;
                    }
                }
                puzzleManager.wispsCollected++;
                puzzleManager.wisps.Remove(this);
                Destroy(gameObject);
            }
            else if(type == wispType.Red)
            {
                //puzzle failed due to red wisp
                puzzleManager.redCollected = true;
                puzzleManager.puzzleLocked = true;
                Destroy(gameObject);
            }
            else if (type == wispType.Yellow || (type == wispType.Orange && !lerpToNewPos))
            {
                puzzleManager.wispsCollected++;
                puzzleManager.wisps.Remove(this);
                Destroy(gameObject);
            }

            if (((type == wispType.Blue) || (type == wispType.Red)) && puzzleManager.puzzleLocked)
                Instantiate(wispFailAudioFab, transform.position, Quaternion.identity);
            else
                Instantiate(wispSucessAudioFab, transform.position, Quaternion.identity);
        }

        if (type == wispType.Orange)
        {
            if (Vector3.Distance(player.transform.position, transform.position) <= 5 && !wispLock)
            {
                if (chance == 1)
                {
                    lerpToNewPos = true;
                    iniPos = transform.position;
                    SetPos();
                    wispLock = true;
                }
                else
                    wispLock = true;
            }

            if (lerpToNewPos)
            {
                timer += Time.deltaTime;
                timer = Mathf.Clamp(timer / dur, 0, 1);
                transform.position = Vector3.Lerp(iniPos, nextPos, timer);
                if(timer >= 1)
                {
                    lerpToNewPos = false;
                    timer = 0;
                }
            }
        }
    }

    public void SetPos()
    {
        int negUsez = 1, negUsex = 1;
        if (puzzleManager.methodLibrary.RandomNum(100) % 2 == 0)
            negUsez = -1;
        else
            negUsez = 1;
        if (puzzleManager.methodLibrary.RandomNum(100) % 2 == 0)
            negUsex = -1;
        else
            negUsex = 1;

        if (lerpToNewPos)
            nextPos = new Vector3(islandCenter.x + (negUsex * puzzleManager.methodLibrary.RandomNum(20)), wispYPos,
                                  islandCenter.z + (negUsez * puzzleManager.methodLibrary.RandomNum(20)));
        else
            transform.position = new Vector3(islandCenter.x + (negUsex * puzzleManager.methodLibrary.RandomNum(20)), wispYPos,
                                             islandCenter.z + (negUsez * puzzleManager.methodLibrary.RandomNum(20)));

        positionCheck.enabled = true;
        NavMeshPath path = new NavMeshPath();
        positionCheck.CalculatePath(transform.position, path);
        if (path.status == NavMeshPathStatus.PathPartial || Vector3.Distance(transform.position, player.transform.position)<=2f)
            SetPos();
        positionCheck.enabled = false;

        string posOfWisp = transform.position.x + "\t" + wispYPos + "\t" + transform.position.z;
        PlayerPrefs.SetString("pos" + wispNum, posOfWisp);
    }
}
