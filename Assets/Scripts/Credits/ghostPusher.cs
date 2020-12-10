using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ghostPusher : MonoBehaviour
{
    public ghostSpawner dad;
    public Vector3 start, finish;
    public float timer;
    public float timeToGoAcross;
    float scale;
    float order;
    float range;
    public SpriteRenderer sprRenderer;
    float min1 = 1, max1 = 2, min2 = 0.7f, max2 = 1.2f;
    // Start is called before the first frame update
    void Start()
    {
        start = transform.position;
        scale = Random.Range(min1, max1);
        transform.localScale = new Vector3(scale * Random.Range(min2,max2), scale * Random.Range(min2,max2));
        order = transform.localScale.x + transform.localScale.y;
        order /= 2;
        range = (max1 * max2) - (min1 * max1);
        order /= range;
        sprRenderer.sortingOrder = (int)order;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(start, finish, timer);
        timer += Time.deltaTime / timeToGoAcross;
        if (timer >= 1)
        {
            dad.dad.ghosts.Remove(gameObject);
            Destroy(gameObject);
        }
    }
}