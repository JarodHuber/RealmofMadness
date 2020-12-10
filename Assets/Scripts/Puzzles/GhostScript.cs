using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostScript : MonoBehaviour
{
    public PuzzleGeneration puzzleManager;
    public Transform target;
    NavMeshAgent agent;
    public int ghostNum;
    public Vector3 islandCenter;
    public bool setPos;
    public enum ghostType { normal, glitchy };
    public ghostType type;
    public float maxDist = 25, minSpeed = 2, maxSpeed = 8;
    public GameObject ghostAudioFab;
    float greatestDist, curDist, speed = 4;

    NavMeshObstacle lolz;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        puzzleManager.puzzleSolved = true;

        if (setPos)
        {
            SetPos();
            setPos = false;
            type = (ghostType)puzzleManager.methodLibrary.SkewedNum((int)ghostType.normal, 90, (int)ghostType.glitchy, 10);
            PlayerPrefs.SetInt("type" + ghostNum, (int)type);
            if(Mathf.Abs(Vector3.Distance(target.position, transform.position)) <= 5)
            {
                setPos = true;
                Start();
            }
        }
        else
        {

            string vector = PlayerPrefs.GetString("pos" + ghostNum);
            print(vector);
            float[] wispPoses = puzzleManager.methodLibrary.ParseVector3(vector);
            transform.position = new Vector3(wispPoses[0], wispPoses[1], wispPoses[2]);
            type = (ghostType)PlayerPrefs.GetInt("type" + ghostNum);
        }

        if (type == ghostType.glitchy)
        {
            lolz = gameObject.AddComponent<NavMeshObstacle>();
            lolz.shape = NavMeshObstacleShape.Capsule;
            lolz.center = new Vector3(0, -1.73f, 0);
            lolz.carving = true;
            lolz.carveOnlyStationary = false;
        }
    }

    private void Update()
    {
        agent.SetDestination(target.position);
        curDist = Vector3.Distance(transform.position, target.position);
        if (type == 0 && Vector3.Distance(transform.position, target.position) <= 1f)
        {
            puzzleManager.puzzleSolved = false;
            if (puzzleManager.failedAttempts.number == 0 && puzzleManager.voScript.audioSource.isPlaying && puzzleManager.voScript.audioSource.clip == puzzleManager.voScript.ghostLine)
                puzzleManager.voScript.CutOff();
            Instantiate(ghostAudioFab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        if(curDist> greatestDist)
        {
            greatestDist = curDist;
            speed = Mathf.Lerp(minSpeed, maxSpeed, curDist / maxDist);
        }
        agent.speed = speed;
    }

    void SetPos()
    {
        Vector3 pos;
        int negUsez = 1, negUsex = 1;
        if (puzzleManager.methodLibrary.SkewedNum(0, 50, 1, 50) == 0)
            negUsez = -1;
        else
            negUsez = 1;
        if (puzzleManager.methodLibrary.SkewedNum(0, 50, 1, 50) == 0)
            negUsex = -1;
        else
            negUsex = 1;
        pos = new Vector3(islandCenter.x + (negUsex * puzzleManager.methodLibrary.RandomNum(20)), transform.position.y,
                          islandCenter.z + (negUsez * puzzleManager.methodLibrary.RandomNum(20)));

        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(target.position, path);
        if (path.status == NavMeshPathStatus.PathPartial || Vector3.Distance(pos, target.position) <=7)
        {
            SetPos();
        }

        string posOfGhost = pos.x + "\t" + pos.y + "\t" + pos.z;
        PlayerPrefs.SetString("pos" + ghostNum, posOfGhost);
        transform.position = pos;
    }
}