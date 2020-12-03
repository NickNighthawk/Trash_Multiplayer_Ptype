using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCube : MonoBehaviour
{
    public Vector3 cubeCentreOffset = Vector3.zero;
    public float cubeSize = 10f;
    public Color cubeColor = Color.green;

    // Start is called before the first frame update
    private void OnDrawGizmos()
    {
        Gizmos.color = cubeColor;
        Gizmos.DrawWireCube(this.transform.position + cubeCentreOffset, Vector3.one * cubeSize);
    }
}
