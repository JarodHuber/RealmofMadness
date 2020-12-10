using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallucinationManager : MonoBehaviour
{
    [HideInInspector]
    public List<int> order = new List<int>(), theta = new List<int>(), radius = new List<int>();
    [HideInInspector]
    public GameObject instance;
    [HideInInspector]
    public int difficulty;
    [HideInInspector]
    public float speed;
    [HideInInspector]
    public Vector3 place;

    public PuzzleGeneration manager;
    public List<GameObject> hallucinations = new List<GameObject>();
    public int maxHallucinations = 25;

    HallucinationSpinner hs;

    void Start()
    {
        difficulty = Mathf.Clamp(manager.currDifficulty, 0, maxHallucinations);
        if (PlayerPrefs.GetInt("FailedAttempts") >= 1)
            notNewPuzzle();
        else newPuzzle();
    }

    void notNewPuzzle()
    {
        for (int counter = 0; counter < difficulty; counter++)
        {
            instance = Instantiate(hallucinations[order[counter]], transform);
            place.x = radius[counter] * Mathf.Cos(Mathf.Deg2Rad * theta[counter]);
            place.z = radius[counter] * Mathf.Sin(Mathf.Deg2Rad * theta[counter]);
            hs = instance.GetComponent<HallucinationSpinner>();
            hs.speed = Random.Range(1, 10);
            hs.center = transform.position;
            hs.player = manager.player.transform;
        }
    }

    void newPuzzle()
    {
        order.Clear();
        radius.Clear();
        theta.Clear();
        for (int counter = 0; counter < difficulty; counter++)
        {
            order.Add(Random.Range(0, 3));
            instance = Instantiate(hallucinations[order[counter]], transform);
            radius.Add(Random.Range(40, 51));
            theta.Add(Random.Range(0, 361));
            place.x = radius[counter] * Mathf.Cos(Mathf.Deg2Rad * theta[counter]);
            place.y = Mathf.Abs(radius[counter] * Mathf.Sin(Mathf.Deg2Rad * theta[counter]))-2;
            place.z = radius[counter] * Mathf.Sin(Mathf.Deg2Rad * theta[counter]);
            instance.transform.position = place;
            hs = instance.GetComponent<HallucinationSpinner>();
            hs.speed = Random.Range(-10, 10);
            hs.pg = manager;
            hs.center = manager.islandCenter;
            hs.player = manager.player.transform;
        }
    }
}