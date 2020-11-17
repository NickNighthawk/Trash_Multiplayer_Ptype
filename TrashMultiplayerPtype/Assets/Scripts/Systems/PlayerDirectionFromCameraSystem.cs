using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class SetPlayerDirectionFromCamera : SystemBase
{
    protected override void OnUpdate()
    {
        float3 cameraPosition = Camera.main.transform.position;

        Entities
            .WithAll<CameraTag>()
            .ForEach((ref Translation translation) =>
            {
                cameraPosition = translation.Value;
            }).WithoutBurst().Run();

        Entities
            .ForEach((ref Translation translation, ref DirectionFromCamera dirFromCam) => {

                dirFromCam.Value = translation.Value - cameraPosition;
        }).Run();
    }
}
