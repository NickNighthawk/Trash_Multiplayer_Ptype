using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.NetCode;
using Unity.Physics;

/*
[UpdateInGroup(typeof(GhostPredictionSystemGroup))]
[AlwaysSynchronizeSystem]
public class NetMovableSystem : SystemBase //DISABLED
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
            .WithNone<NetPlayerControllerComponent>()
            .ForEach((Entity e, ref Translation trans, ref Rotation rot, ref NetCharacterControllerComponent cControl, in PredictedGhostComponent prediction) =>
            {
                if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                    return;
                //Debug.Log(String.Format("Character control direction: {0}, Magnitude {1}",cControl.CurrentDirection, cControl.CurrentMagnitude));

                // Total current velocity
                var currentVelocity = cControl.VerticalVelocity + cControl.HorizontalVelocity;

                // Player input velocity
                var inputVelocity = (cControl.CurrentDirection * cControl.CurrentMagnitude * cControl.Speed);

                // Player input opposing force
                var inputStoppingForce = float3.zero;

                 Currently not working as intended. Maybe don't need it anyhow.
                if (math.pow((currentVelocity.x + inputVelocity.x), 2) < math.pow(currentVelocity.x, 2))
                    inputStoppingForce.x = -cControl.InputStoppingForce;
                if (math.pow((currentVelocity.z + inputVelocity.z), 2) < math.pow(currentVelocity.z, 2))
                    inputStoppingForce.z = -cControl.InputStoppingForce;


                // Ground Check
                bool isGrounded = cControl.IsGrounded;

                // Gravity
                var gravityStep = cControl.Gravity;

                // *Collisions*
                var collisionVelocity = float3.zero;

                // Drag
                var dragCoefficient = isGrounded ? cControl.DragGrounded:cControl.DragInAir;
                var drag = (currentVelocity + inputVelocity + gravityStep + collisionVelocity + inputStoppingForce) * dragCoefficient;

                // Combine forces
                var newVelocity = currentVelocity + inputVelocity + gravityStep + collisionVelocity + inputStoppingForce - drag;
                var step = newVelocity * deltaTime;

                var newTranslation = trans.Value + step;
                if (!math.isnan(newTranslation.x) && !math.isnan(newTranslation.y) && !math.isnan(newTranslation.z))
                {
                    trans.Value = newTranslation;

                    // Update Character Controller velocity

                }

                var lookRotation = new float3(cControl.CurrentDirection.x, 0, cControl.CurrentDirection.z);
                var newRotation = quaternion.LookRotationSafe(lookRotation, new float3(0,1,0));
                rot.Value = newRotation;

                // Update Character Controller
                cControl.HorizontalVelocity = new float3(newVelocity.x, 0, newVelocity.z);
                cControl.VerticalVelocity = new float3(0, newVelocity.y, 0);

                //Debug.Log(String.Format("Movable component system: Translation {0}, Magnitude {1}, Rotation {2}", trans.Value, newMagnitude, newRotation));

            }).ScheduleParallel();


    }

}
*/
