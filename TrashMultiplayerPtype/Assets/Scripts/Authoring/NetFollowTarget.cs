using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class NetFollowTarget : IComponentData
{
    public Entity FollowTarget;
}
