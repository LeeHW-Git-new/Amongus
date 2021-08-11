using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpwanPosition : MonoBehaviour
{
    [SerializeField]
    private Transform[] positions;

    private int index;

    public Vector3 GetSpawnPositon()
    {
        Vector3 pos = positions[index++].position;
        if(index >= positions.Length)
        {
            index = 0;
        }
        return pos;
    }
}
