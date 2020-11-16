using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Physics.Authoring;
using Unity.Physics;


[DisallowMultipleComponent, RequireComponent(typeof(PhysicsBodyAuthoring))]
public class FreezeRotationAuthoring : MonoBehaviour
{
    public bool3 Flags;
}

[UpdateAfter(typeof(PhysicsBodyConversionSystem))]
public class FreezeRotationConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((FreezeRotationAuthoring freezeRotation) =>
        {
            Entity entity = GetPrimaryEntity(freezeRotation.gameObject);
            PhysicsMass mass = DstEntityManager.GetComponentData<PhysicsMass>(entity);
            if (freezeRotation.Flags.x)
                mass.InverseInertia.x = 0f;
            if (freezeRotation.Flags.y)
                mass.InverseInertia.y = 0f;
            if (freezeRotation.Flags.z)
                mass.InverseInertia.z = 0f;
            DstEntityManager.SetComponentData(entity, mass);
        });
    }
}
