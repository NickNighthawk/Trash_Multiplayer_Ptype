using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class AudioListenerAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public AudioListener audioListener;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var aQuery =
            World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<AudioListener>());
        if (aQuery.IsEmpty)
        {
            Debug.Log("No AudioListener found, converting AudioListener");
            conversionSystem.AddHybridComponent(audioListener);
        }
        else Debug.Log("AudioListener found");
    }
}
