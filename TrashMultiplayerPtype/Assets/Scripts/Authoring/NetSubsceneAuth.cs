using Unity.Entities;
using Unity.Mathematics;
using Unity.Scenes;
using UnityEngine;
using Unity.Transforms;

[DisallowMultipleComponent]
public class NetSubsceneAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public SubScene[] subscenes;
    public float loadDistance = 15;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new NetSubscene(){SubScenes = subscenes, LoadDistance = loadDistance});
    }
}

public class NetSubscene : IComponentData
{
    public SubScene[] SubScenes;
    public float LoadDistance;
}
