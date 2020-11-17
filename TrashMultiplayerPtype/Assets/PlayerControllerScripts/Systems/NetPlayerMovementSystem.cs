using System;
using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Collections;
using UnityEngine.Rendering;

/// <summary>
/// Main control system for player input.
/// </summary>
[UpdateInGroup(typeof(GhostPredictionSystemGroup))]
public class NetPlayerMovementSystem : SystemBase
{
    GhostPredictionSystemGroup m_GhostPredictionSystemGroup;

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<EnableNetSimpleMove>();
        m_GhostPredictionSystemGroup = World.GetExistingSystem<GhostPredictionSystemGroup>();
    }

    protected override void OnUpdate()
    {
        var tick = m_GhostPredictionSystemGroup.PredictingTick;

        Entities
            .WithAll<NetPlayerControllerComponent>()
            .ForEach((
                DynamicBuffer<NetSimpleMoveInput> inputBuffer,
                Entity entity,
                ref NetCharacterControllerComponent controller,
                ref Rotation rot,
                in DirectionFromCamera dirFromCam,
                in PredictedGhostComponent prediction
            ) =>
        {

            if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                return;

            NetSimpleMoveInput input;
            inputBuffer.GetDataAtTick(tick, out input);

            float movementX = input.horizontal;
            float movementZ = input.vertical;
            float movementY = (input.isJumping ? 1.0f : 0.0f) + (input.isCrouching ? -1.0f : 0.0f);

            //Debug.Log(String.Format("Movement system: direction from camera: {0}", dirFromCam.Value));

            if(math.isnan(dirFromCam.Value.x) || math.isnan(dirFromCam.Value.y) || math.isnan(dirFromCam.Value.z)) return;

            float3 forward = math.normalize(new float3(dirFromCam.Value.x, 0.0f, dirFromCam.Value.z));
            float3 up = new float3(0, 1, 0);
            float3 right = -math.cross(forward, up);

            if (!MathUtils.IsZero(movementX) || !MathUtils.IsZero(movementZ)|| !MathUtils.IsZero(movementY))
            {
                controller.CurrentDirection =
                    math.normalize(((forward * movementZ) + (right * movementX) + (up * movementY)));
                controller.CurrentMagnitude =
                    input.isSprinting ? 1.5f : 1.0f * controller.Speed;
            }
            else
            {
                controller.CurrentMagnitude = 0.0f;
            }

            controller.Crouch = input.isCrouching;
            controller.Jump = input.isJumping;

            //Debug.Log(String.Format("Movement System: Input forward: {0} right: {1} up: {2}", movementZ, movementX, movementY));
        }).ScheduleParallel();
    }
}

