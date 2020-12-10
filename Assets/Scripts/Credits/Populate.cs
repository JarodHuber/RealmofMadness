using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Populate : MonoBehaviour
{
    public bool isDie = false;

    public FancyCreds dad;

    Vector3 up = new Vector3(0, 28f, 0);
    Vector3 right = new Vector3(30f, 0, 0);
    public float spawnDelay, spawnDelay2, spawnDelay3, spawnDelay4;

    public float minTime, maxTime;
    GameObject instance;

    public Vector2 minBound = new Vector2(-520,-390);
    public Vector2 maxBound = new Vector2(520, 390);

    bool placeable = true;
    // Start is called before the first frame update
    void Start()
    {
        dad = GameObject.Find("FancyCredits").GetComponent<FancyCreds>();
        dad.gam.Add(gameObject);
        dad.vec.Add(transform.position);

        spawnDelay = Random.Range(minTime, maxTime);
        spawnDelay2 = Random.Range(minTime, maxTime);
        spawnDelay3 = Random.Range(minTime, maxTime);
        spawnDelay4 = Random.Range(minTime, maxTime);
        Vector3 mushUp = transform.position + up;
        Vector3 mushRight = transform.position + right;
        Vector3 mushLeft = transform.position - right;
        Vector3 mushDown = transform.position - up;

        if (mushUp.y <= maxBound.y)
            StartCoroutine(CloneSelfAtLocal(mushUp, spawnDelay));
        if (mushRight.x <= maxBound.x)
            StartCoroutine(CloneSelfAtLocal(mushRight, spawnDelay2));
        if (mushLeft.x >= minBound.x)
            StartCoroutine(CloneSelfAtLocal(mushLeft, spawnDelay3));
        if(mushDown.y >= minBound.y)
            StartCoroutine(CloneSelfAtLocal(mushDown, spawnDelay4));
    }

    public IEnumerator CloneSelfAtLocal(Vector3 loc, float time)
    {
        dad.mush.Add(this);
        yield return new WaitForSeconds(time);
        placeable = true;
        foreach (Vector3 pos in dad.vec)
            if (pos == loc)
            {
                placeable = false;
                break;
            }
        if (placeable)
        {
            instance = Instantiate(this.gameObject, loc, Quaternion.identity, dad.transform);
            instance.GetComponent<Populate>().minTime = minTime;
            instance.GetComponent<Populate>().maxTime = maxTime;
        }
        dad.mush.Remove(this);
    }
}
