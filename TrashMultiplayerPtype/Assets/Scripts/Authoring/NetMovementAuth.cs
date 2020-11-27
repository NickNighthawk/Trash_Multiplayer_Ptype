using Unity.Entities;
using UnityEngine;
using Unity.Transforms;

[DisallowMultipleComponent]
public class NetMovementAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        //dstManager.AddComponentData(entity, new Translation(){Value = this.gameObject.transform.position});
        //dstManager.AddComponentData(entity, new Rotation(){Value = this.gameObject.transform.rotation});
        dstManager.AddComponentData(entity, new NetMovementComponent());
    }
}

public struct NetMovementComponent : IComponentData
{
}
