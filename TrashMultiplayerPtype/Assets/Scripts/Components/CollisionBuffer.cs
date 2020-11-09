using UnityEngine;
using System.Collections;
using Unity.Entities;

public struct CollisionBuffer : IBufferElementData
{
    public Entity entity;
}