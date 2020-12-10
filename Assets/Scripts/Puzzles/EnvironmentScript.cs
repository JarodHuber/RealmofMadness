using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnvironmentScript : MonoBehaviour
{
    public PuzzleGeneration puzzleManager;
    public bool isPuzzle;
    public Vector3 islandCenter;
    public Transform player;
    public AudioClip step1, step2;

    Vector3 target;
    NavMeshAgent agent;
    AudioSource audioSource;
    Animator anim;
    List<Vector3> anchors = new List<Vector3>();
    float anchorDegree = 0;
    bool running = false, shaking = true;
    bool setPos = false;
    float[] anchorPoses = new float[3];

    public GameObject poof;

    private void Start()
    {
        GetComponent<NavMeshObstacle>().enabled = false;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        agent.speed = 4f;

        if (setPos)
        {
            while (anchors.Count < 6)
            {
                anchorPoses = puzzleManager.methodLibrary.ParseVector3(PlayerPrefs.GetString("pos" + anchors.Count));
                anchors.Add(new Vector3(anchorPoses[0], anchorPoses[1], anchorPoses[2]));
            }
        }
        else
            while (anchors.Count < 6)
                SetPos();
    }

    private void Update()
    {
        if (!audioSource.isPlaying && running && !shaking)
        {
            if (audioSource.clip == step1)
                audioSource.clip = step2;
            else
                audioSource.clip = step1;
            audioSource.Play();
        }
        else if (!running)
            audioSource.Stop();

        if (Vector3.Distance(transform.position, player.position) <= 5 && !running)
        {
            foreach (Vector3 a in anchors)
            {
                if(Vector3.Distance(transform.position, a) <= Vector3.Distance(player.position, a) && 
                   Mathf.Abs(Vector3.Distance(transform.position, a) - Vector3.Distance(player.position, a)) > anchorDegree &&
                   Vector3.Distance(transform.position, a) > 1f)
                {
                    anchorDegree = Vector3.Distance(transform.position, a) - Vector3.Distance(player.position, a);
                    target = a;
                    shaking = false;
                }
            }
            running = true;
        }

        if(running && shaking)
        {
            anim.SetBool("Shaking", true);
            anim.SetBool("Run", false);
            if (Vector3.Distance(transform.position, player.position) > 5)
            {
                anim.SetBool("Shaking", false);
                running = false;
                shaking = false;
            }
        }
        else if (running)
        {
            anim.SetBool("Shaking", false);
            anim.SetBool("Run", true);
            agent.SetDestination(target);
            agent.isStopped = false;
        }

        if(running && Vector3.Distance(transform.position, target) < 1)
        {
            if(Vector3.Distance(player.position, transform.position) > 5)
            {
                anim.SetBool("Run", false);
            }
            agent.isStopped = true;
            anchorDegree = 0;
            shaking = true;
            running = false;
        }
    }

    public void Select()
    {
        if (isPuzzle)
        {
            puzzleManager.enviroList.Remove(this);
            if (puzzleManager.enviroList.Count <= 0)
                puzzleManager.puzzleSolved = true;
            Instantiate(poof, transform.position, poof.transform.rotation);
            Destroy(gameObject);
        }
    }

    public void SetPos()
    {
        Vector3 pos;
        int negUsez = 1, negUsex = 1;
        if (puzzleManager.methodLibrary.SkewedNum(0,50,1,50) == 0)
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
        agent.CalculatePath(pos, path);
        if (path.status == NavMeshPathStatus.PathPartial)
            SetPos();

        string posOfWaypoint = pos.x + "\t" + islandCenter.y + "\t" + pos.z;
        PlayerPrefs.SetString("pos" + anchors.Count, posOfWaypoint);

        anchors.Add(pos);
        
    }
}
