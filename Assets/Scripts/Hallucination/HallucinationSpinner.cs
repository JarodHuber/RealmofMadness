using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallucinationSpinner : MonoBehaviour
{
    public float speed;
    public Vector3 center;
    public Transform player;
    public Vector3 centerOffSet;
    public float angle;
    public PuzzleGeneration pg;

    void Start()
    {
        if(speed == 0)
        {
            speed = pg.methodLibrary.SkewedNum(1, 1, 2, 1);
            if (speed == 2)
                speed = -1;
        }
        transform.rotation = new Quaternion(0, 2 * Mathf.PI - angle, 0, transform.rotation.w);
    }

    void Update()
    {
        transform.RotateAround(center,Vector3.up,speed*Time.deltaTime);
        transform.LookAt(player);
    }
}