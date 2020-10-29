using UnityEngine;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class PlayerMoveSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");
        var dt = Time.DeltaTime;

        Entities
            .WithAll<Player>()
            .ForEach((ref Movable mov) =>
        {
            mov.direction = new float3(x, 0, y);
            //rot.Value = math.mul(rot.Value, quaternion.RotateY(math.radians(2 * dt)));
        }).Schedule();

    }
}