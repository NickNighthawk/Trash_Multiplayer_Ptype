using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.NetCode;
using Cinemachine;

[DisallowMultipleComponent]
//[RequiresEntityConversion]
public class NetCameraAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public AudioListener audioListener;
    public Camera cam;
    public CinemachineBrain brain;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, default(GhostOwnerComponent));
        dstManager.AddComponentData(entity, new CameraTag() { });

        conversionSystem.AddHybridComponent(cam);
        conversionSystem.AddHybridComponent(audioListener);
        conversionSystem.AddHybridComponent(brain);
    }
}
