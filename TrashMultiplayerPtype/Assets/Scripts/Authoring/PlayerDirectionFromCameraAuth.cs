using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerDirectionFromCameraAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new DirectionFromCamera());
    }
}

public struct DirectionFromCamera : IComponentData
{
    public float3 Value;
}
