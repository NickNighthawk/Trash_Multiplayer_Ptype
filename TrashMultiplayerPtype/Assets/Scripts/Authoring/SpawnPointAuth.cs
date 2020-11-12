using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class SpawnPointAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new SpawnPoint(){Value = this.transform.position});
        Debug.Log("Setting up spawn point at " + transform.position);
        DestroyImmediate(this);
    }
}

public struct SpawnPoint : IComponentData
{
    public float3 Value;
}
