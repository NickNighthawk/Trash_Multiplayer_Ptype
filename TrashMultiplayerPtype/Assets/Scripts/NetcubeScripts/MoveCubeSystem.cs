using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using Unity.Collections;
using UnityEngine;
using System;
using Unity.Transforms;
//MoveCubeSystem.cs



[UpdateInGroup(typeof(GhostPredictionSystemGroup))]
public class MoveCubeSystem : SystemBase
{
    GhostPredictionSystemGroup m_GhostPredictionSystemGroup;

    protected override void OnCreate()
    {
        m_GhostPredictionSystemGroup = World.GetExistingSystem<GhostPredictionSystemGroup>();
    }

    protected override void OnUpdate()
    {
        var tick = m_GhostPredictionSystemGroup.PredictingTick;
        var dt = Time.DeltaTime;
        Entities.ForEach((DynamicBuffer<CubeInput> inputBuffer, ref Translation trns, in PredictedGhostComponent prediction) =>
        {
            if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                return;
            CubeInput input;
            inputBuffer.GetDataAtTick(tick, out input);
            if(input.horizontal > 0)
                trns.Value.x += dt;
            if (input.horizontal < 0)
                trns.Value.x -= dt;
            if (input.vertical > 0)
                trns.Value.z += dt;
            if (input.vertical < 0)
                trns.Value.z -= dt;

        }).ScheduleParallel();
    }
}
