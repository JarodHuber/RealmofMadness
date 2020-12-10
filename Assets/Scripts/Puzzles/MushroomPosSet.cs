using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomPosSet : MonoBehaviour
{
    public List<Vector3> positions;

    public void SetMushroomPos(int shroomLocal)
    {
        transform.position = positions[shroomLocal];
    }
}
