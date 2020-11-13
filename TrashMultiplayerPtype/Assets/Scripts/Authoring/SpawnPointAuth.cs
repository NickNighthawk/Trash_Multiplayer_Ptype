using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;

[DisallowMultipleComponent]
public class SpawnPointAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public Vector3 spawnPointInWorld = Vector3.zero;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Translation(){Value = spawnPointInWorld});
        Debug.Log("Setting up spawn point at " + spawnPointInWorld);
    }
}

public struct SpawnPoint : IComponentData
{
    public float3 Value;
}
