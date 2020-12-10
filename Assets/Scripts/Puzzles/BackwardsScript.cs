using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackwardsScript : MonoBehaviour
{
    public PuzzleGeneration sg;
    public bool isPuzzle = false;
    public GameObject player;
    public GameObject portal;

    private void Update()
    {
        if (isPuzzle && (Vector3.Distance(portal.transform.position, player.transform.position) >=
                         Vector3.Distance(portal.transform.position, player.transform.position + player.transform.forward)))
            sg.puzzleSolved = false;
        else if(isPuzzle)
            sg.puzzleSolved = true;
    }
}
