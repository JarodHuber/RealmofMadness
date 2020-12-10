using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class FancyCreds : MonoBehaviour
{
    public enum state { Next, Stay, Prev, Transition };

    [HideInInspector]
    public bool inputAcceptable = true, doinSomthin = false;
    [HideInInspector]
    public int lastIndex = 0;

    public int index = 0;

    [Space(10)]
    public state currState = state.Stay;

    [Space(10)]
    public GameObject cammy;

    [Space(10)]
    public List<Sprite> Credits = new List<Sprite>();

    [Header("Ghost Variables")]
    public Vector3[] ghostSpawnLoc;
    public Sprite ghost;
    ghostSpawner ghostSpawn;
    public float spawnLength = 5;
    public int ghostsPerTick = 10;
    public float spawnRangeLarge = 100;
    public float spawnRangeSmall = 100;
    public float timeToGoAcross = 3;
    public List<GameObject> spawners = new List<GameObject>();
    public List<GameObject> ghosts = new List<GameObject>();
    public bool presetGhostSpawn = false;
    public float ghostSpawnersToSet = 0;
    public bool switcher = false, allDone = false, credSet = false;

    [Header("Fade Variables")]
    public GameObject Tree;
    public Material thisMat;
    public Material thatMat;
    public float timeToFinishFade;
    ShaderAlphaChanger sac;
    public bool isFade = false;
    public bool FadeIsFin = false;

    [Header("Mushroom Growth Variables")]
    public float minTime;
    public float maxTime;
    public GameObject Mushroom;
    public List<GameObject> gam = new List<GameObject>();
    public List<Vector3> vec = new List<Vector3>();
    public List<Populate> mush = new List<Populate>();

    [Header("Portal Variables")]
    public GameObject portal;
    public float portalAnimTime = 5;
    public float startScaleMultiplier, endScaleMultiplier;

    [HideInInspector]
    public bool portalHiding = false, portalPast = false;

    [Header("Jump And Flip Variables")]
    public float jumpHeight;
    public float timeToJumpUp;
    public float timeToFlip;
    public JumpFlip jumperFlipper;

    [HideInInspector]
    GameObject instance;
    public SpriteRenderer sprRender;
    bool playAnim = false;
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        thisMat.SetColor("_Color", Vector4.one);
        thisMat.SetColor("_Color2", Vector4.one);
        thatMat.SetColor("_Color", Vector4.one);
        thatMat.SetColor("_Color2", Vector4.one);
        sprRender = GetComponent<SpriteRenderer>();
        sprRender.sprite = Credits[index];
        sprRender.material = thisMat;
        if (!presetGhostSpawn || ghostSpawnersToSet > ghostSpawnLoc.Length)
            ghostSpawnersToSet = ghostSpawnLoc.Length;
    }

    // Update is called once per frame
    void Update()
    {
        if (playAnim)
        {
            inputAcceptable = false;
            doinSomthin = true;
            playAnim = false;
            if (index == 1)
            {
                instance = Instantiate(Mushroom,
                    new Vector3((int)Random.Range(-256, 256), (int)Random.Range(-192, 192), -10),
                    Quaternion.identity, transform);
                instance.GetComponent<Populate>().minTime = minTime;
                instance.GetComponent<Populate>().maxTime = maxTime;
            }
            if(index == 2)
            {
                for (int counter = 0; counter < ghostSpawnersToSet; counter++)
                {
                    print("ghost" + counter);
                    instance = new GameObject("Spawner" + counter);
                    instance.transform.position = ghostSpawnLoc[counter];
                    ghostSpawn = instance.AddComponent<ghostSpawner>();
                    ghostSpawn.timeLeftToSpawn = spawnLength;
                    ghostSpawn.amountPerTick = ghostsPerTick / (ghostSpawnersToSet/ghostSpawnLoc.Length);
                    ghostSpawn.spawnRangeL = spawnRangeLarge;
                    ghostSpawn.spawnRangeS = spawnRangeSmall;
                    ghostSpawn.timeToGoAcross = timeToGoAcross;
                    ghostSpawn.ghost = ghost;
                    ghostSpawn.dad = this;
                    spawners.Add(instance);
                }
                allDone = false;
                credSet = false;
            }
            if(index == 3)
            {
                instance = Instantiate(portal, Vector3.forward, Quaternion.identity, transform);
                instance.GetComponent<Fly>().dad = this;
                instance.GetComponent<Fly>().timeToGrow = portalAnimTime;
                instance.GetComponent<Fly>().startScaleMult = startScaleMultiplier;
                instance.GetComponent<Fly>().endScaleMult = endScaleMultiplier;
            }
            if(index == 4)
            {
                jumperFlipper = gameObject.AddComponent<JumpFlip>();
                jumperFlipper.jumpHeight = jumpHeight;
                jumperFlipper.timeToJumpUp = timeToJumpUp;
                jumperFlipper.timeToFlip = timeToFlip;
                jumperFlipper.dad = this;
            }
            if(index == 0)
            {
                print("Step1");
                instance = new GameObject("CreditToFade");
                instance.transform.parent = transform;
                sac = instance.AddComponent<ShaderAlphaChanger>();
                sac.dadRenderer = sprRender;
                sac.thisRenderer = instance.AddComponent<SpriteRenderer>();
                sac.dad = this;
                sac.thisMat = thatMat;
                sac.thatMat = thisMat;
                sac.timeToFinishAnim = timeToFinishFade;
                isFade = true;
                sac.thisRenderer.material = thatMat;
                sac.thisRenderer.sprite = Credits[lastIndex];
                sprRender.sprite = Credits[index];
            }
            print("anim" + index);
        }

        if (currState == state.Transition && index == 1 && mush.Count == 0 && gam.Count > 0)
        {
            print("Error");
            sprRender.sprite = Credits[index];
            foreach (GameObject mushroom in gam)
                Destroy(mushroom, Random.Range(0f, 1.5f));
            vec.Clear();
            gam.Clear();
            StartCoroutine(waitForAccept());
            doinSomthin = false;
            sprRender.sprite = Credits[index];
        }

        if(currState == state.Transition && index == 2 && !allDone)
        {
            switcher = true;
            foreach (GameObject go in spawners)
            {
                ghostSpawn = go.GetComponent<ghostSpawner>();
                if (ghostSpawn.timeLeftToSpawn > spawnLength - timeToGoAcross)
                {
                    switcher = false;
                    break;
                }
            }
            if (switcher)
            {
                if (!credSet)
                {
                    credSet = true;
                    sprRender.sprite = Credits[index];
                }
                allDone = true;
                foreach(GameObject go in spawners)
                {
                    if (!go.GetComponent<ghostSpawner>().isDie)
                    {
                        allDone = false;
                        break;
                    }
                }
            }
            if (allDone)
            {
                foreach(GameObject go in spawners)
                {
                    Destroy(go);
                }
            }
        }
        if (currState == state.Transition && index == 2 && ghosts.Count <= 0 && switcher)
        {
            print(ghosts.Count);
            inputAcceptable = true;
            switcher = false;
            allDone = false;
            doinSomthin = false;
            spawners.Clear();
            ghosts.Clear();
        }

        if (currState == state.Transition && index == 3 && portalHiding)
            sprRender.sprite = Credits[index];
        if (currState == state.Transition && index == 3 && portalPast)
        {
            Destroy(instance);
            portalHiding = false;
            portalPast = false;
            inputAcceptable = true;
            doinSomthin = false;
        }

        if (currState == state.Transition && index == 0 && isFade && FadeIsFin)
        {
            print("pass");
            inputAcceptable = true;
            isFade = false;
            FadeIsFin = false;
            doinSomthin = false;
            Destroy(instance);
        }

        lastIndex = index;

        if (inputAcceptable && Input.GetAxis("Horizontal") > 0 && currState == state.Stay && !doinSomthin)
        {
            currState = state.Next;
            nextSlide();
        } else if (inputAcceptable && Input.GetAxis("Horizontal") < 0 && currState == state.Stay && !doinSomthin)
        {
            currState = state.Prev;
            prevSlide();
        } else if (inputAcceptable && currState != state.Stay && !doinSomthin)
        {
            currState = state.Stay;
        }

        if (!inputAcceptable && currState != state.Transition && doinSomthin)
        {
            currState = state.Transition;
        }
    }

    void nextSlide()
    {
        print(inputAcceptable);
        index++;
        if(index >= Credits.Count)
            index = 0;
        inputAcceptable = false;
        playAnim = true;
        doinSomthin = true;
        print(inputAcceptable);
    }
    void prevSlide()
    {
        print(inputAcceptable);
        index--;
        if (index < 0)
            index = Credits.Count-1;
        inputAcceptable = false;
        playAnim = true;
        doinSomthin = true;
        print(inputAcceptable);
    }
    public IEnumerator waitForAccept()
    {
        yield return new WaitForSeconds(1.5f);
        inputAcceptable = true;
    }
}