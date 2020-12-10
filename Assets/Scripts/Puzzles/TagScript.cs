using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class TagScript : MonoBehaviour
{
    public PuzzleGeneration puzzleManager;
    public bool isPuzzle;
    public Vector3 islandCenter;
    public Transform player;
    public Transform tagger;
    public GameObject targetG;
    public Vector3 target;
    public bool running = false, shaking = true, taggable = true;
    public AudioClip step1, step2, tagSFX;

    NavMeshAgent agent;
    AudioSource audioSource1, audioSource2;
    MeshRenderer mesh;
    Animator anim;
    List<Vector3> anchors = new List<Vector3>();
    float currentDistanceToTarget, distanceToPlayer;

    private void Start()
    {
        GetComponent<NavMeshObstacle>().enabled = false;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        mesh = GetComponentInChildren<MeshRenderer>();
        audioSource1 = GetComponent<AudioSource>();
        audioSource2 = transform.GetChild(0).GetComponent<AudioSource>();
        agent.speed = 4f;
        anim.SetBool("Run", true);
        audioSource2.clip = tagSFX;
    }

    private void Update()
    {
        if (!audioSource1.isPlaying)
        {
            if (audioSource1.clip == step1)
                audioSource1.clip = step2;
            else
                audioSource1.clip = step1;
            audioSource1.Play();
        }

        tagger = puzzleManager.Tagger.transform;
        if (gameObject.tag == "Tagger")
        {
            print(gameObject.name);
            mesh.material.color = new Color(0.8301f, 0.2388f, 0.2388f, 1.0f);
            agent.speed = 7;
            currentDistanceToTarget = float.MaxValue;
            distanceToPlayer = Mathf.Abs(Vector3.Distance(transform.position, player.position)) + 10;
            if (player.gameObject.GetComponent<RigidBodyMovement>().taggable)
            {
                targetG = player.gameObject;
                currentDistanceToTarget = Mathf.Abs(Vector3.Distance(transform.position, targetG.transform.position)) - 5;
            }
            SetTarget();
            agent.SetDestination(targetG.transform.position);
            if (Mathf.Abs(Vector3.Distance(targetG.transform.position, transform.position)) <= 2)
                if (targetG.GetComponent<RigidBodyMovement>() != null && targetG.tag == "Tagee")
                    tagPlayer(targetG);
                else if (targetG.GetComponent<TagScript>() != null && targetG.tag == "Tagee")
                    tagParticipant(targetG);
        }

        else if (gameObject.tag == "Tagee")
        {
            agent.speed = 4;
            mesh.material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            if (!running)
            {
                target = newTarget();
                if (Mathf.Abs(Vector3.Distance(target, tagger.position)) < 3)
                    target *= -1;
                running = true;
            }
            else if (running)
            {
                currentDistanceToTarget = Mathf.Abs(Vector3.Distance(transform.position, target));
                if (currentDistanceToTarget <= 3)
                    running = false;
            }
            agent.SetDestination(target);
        }
    }

    void SetTarget()
    {
        foreach (Transform t in puzzleManager.participants)
        {
            if (t.gameObject.tag == "Tagee")
            {
                float distanceToParticipantCheck = Mathf.Abs(Vector3.Distance(transform.position, t.position));
                if (currentDistanceToTarget > distanceToParticipantCheck && t.gameObject.GetComponent<TagScript>().taggable)
                {
                    targetG = t.gameObject;
                    currentDistanceToTarget = distanceToParticipantCheck;
                }
                else if (currentDistanceToTarget <= distanceToParticipantCheck && !t.gameObject.GetComponent<TagScript>().taggable)
                    t.gameObject.GetComponent<TagScript>().taggable = true;

                if (distanceToPlayer < currentDistanceToTarget)
                    player.gameObject.GetComponent<RigidBodyMovement>().taggable = true;
            }
        }
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(target, path);
        if (path.status == NavMeshPathStatus.PathPartial || targetG == null)
            SetTarget();
    }

    void tagPlayer(GameObject player)
    {
        audioSource2.Play();
        this.taggable = true;
        player.GetComponent<RigidBodyMovement>().taggable = false;
        player.tag = "Tagger";
        this.gameObject.tag = "Tagee";
        this.targetG = null;
    }

    void tagParticipant(GameObject tagee)
    {
        audioSource2.Play();
        TagScript hold = tagee.GetComponent<TagScript>();
        if(hold != null)
        {
            this.taggable = false;
            hold.taggable = true;
            tagee.tag = "Tagger";
            this.gameObject.tag = "Tagee";
            this.targetG = null;
        }
    }
    public void Select()
    {
        if (isPuzzle)
        {
            if (player.tag == "Tagger")
            {
                player.tag = "Tagee";
                gameObject.tag = "Tagger";
                this.taggable = false;
            }
        }
    }

    Vector3 newTarget()
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

        Vector3 target = new Vector3(islandCenter.x + (negUsex * puzzleManager.methodLibrary.RandomNum(20)), transform.position.y, 
                                     islandCenter.z + (negUsez * puzzleManager.methodLibrary.RandomNum(20)));

        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(target, path);

        if (path.status == NavMeshPathStatus.PathPartial)
            target = newTarget();

        return target;
    }

    #region Jarod'sAttempt
    public void SetAnchors()
    {
        anchors.Clear();
        while (anchors.Count < 6)
            SetPos();
    }

    public void Tag(GameObject theTagged)
    {
        if(theTagged != player.gameObject)
            theTagged.GetComponent<TagScript>().taggable = false;
        theTagged.tag = "Tagger";
        gameObject.tag = "Tagee";
        running = false;
        agent.speed = 3;
        foreach (TagScript t in puzzleManager.players)
        {
            t.running = false;
            //t.tagees.Remove(theTagged.transform);
            //t.tagees.Add(transform);
        }
    }

    public void SetPos()
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

        pos = new Vector3(islandCenter.x + (negUsex * puzzleManager.methodLibrary.RandomNum(15)), transform.position.y,
                                         islandCenter.z + (negUsez * puzzleManager.methodLibrary.RandomNum(15)));

        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(pos, path);
        if (path.status == NavMeshPathStatus.PathPartial)
        {
            SetPos();
        }

        string posOfWaypoint = pos.x + "\t" + islandCenter.y + "\t" + pos.z;
        PlayerPrefs.SetString("pos" + anchors.Count, posOfWaypoint);

        anchors.Add(pos);

    }
    #endregion
}
