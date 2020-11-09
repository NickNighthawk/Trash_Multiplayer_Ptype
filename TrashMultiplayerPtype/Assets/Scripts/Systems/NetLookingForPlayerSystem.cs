using Cinemachine;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;

public class NetLookingForPlayerSystem : SystemBase
{
    GhostPredictionSystemGroup m_GhostPredictionSystemGroup;

    protected override void OnCreate()
    {
        m_GhostPredictionSystemGroup = World.GetExistingSystem<GhostPredictionSystemGroup>();
    }

    protected override void OnUpdate()
    {
        var tick = m_GhostPredictionSystemGroup.PredictingTick;

        var player = Entity.Null;

        Transform playerTransform;

        // Find player
        Entities
            .WithAll<NetPlayer>()
            .ForEach((Entity e, in Transform transform, in PredictedGhostComponent prediction) =>
            {
                if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                    return;

                //Debug.Log("Found player target " + player);

                player = e;

            }).WithoutBurst().Run();

        if(player == Entity.Null) return;

        playerTransform = World.EntityManager.GetComponentObject<Transform>(player);

        // Find cinemachine free look camera
        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        Entities
            .WithAll<LookingForPlayer>()
            .ForEach((Entity e, CinemachineFreeLook cineCam, in PredictedGhostComponent prediction) =>
            {
                if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                    return;

                Debug.Log("Setting Cinemachine Camera target " + player);

                cineCam.Follow = playerTransform;
                cineCam.LookAt = playerTransform;

                commandBuffer.RemoveComponent<LookingForPlayer>(e);

            }).WithoutBurst().Run();
        commandBuffer.Playback(EntityManager);
    }
}
