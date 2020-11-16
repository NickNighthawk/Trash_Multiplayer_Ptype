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
        Entities
            .WithNone<NetPlayer>()
            .ForEach((ref PhysicsVelocity physVel, ref PhysicsMass physMass, ref Rotation rot, in Movable mov) =>
        {
            var step = mov.direction * mov.speed;
            physVel.Linear = step;
            physMass.InverseInertia.x = 0f;
            physMass.InverseInertia.z = 0f;
        }).Schedule();
    }
}
