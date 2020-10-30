using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
//[RequiresEntityConversion]
public class FollowAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public GameObject followerObject;
    //public AudioListener audioListener;
    //public Camera cam;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        FollowEntity followEntity = followerObject.GetComponent<FollowEntity>();
        if (followEntity == null)
        {
            followEntity = followerObject.AddComponent<FollowEntity>();
        }

        followEntity.entityToFollow = entity;
        
        //dstManager.AddComponentData(entity, new CameraTag() {}); 
        //conversionSystem.AddHybridComponent(audioListener);
        //conversionSystem.AddHybridComponent(cam);
    }


}
