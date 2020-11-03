using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class FollowMono : MonoBehaviour
{
    public float3 offset = new float3(0f, 0f, 0f);

    private EntityManager manager;

    private void Awake()
    {
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    private void LateUpdate()
    {
        EntityQuery query = manager.CreateEntityQuery(typeof(EntityToFollow));

        if (!query.IsEmpty)
        {
            Translation entPos = manager.GetComponentData<Translation>(query.GetSingletonEntity());

            transform.position = entPos.Value + offset;
        }

    }
}
