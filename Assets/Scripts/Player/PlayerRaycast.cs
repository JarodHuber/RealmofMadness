using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerRaycast : MonoBehaviour
{
    public float DistanceToSee;
    RaycastHit WhatIHit;
    public bool back = true;
    public AudioSource audioSource;
    public AudioClip swish, smack;

    void FixedUpdate()
    {
        Debug.DrawRay(this.transform.position, this.transform.forward * DistanceToSee, Color.magenta);

        if (Physics.Raycast(this.transform.position, this.transform.forward, out WhatIHit, DistanceToSee, 9))
        {
            if (Input.GetMouseButtonDown(0))
            {
                audioSource.clip = smack;
                audioSource.Play();
                if (WhatIHit.collider.gameObject.GetComponentInParent<EnvironmentScript>() != null)
                {
                    WhatIHit.collider.gameObject.GetComponentInParent<EnvironmentScript>().Select();
                }
                else if (WhatIHit.collider.gameObject.GetComponentInParent<TagScript>() != null)
                {
                    WhatIHit.collider.gameObject.GetComponentInParent<TagScript>().Select();
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                audioSource.clip = swish;
                audioSource.Play();
            }
        }

    }
}
