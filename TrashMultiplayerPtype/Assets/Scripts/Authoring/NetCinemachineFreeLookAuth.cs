using Cinemachine;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.NetCode;

[DisallowMultipleComponent]
public class NetCinemachineFreeLookAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public CinemachineFreeLook cineCamFreeLook;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, default(GhostOwnerComponent));
        dstManager.AddComponentData(entity, new CineCamTag());
        dstManager.AddComponentData(entity, new LookingForPlayer());
        conversionSystem.AddHybridComponent(cineCamFreeLook);
    }
}

public class CineCamTag : IComponentData
{
}

public class LookingForPlayer : IComponentData
{
}
