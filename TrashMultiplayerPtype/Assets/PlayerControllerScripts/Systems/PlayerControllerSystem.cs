using System;
using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;


/// <summary>
/// Main control system for player input.
/// </summary>
public class PlayerControllerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithAll<PlayerControllerComponent>().ForEach((
            Entity entity,
            ref CharacterControllerComponent controller,
            ref Rotation rot,
            in DirectionFromCamera dirFromCam
            ) =>
        {
            float movementX = (Input.GetAxis("Move Right") > 0.0f ? 1.0f : 0.0f) + (Input.GetAxis("Move Left") > 0.0f ? -1.0f : 0.0f);
            float movementZ = (Input.GetAxis("Move Forward") > 0.0f ? 1.0f : 0.0f) + (Input.GetAxis("Move Backward") > 0.0f ? -1.0f : 0.0f);
            float movementY = (Input.GetAxis("Jump") > 0.0f ? 1.0f : 0.0f) + (Input.GetAxis("Crouch") > 0.0f ? -1.0f : 0.0f);

            float3 forward = math.normalize(new float3(dirFromCam.Value.x, 0.0f, dirFromCam.Value.z));
            float3 up = new float3(0, 1, 0);
            float3 right = -math.cross(forward, up);

            if (!MathUtils.IsZero(movementX) || !MathUtils.IsZero(movementZ)|| !MathUtils.IsZero(movementY))
            {
                controller.CurrentDirection = math.normalize(((forward * movementZ) + (right * movementX) + (up * movementY)));
                controller.CurrentMagnitude = Input.GetKey(KeyCode.LeftShift) ? 1.5f : 1.0f * controller.Speed;
            }
            else
            {
                controller.CurrentMagnitude = 0.0f;
            }

            controller.Jump = Input.GetAxis("Jump") > 0.0f;
            controller.Crouch = Input.GetAxis("Crouch") > 0.0f;
            //Debug.Log(String.Format("Input {0} vertical {1} horizontal", movementZ, movementX));
        }).WithoutBurst().Run();
    }
}

