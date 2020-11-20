using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.NetCode;
using Unity.Physics;

[UpdateInGroup(typeof(GhostPredictionSystemGroup))]
[AlwaysSynchronizeSystem]
public class NetMovableSystem : SystemBase
{
    GhostPredictionSystemGroup m_GhostPredictionSystemGroup;
    protected override void OnCreate()
    {
        m_GhostPredictionSystemGroup = World.GetExistingSystem<GhostPredictionSystemGroup>();
    }
    protected override void OnUpdate()
    {
        var tick = m_GhostPredictionSystemGroup.PredictingTick;
        var deltaTime = Time.DeltaTime;

        // Non-physics body movement
        Entities
            .WithNone<PhysicsVelocity>()
            .WithAll<NetPlayerControllerComponent>()
            .ForEach((Entity e, ref Translation trans, ref Rotation rot, ref NetCharacterControllerComponent cControl, in PredictedGhostComponent prediction) =>
            {
                if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                    return;
                //Debug.Log(String.Format("Character control direction: {0}, Magnitude {1}",cControl.CurrentDirection, cControl.CurrentMagnitude));

                var step = (cControl.CurrentDirection * cControl.CurrentMagnitude * cControl.Speed) * deltaTime;
                var newTranslation = trans.Value + step;
                if(!math.isnan(newTranslation.x) && !math.isnan(newTranslation.y) && !math.isnan(newTranslation.z)) trans.Value = newTranslation;

                var newMagnitude = cControl.CurrentMagnitude - cControl.CurrentMagnitude * cControl.Drag * deltaTime;
                cControl.CurrentMagnitude = newMagnitude;

                var lookRotation = new float3(cControl.CurrentDirection.x, 0, cControl.CurrentDirection.z);
                var newRotation = quaternion.LookRotationSafe(lookRotation, new float3(0,1,0));
                rot.Value = newRotation;

                //Debug.Log(String.Format("Movable component system: Translation {0}, Magnitude {1}, Rotation {2}", trans.Value, newMagnitude, newRotation));

            }).ScheduleParallel();


    }
}
