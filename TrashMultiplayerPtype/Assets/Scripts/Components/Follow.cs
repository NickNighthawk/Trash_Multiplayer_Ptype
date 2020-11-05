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
    ////use late update for cams 
    //private void LateUpdate()
    //{
    //    //prev: check if entity to follow is null (assigned in inspector)

    //    Debug.Log("Player entity in mono: " + playerEnt);

    //    //Debug.Log("Query test: " + playerQuery); 
    //    if (playerEnt == Entity.Null)
    //        return;
    //    else
    //    {
    //        Translation entityPos = manager.GetComponentData<Translation>(playerEnt);
    //        //Debug.Log("entity pos: " + entityPos.Value);
    //        transform.position = entityPos.Value;
    //    }
    //}
}
