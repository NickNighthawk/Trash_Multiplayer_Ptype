using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Scenes;

[DisallowMultipleComponent]
public class NetSubsceneReferenceAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public SubScene[] subScenes;
    public bool[] scenesLoaded;

    public float loadDistance = 5;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new SubsceneReference(){SubScenes = subScenes, ScenesLoaded = new bool[subScenes.Length], LoadDistance = loadDistance});
    }
}

public class SubsceneReference : IComponentData
{
    public SubScene[] SubScenes;
    public bool[] ScenesLoaded;
    public float LoadDistance;
}
