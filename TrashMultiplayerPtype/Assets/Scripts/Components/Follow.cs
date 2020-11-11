using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Collections;
using Unity.NetCode;

public class Follow : MonoBehaviour
{
    public static Follow instance;

    private void Awake()
    {
        instance = this;
    }
   
}
