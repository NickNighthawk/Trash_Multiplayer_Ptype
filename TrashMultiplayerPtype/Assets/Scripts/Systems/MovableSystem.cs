using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

public class MovableSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        Entities
            .WithNone<NetPlayer>()
            .ForEach((ref Translation trans, ref Rotation rot, ref CharacterControllerComponent cControl) =>
            {
                var step = (cControl.CurrentDirection * cControl.CurrentMagnitude) * deltaTime;
                cControl.CurrentMagnitude -= cControl.CurrentMagnitude * cControl.Drag * deltaTime;
                trans.Value += step;
                var lookRotation = new float3(cControl.CurrentDirection.x, 0, cControl.CurrentDirection.z);
                rot.Value = quaternion.LookRotationSafe(lookRotation, new float3(0,1,0));

            }).Run();
    }
}
