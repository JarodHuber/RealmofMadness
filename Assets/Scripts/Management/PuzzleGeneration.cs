using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class PuzzleGeneration : MonoBehaviour
{
    #region Variables
    enum status { Failed, Solved };
    public enum puzzle { None, BehindPuzzle, GhostPuzzle, BackwardsPuzzle, EnvironmentPuzzle, WispPuzzle, TagPuzzle };

    public puzzle type;

    [Header("Seed Varibles")]
    public MethodLibrary methodLibrary;
    public int[] seedValues = new int[14], voSeedVals = new int[7];
    public static int MaxDifficulty = 50;
    public int currDifficulty, difficulty = 0;

    int[] chances = new int[6];

    [Header("Necessary External Objects")]
    public LightingManager lightingManager;
    public HallucinationManager hallucinationManager;
    public GameObject player;
    public VOScript voScript;
    public MushroomPosSet mushPos;
    public Portal portal;
    public BackwardsScript backwardsScript;

    [Header("Lists")]
    public List<EnvironmentScript> enviroList = new List<EnvironmentScript>();
    public List<WispScript> wisps = new List<WispScript>();
    public List<Transform> objs = new List<Transform>();

    [Header("Center Marker")]
    public GameObject ground;
    public Vector3 islandCenter;

    [Header("General Puzzle variables")]
    public AudioClip step1;
    public AudioClip step2;
    public bool puzzleSolved;

    bool setPos = false;
    GameObject instance;

    #region ghost Vars
    [Header("Ghost Variables")]
    public GameObject ghostFab;

    GhostScript ghostScript;
    #endregion

    #region wisp Vars
    [Header("Wisp Variables")]
    public List<Transform> wispPoses = new List<Transform>();
    public bool puzzleLocked = false;
    public int wispsCollected = 0;
    public GameObject wispFab;

    WispScript wispScript;
    #endregion

    #region environment Vars
    [Header("Enviroment Variables")]
    public GameObject poof;

    GameObject[] environmentStuff;
    EnvironmentScript environmentScript;
    int laterObjs, mult, inc;
    #endregion

    #region tag Vars
    [HideInInspector]
    public GameObject Tagger;

    [Header("Tag Variables")]
    public List<Transform> participants = new List<Transform>();
    public List<TagScript> players = new List<TagScript>();
    public AudioClip tagSFX;

    TagScript tagScript;
    #endregion

    #region VO Vars
    [HideInInspector]
    public bool redCollected = false, blueCollected = false;

    int enviroCount;
    bool puzzleUniqueFail = false;
    #endregion

    [Header("Debug Tools")]
    public bool debug = false;
    public bool debugReset = false;

    [Header("UI")]
    public UINumber lvlNum;
    public UINumber failedAttempts;
    #endregion

    private void Awake()
    {
        seedValues = methodLibrary.ParseSeed(PlayerPrefs.GetString("Seed"));
        voSeedVals = methodLibrary.ParseSeed(PlayerPrefs.GetString("voSeed"));
        voScript.voFailVals.AddRange(methodLibrary.ParseSeed(PlayerPrefs.GetString("voFailSeed")));
        if(PlayerPrefs.GetString("hallucinationOrder") != "")
            hallucinationManager.order.AddRange(methodLibrary.ParseSeed(PlayerPrefs.GetString("hallucinationOrder")));
        if (PlayerPrefs.GetString("hallucinationTheta") != "")
            hallucinationManager.theta.AddRange(methodLibrary.ParseSeed(PlayerPrefs.GetString("hallucinationTheta")));
        if (PlayerPrefs.GetString("hallucinationRadius") != "")
            hallucinationManager.radius.AddRange(methodLibrary.ParseSeed(PlayerPrefs.GetString("hallucinationRadius")));


        if (PlayerPrefs.GetInt("Debug/Presentation") == 0)
            debug = false;
        else if (PlayerPrefs.GetInt("Debug/Presentation") == 1)
            debug = true;

        lightingManager.SetFog(seedValues[0]);

        if (PlayerPrefs.GetInt("puzzleSolved") == (int)status.Solved)
        {
            PlayerPrefs.SetInt("failedAttempts", 0);
            PlayerPrefs.SetInt("puzzleSolved", 0);
        }
        else if (PlayerPrefs.GetInt("puzzleSolved") == (int)status.Failed)
        {
            PlayerPrefs.SetInt("failedAttempts", PlayerPrefs.GetInt("failedAttempts") + 1);
            difficulty = PlayerPrefs.GetInt("difficulty");
        }

    }

    void Start()
    {
        environmentStuff = GameObject.FindGameObjectsWithTag("Environment");
        islandCenter = new Vector3(ground.transform.position.x, -1, ground.transform.position.z);
        for (int counter = 0; counter < environmentStuff.Length; counter++)
            objs.Add(environmentStuff[counter].transform);
        objs.Add(player.transform);
        objs.Add(portal.transform);

        definePuzzle(seedValues);
    }

    public void Update()
    {
        if (puzzleLocked || player.tag == "Tagger")
            puzzleSolved = false;
        else if (((type == puzzle.WispPuzzle) && wispsCollected >= difficulty) || ((type == puzzle.TagPuzzle) && player.tag != "Tagger"))
            puzzleSolved = true;

        if ((type == puzzle.TagPuzzle) && Tagger.tag != "Tagger")
            Tagger = GameObject.FindGameObjectWithTag("Tagger");

        if (debug)
            if (Input.GetKey(KeyCode.Alpha1))
                DebugReload(0);
            else if (Input.GetKey(KeyCode.Alpha2))
                DebugReload(1);
            else if (Input.GetKey(KeyCode.Alpha3))
                DebugReload(2);
            else if (Input.GetKey(KeyCode.Alpha4))
                DebugReload(3);
            else if (Input.GetKey(KeyCode.Alpha5))
                DebugReload(4);
            else if (Input.GetKey(KeyCode.Alpha6))
                DebugReload(5);
            else if (Input.GetKey(KeyCode.Alpha7))
                DebugReload(6);
    }

    private void DebugReload(int puzzle)
    {
        int lvlNum = methodLibrary.RandomNum(100);
        while(lvlNum == 2)
            lvlNum = methodLibrary.RandomNum(100);
        string seedH = methodLibrary.DecToHex(lvlNum)+ "r" + puzzle + "r0r0r2r20r32r32rar32r32r32r32";
        PlayerPrefs.SetString("Seed", seedH);
        PlayerPrefs.SetInt("puzzleSolved", 1);
        debugReset = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene(3);
    }

    private void OnDestroy()
    {
        if (puzzleSolved && !debugReset)
            puzzleCompleted();
        else if (!debugReset)
        {
            if(type == puzzle.BehindPuzzle || type == puzzle.BackwardsPuzzle)
            {
                voSeedVals[0] = PlayerPrefs.GetInt("failedAttempts");
                if (type == puzzle.BehindPuzzle && lvlNum.number != 1)
                    voSeedVals[0]++;
            }
            else if (type == puzzle.EnvironmentPuzzle)
            {
                if(enviroCount == enviroList.Count && voSeedVals[4] == 0)
                    voSeedVals[0] = 1;
                else
                    voSeedVals[0] = 0;
            }
            else if (type == puzzle.WispPuzzle)
            {
                if (wispsCollected < difficulty && redCollected && blueCollected)
                    voSeedVals[0] = 3;
                else if (wispsCollected < difficulty)
                    voSeedVals[0] = 0;
                else if (blueCollected)
                    voSeedVals[0] = 2;
                else if (redCollected) 
                    voSeedVals[0] = 1;
            }
        }
        PlayerPrefs.SetString("voSeed", methodLibrary.toSeed(voSeedVals));
        PlayerPrefs.SetString("voFailSeed", methodLibrary.toSeed(voScript.voFailVals));
        PlayerPrefs.SetString("hallucinationOrder", methodLibrary.toSeed(hallucinationManager.order));
        PlayerPrefs.SetString("hallucinationTheta", methodLibrary.toSeed(hallucinationManager.theta));
        PlayerPrefs.SetString("hallucinationRadius", methodLibrary.toSeed(hallucinationManager.radius));
    }

    public void puzzleCompleted()
    {
        float temp = PlayerPrefs.GetFloat("songPoint");

        voSeedVals[seedValues[1] + 1] = 1;
        voSeedVals[0] = 0;
        chances[0] = seedValues[6];
        chances[1] = seedValues[7];
        chances[2] = seedValues[8];
        chances[3] = seedValues[9];
        chances[4] = seedValues[10];
        chances[5] = seedValues[11];
        chanceAlter();

        PlayerPrefs.DeleteAll();

        PlayerPrefs.SetFloat("songPoint", temp);
        PlayerPrefs.SetString("Seed", methodLibrary.GenerateSeed(seedValues[0], MaxDifficulty, chances));
        PlayerPrefs.SetInt("puzzleSolved", 1);

        if(lvlNum.number == 1)
            PlayerPrefs.SetInt("overideBool", (failedAttempts.number == 0) ? 1 : 0);

        if (debug)
            PlayerPrefs.SetInt("Debug/Presentation", 1);
        else if (!debug)
            PlayerPrefs.SetInt("Debug/Presentation", 0);
    }

    public void chanceAlter()
    {
        for (int counter = 0; counter < chances.Length; counter++)
            if (counter != seedValues[1] && counter != 2 && counter != 0)
                chances[counter] += 10;
            else if (counter != seedValues[1])
                chances[counter] += 5;
            else
                chances[counter] -= 10;
    }

    public void definePuzzle(int[] seed)
    {
        currDifficulty = seed[5];
        if (seed[1] == 0)
        {
            /// backOfPortalPuzzle
            portal.ActivatePuzzle(0);
            type = puzzle.BehindPuzzle;
        }
        else if (seed[1] == 1)
        {
            /// ghostPuzzle
            ActivateGhostPuzzle();
            type = puzzle.GhostPuzzle;
        }
        else if (seed[1] == 2)
        {
            /// backwardsPuzzle
            ActivateBackwardsPuzzle();
            type = puzzle.BackwardsPuzzle;
        }
        else if (seed[1] == 3)
        {
            /// enviroPuzzle
            ActivateEnvironmentPuzzle(seed[4]);
            type = puzzle.EnvironmentPuzzle;
        }
        else if (seed[1] == 4)
        {
            /// wispPuzzle
            ActivateWispPuzzle();
            type = puzzle.WispPuzzle;
        }
        else if (seed[1] == 5)
        {
            /// tagPuzzle
            ActivateTagPuzzle(seed[4]);
            type = puzzle.TagPuzzle;
        }
        mushPos.SetMushroomPos(seed[1]);

        int attemptNum = PlayerPrefs.GetInt("failedAttempts");
        lvlNum.Number(seed[0]);
        failedAttempts.Number(attemptNum);
        puzzleUniqueFail = voScript.SetFail(seed[1], attemptNum);
        if(!puzzleUniqueFail)
        {
            if(!voScript.voFailVals.Contains(1))
                for (int x = 0; x < 10; x++)
                    voScript.voFailVals[x] = 1;

            voSeedVals[0] = methodLibrary.SkewedNum(0, voScript.voFailVals[0], 1, voScript.voFailVals[1], 2, voScript.voFailVals[2], 3, voScript.voFailVals[3], 4, voScript.voFailVals[4], 
                                                    5, voScript.voFailVals[5], 6, voScript.voFailVals[6], 7, voScript.voFailVals[7], 8, voScript.voFailVals[8], 9, voScript.voFailVals[9]);

            voScript.voFailVals[voSeedVals[0]] = 0;
        }

        if(attemptNum == 0)
        {
            if (seed[0] == 2)
                voScript.OverideLine(false, (PlayerPrefs.GetInt("overideBool")==1));
            if (voSeedVals[seed[1] + 1] == 0)
            {
                if (seed[1] == 0 && seed[0] == 1)
                    voScript.OverideLine();
                voScript.SetLines(seed[1]);

                if (seed[1] == 0)
                    voScript.tutorialStart = true;
                else
                    voScript.Play();
            }
        }
        else
        {
            voScript.Play(VOScript.PlayType.Fail, voSeedVals[0]);
        }
    }

    #region backwardsActivate
    public void ActivateBackwardsPuzzle()
    {
        backwardsScript.sg = this;
        backwardsScript.isPuzzle = true;
        backwardsScript.player = player;
        backwardsScript.portal = portal.gameObject;
    }
    #endregion

    #region wispActivate
    public void ActivateWispPuzzle()
    {
        if (PlayerPrefs.GetInt("failedAttempts") == 0)
        {
            setPos = true;
            difficulty = (Random.Range(1, currDifficulty) * 2) + 1;
            PlayerPrefs.SetInt("difficulty", difficulty);
        }
        else
        {
            difficulty = PlayerPrefs.GetInt("difficulty");
        }

        for (int counter = 0; counter < difficulty; counter++)
        {
            instance = Instantiate(wispFab);
            wispScript = instance.GetComponent<WispScript>();
            wisps.Add(wispScript);
            wispScript.puzzleManager = this;
            wispScript.positionCheck = GetComponent<NavMeshAgent>();
            GetComponent<NavMeshAgent>().enabled = false;
            wispScript.wispNum = counter;
            wispScript.player = player;
            wispScript.islandCenter = islandCenter;
            wispScript.setPos = setPos;
        }
    }
    #endregion

    #region ghostActivate
    public void ActivateGhostPuzzle()
    {
        if (PlayerPrefs.GetInt("failedAttempts") == 0)
        {
            setPos = true;
            difficulty = (methodLibrary.RandomNum(currDifficulty)) + 1;
            PlayerPrefs.SetInt("difficulty", difficulty);
        }
        else
        {
            difficulty = PlayerPrefs.GetInt("difficulty");
        }

        for (int counter = 0; counter < PlayerPrefs.GetInt("difficulty") + 1; counter++)
        {
            instance = Instantiate(ghostFab);
            ghostScript = instance.GetComponent<GhostScript>();
            ghostScript.puzzleManager = this;
            ghostScript.ghostNum = counter;
            ghostScript.islandCenter = islandCenter;
            ghostScript.setPos = setPos;
            ghostScript.target = player.transform;
        }
    }
    #endregion

    #region environmentActivate
    public void ActivateEnvironmentPuzzle(int objToMakeAlive)
    {
        if (PlayerPrefs.GetInt("failedAttempts") == 0)
        {
            setPos = true;
            difficulty = (methodLibrary.RandomNum(currDifficulty)) + 1;
            mult = methodLibrary.RandomNum(20);
            inc = methodLibrary.RandomNum(10);
            PlayerPrefs.SetInt("difficulty", difficulty);
            PlayerPrefs.SetInt("mult", mult);
            PlayerPrefs.SetInt("inc", inc);
        }
        else
        {
            difficulty = PlayerPrefs.GetInt("difficulty");
            mult = PlayerPrefs.GetInt("mult");
            inc = PlayerPrefs.GetInt("inc");
        }

        environmentScript = environmentStuff[objToMakeAlive].AddComponent<EnvironmentScript>();
        environmentScript.poof = poof;
        environmentScript.puzzleManager = this;
        environmentScript.isPuzzle = true;
        environmentScript.islandCenter = islandCenter;
        environmentScript.player = player.transform;
        environmentScript.step1 = step1;
        environmentScript.step2 = step2;
        laterObjs = objToMakeAlive;
        enviroList.Add(environmentScript);

        for (int n = 1; n < difficulty; n++)
        {
            laterObjs = methodLibrary.RandomNum(environmentStuff.Length, false, mult, inc);

            if (environmentStuff[laterObjs].GetComponent<EnvironmentScript>() == null)
            {
                environmentScript = environmentStuff[laterObjs].AddComponent<EnvironmentScript>();
                environmentScript.poof = poof;
                environmentScript.puzzleManager = this;
                environmentScript.isPuzzle = true;
                environmentScript.islandCenter = islandCenter;
                environmentScript.player = player.transform;
                environmentScript.step1 = step1;
                environmentScript.step2 = step2;
                enviroList.Add(environmentScript);
            }
        }

        enviroCount = enviroList.Count;
    }
    #endregion

    #region TagActivate
    /// <summary>
    /// Tag you're it
    /// </summary>
    /// <param name="iniTagger">Seed value [4] the 5th slot, lvlSpecific seed spot</param>
    public void ActivateTagPuzzle(int iniTagger)
    {
        for (int counter = 0; counter <environmentStuff.Length; counter++)
        {
            environmentStuff[counter].tag = "Tagee";
            tagScript = environmentStuff[counter].AddComponent<TagScript>();
            tagScript.puzzleManager = this;
            tagScript.isPuzzle = true;
            tagScript.islandCenter = islandCenter;
            tagScript.player = player.transform;
            tagScript.step1 = step1;
            tagScript.step2 = step2;
            tagScript.tagSFX = tagSFX;
            players.Add(tagScript);
            participants.Add(environmentStuff[counter].transform);
        }

        if (iniTagger >= environmentStuff.Length)
        {
            Tagger = player;
            player.tag = "Tagger";
        }
        else
        {
            player.tag = "Tagee";
            environmentStuff[iniTagger].tag = "Tagger";
            Tagger = environmentStuff[iniTagger];
        }
    }
    #endregion
}