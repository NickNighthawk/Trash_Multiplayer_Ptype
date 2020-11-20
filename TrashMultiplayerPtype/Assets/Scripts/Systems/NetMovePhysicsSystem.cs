using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using Unity.Physics;

public class NetMovePhysicsSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;

        //Physics body movement
        Entities
            .WithAll<NetPlayerControllerComponent>()
            .ForEach((Entity e, ref Rotation rot, ref PhysicsVelocity physVel, ref NetCharacterControllerComponent cControl) =>
            {
                //Debug.Log(String.Format("Character control direction: {0}, Magnitude {1}",cControl.CurrentDirection, cControl.CurrentMagnitude));

                var step = (cControl.CurrentDirection * cControl.CurrentMagnitude * cControl.Speed) * deltaTime;
                if (!math.isnan(step.x) && !math.isnan(step.y) && !math.isnan(step.z))
                {
                    physVel.Linear += step;
                }

                var lookRotation = new float3(cControl.CurrentDirection.x, 0, cControl.CurrentDirection.z);
                var newRotation = quaternion.LookRotationSafe(lookRotation, new float3(0,1,0));
                rot.Value = newRotation;

                //Debug.Log(String.Format("Movable component system: Translation {0}, Magnitude {1}, Rotation {2}", trans.Value, newMagnitude, newRotation));

            }).ScheduleParallel();
    }
}
