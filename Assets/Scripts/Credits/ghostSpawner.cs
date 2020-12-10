using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ghostSpawner : MonoBehaviour
{
    public FancyCreds dad;
    public GameObject instance;
    public Vector3 finish, offset;
    public Vector3 rot = Vector3.zero;
    public Sprite ghost;
    public SpriteRenderer sprRenderer;
    public ghostPusher push;
    public float timeLeftToSpawn;
    public float amountPerTick;
    public bool isDie = false;
    public float spawnRangeL;
    public float spawnRangeS;
    public float timeToGoAcross;
    public float rotz;
    float x, y, z;
    // Start is called before the first frame update
    void Start()
    {
        finish = -transform.position;
        if (transform.position == dad.ghostSpawnLoc[0])
            rotz = 0;
        else if (transform.position == dad.ghostSpawnLoc[1])
            rotz = 180;
        else if (transform.position == dad.ghostSpawnLoc[2])
            rotz = 90;
        else if (transform.position == dad.ghostSpawnLoc[3])
            rotz = 270;

        rot.z = rotz;
        print(rotz);
    }

    // Update is called once per frame
    void Update()
    {
        if (timeLeftToSpawn <= 0)
            isDie = true;
        if (!isDie)
        {
            for (int counter = 0; counter < amountPerTick; counter++)
            {
                instance = new GameObject("ghostie");
                if(rotz == 0 || rotz == 180)
                {
                    x = Random.Range(transform.position.x - spawnRangeL, transform.position.x + spawnRangeL);
                    y = Random.Range(transform.position.y - spawnRangeS, transform.position.y + spawnRangeS);
                } else if (rotz == 90 || rotz == 270)
                {
                    x = Random.Range(transform.position.x - spawnRangeS, transform.position.x + spawnRangeS);
                    y = Random.Range(transform.position.y - spawnRangeL, transform.position.y + spawnRangeL);
                }
                offset = new Vector3(x, y, z);
                instance.transform.position = offset;
                push = instance.AddComponent<ghostPusher>();
                push.dad = this;
                push.finish = finish + offset;
                if (transform.position == dad.ghostSpawnLoc[0] || transform.position == dad.ghostSpawnLoc[1])
                    push.finish.y = finish.y;
                else if (transform.position == dad.ghostSpawnLoc[2] || transform.position == dad.ghostSpawnLoc[3])
                    push.finish.x = finish.x;
                push.timeToGoAcross = timeToGoAcross;
                sprRenderer = instance.AddComponent<SpriteRenderer>();
                sprRenderer.sprite = ghost;
                push.sprRenderer = sprRenderer;
                instance.transform.localEulerAngles = rot;
                dad.ghosts.Add(instance);
            }
            timeLeftToSpawn -= Time.deltaTime;
        }
    }
}