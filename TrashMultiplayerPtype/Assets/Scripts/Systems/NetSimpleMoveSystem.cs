using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

[UpdateInGroup(typeof(GhostPredictionSystemGroup))]
public class NetSimpleMoveSystem : SystemBase
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

        // Update player movement
        Entities
            .WithAll<Player>()
            .ForEach((DynamicBuffer<NetSimpleMoveInput> inputBuffer, ref Movable mov, ref Rotation rot, ref Translation trans, in PredictedGhostComponent prediction) =>
        {
            if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                return;

            NetSimpleMoveInput input;
            inputBuffer.GetDataAtTick(tick, out input);

            /* Cube input
            if (input.horizontal > 0)
                trans.Value.x += deltaTime;
            if (input.horizontal < 0)
                trans.Value.x -= deltaTime;
            if (input.vertical > 0)
                trans.Value.z += deltaTime;
            if (input.vertical < 0)
                trans.Value.z -= deltaTime;
            */

            float3 dirFromCam = trans.Value - new float3(Camera.main.transform.position);
            dirFromCam.y = 0;

            var forward = math.normalize(dirFromCam);
            var right = math.cross(forward, new float3(0, 1, 0));

            var moveDir = (forward * input.vertical * deltaTime) + (right * -input.horizontal * deltaTime);

            mov.direction = moveDir;

            if (math.abs(input.horizontal + input.vertical) > 0.1f)
            {
                rot.Value = quaternion.LookRotation(moveDir, new float3(0, 1, 0));
            }

        }).ScheduleParallel();
    }
}
