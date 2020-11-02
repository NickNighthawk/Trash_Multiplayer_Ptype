using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Entity entityToFollow;
    public float3 offset = new float3(0f, 0f, 0f);

    private EntityManager manager;

    private void Awake()
    {
        manager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    private void LateUpdate()
    {
        if (entityToFollow == Entity.Null)
        {
            entityToFollow = manager.CreateEntityQuery(typeof(Player)).GetSingletonEntity();
            return;
        }

        Translation entPos = manager.GetComponentData<Translation>(entityToFollow);
        transform.position = entPos.Value + offset;
    }
}
