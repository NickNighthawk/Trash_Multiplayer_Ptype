using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Transforms;
using Unity.NetCode;

[DisallowMultipleComponent]
//[RequiresEntityConversion]
public class PlayerIDAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new EntityToFollow());
    }
}
