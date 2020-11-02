using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
//[RequiresEntityConversion]
public class FollowAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public GameObject followerObject;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        Follow follow = followerObject.GetComponent<Follow>();

        if (follow == null)
        {
            follow = followerObject.AddComponent<Follow>();
        }

        follow.entityToFollow = entity;

    }


}
