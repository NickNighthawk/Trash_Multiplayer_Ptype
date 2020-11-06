using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.NetCode;
using Cinemachine;

[DisallowMultipleComponent]
public class CameraPointAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    private Transform followerObject;

    private void Awake()
    {
        followerObject = this.transform;
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, default(GhostOwnerComponent));
        dstManager.AddComponentData(entity, new NetFollowTarget());
        conversionSystem.AddHybridComponent(followerObject);
    }
}
