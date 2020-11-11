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
        Entities
            .ForEach((ref PhysicsVelocity physVel, ref Rotation rot, in Movable mov) =>
        {
            var step = mov.direction * mov.speed;
            physVel.Linear = step;
            if(mov.lockVerticalRotation) rot.Value = quaternion.LookRotation(math.forward(rot.Value), new float3(0, 1, 0));
        }).Schedule();
    }
}
