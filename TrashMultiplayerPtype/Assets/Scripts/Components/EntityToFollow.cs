using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

[GenerateAuthoringComponent]
public struct EntityToFollow : IComponentData
{
    [GhostField]
    public int playerID;
}
