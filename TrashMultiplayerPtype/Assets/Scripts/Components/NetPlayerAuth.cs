using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

[DisallowMultipleComponent]
public class NetPlayerAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, default(GhostOwnerComponent));
        dstManager.AddComponentData(entity, new NetPlayer());
        conversionSystem.AddHybridComponent(transform);

    }
}
public struct NetPlayer : IComponentData
{
}
