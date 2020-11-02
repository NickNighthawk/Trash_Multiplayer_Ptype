using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;

[DisallowMultipleComponent]
//[RequiresEntityConversion]
public class FollowAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<EntityToFollow>(entity);
    }
}
