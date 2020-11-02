using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class CameraLookAtPlayerSystem : SystemBase
{
    //private float lookSpeed = 2f;

    float3 playerPosition = float3.zero;

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        Entities
            .WithAll<Player>()
            .ForEach((in Translation translation) =>
            {
                playerPosition = translation.Value;
            }).WithoutBurst().Run();

        Entities
            .WithAll<CameraTag>()
            .ForEach((ref Translation translation, ref Rotation rotation) =>
            {
                var up = math.cross(new float3(1,0,0), translation.Value - playerPosition);

                rotation.Value = quaternion.LookRotationSafe((translation.Value - playerPosition), up);

            }).WithoutBurst().Run();

    }
}


