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
        var inputX = Input.GetAxis("Horizontal");
        var inputY = Input.GetAxis("Vertical");
        var dt = Time.DeltaTime;
        float3 cameraPosition = float3.zero;
        
        Entities
            .WithAll<CameraTag>()
            .ForEach((ref Translation translation) =>
            {
                cameraPosition = translation.Value;
            }).WithoutBurst().Run();

        Entities
            .WithAll<Player>()
            .ForEach((ref Movable mov, ref Rotation rot, in Translation translation) =>
            {
                float3 dirFromCam = translation.Value - cameraPosition;
                dirFromCam.y = 0;

                var forward = math.normalize(dirFromCam);
                var right = math.cross(forward, new float3(0, 1, 0));

                var moveDir = (forward * inputY) + (right * -inputX);

                mov.direction = moveDir;

                if (math.abs(inputX + inputY) > 0.1f)
                {
                    rot.Value = quaternion.LookRotation(moveDir, new float3(0, 1, 0));
                }

                
            }).Schedule();

    }
}