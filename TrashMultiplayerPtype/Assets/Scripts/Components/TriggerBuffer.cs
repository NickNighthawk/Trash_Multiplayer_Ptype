using UnityEngine;
using System.Collections;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct TriggerBuffer : IBufferElementData
{
    public Entity entity;
}