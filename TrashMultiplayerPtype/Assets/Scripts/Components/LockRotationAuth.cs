using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[GenerateAuthoringComponent]
public class LockRotation : IComponentData
{
    public bool isLocked = false;
    public Rotation rotation;
}
