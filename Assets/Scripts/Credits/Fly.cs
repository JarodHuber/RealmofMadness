using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fly : MonoBehaviour
{
    public float startScaleMult = 0, endScaleMult = 100, currScaleMult;

    public Vector3 pos, finish, start;
    public Vector3 scale;
    public FancyCreds dad;
    public float timeToGrow = 5;
    public float timeElapse;
    public float lerpValue;
    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
        start = transform.position;
        finish = dad.cammy.transform.position;
        scale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        timeElapse += Time.deltaTime / timeToGrow;
        lerpValue = Mathf.Pow(timeElapse, 5);
        currScaleMult = Mathf.Lerp(startScaleMult, endScaleMult, lerpValue);

        transform.localScale =  scale * currScaleMult;
        if(timeElapse >= 1.1f)
        {
            dad.sprRender.sprite = dad.Credits[dad.index];
            dad.inputAcceptable = true;
            dad.doinSomthin = false;
            Destroy(gameObject);
        }

        //transform.position = Vector3.Lerp(pos, finish, timeElapse);
        //timeElapse += Time.deltaTime/TimeToFly;
        //if(Mathf.Abs(Vector3.Distance(transform.position, finish)) < 1)
        //{
        //    dad.portalHiding = true;
        //}
        //if(transform.position.z <= -499.5)
        //{
        //    dad.portalPast = true;
        //}
        //pos = transform.position;
    }
}
