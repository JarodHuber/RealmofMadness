using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpFlip : MonoBehaviour
{
    public FancyCreds dad;
    public float jumpHeight;
    public float timeToJumpUp;
    public float timeToFlip;
    float shrinkScale, shrinkStart;
    float timeElapsedShrink, timeElapsedJump;
    float jumpPos, start;
    Vector3 scale;
    bool passThrough1 = true;
    // Start is called before the first frame update
    void Start()
    {
        start = transform.position.y;
        shrinkStart = transform.localScale.y;
        scale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (passThrough1)
        {
            timeElapsedShrink += Time.deltaTime / timeToFlip;
            timeElapsedJump += Time.deltaTime / timeToJumpUp;

            if (timeElapsedShrink >= 1 && timeElapsedJump >= 1)
            {
                dad.sprRender.sprite = dad.Credits[dad.index];
                passThrough1 = false;
            }
        }
        else
        {
            timeElapsedShrink -= Time.deltaTime / timeToFlip;
            timeElapsedJump -= Time.deltaTime / timeToJumpUp;

            if (timeElapsedShrink <= 0 && timeElapsedJump <= 0)
            {
                dad.inputAcceptable = true;
                dad.doinSomthin = false;
                dad.jumperFlipper = null;
                Destroy(this);
            }
        }

        jumpPos = Mathf.Lerp(start, jumpHeight, timeElapsedJump);
        shrinkScale = Mathf.Lerp(shrinkStart, 0, timeElapsedShrink);

        transform.position = new Vector3(transform.position.x, jumpPos, transform.position.z);
        transform.localScale = new Vector3(scale.x * shrinkScale, scale.y, scale.z);

    }
}
