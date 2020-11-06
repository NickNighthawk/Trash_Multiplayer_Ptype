using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

[UpdateInGroup(typeof(GhostPredictionSystemGroup))]
public class NetSimpleMoveSystem : SystemBase
{
    GhostPredictionSystemGroup m_GhostPredictionSystemGroup;
    private Transform cameraTransform;
    protected override void OnCreate()
    {
        m_GhostPredictionSystemGroup = World.GetExistingSystem<GhostPredictionSystemGroup>();
    }
    protected override void OnUpdate()
    {
        var tick = m_GhostPredictionSystemGroup.PredictingTick;
        var deltaTime = Time.DeltaTime;

        Entities
            .WithAll<LookingForCamera>()
            .ForEach((Entity e, in PredictedGhostComponent prediction) =>
            {
                if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                    return;

                Debug.Log("Found camera" + e);
                cameraTransform = World.EntityManager.GetComponentObject<Transform>(e);

                World.EntityManager.RemoveComponent<LookingForCamera>(e);
            }).WithoutBurst().WithStructuralChanges().Run();

        // Update player movement
        Entities
            .WithAll<NetPlayer>()
            .ForEach((DynamicBuffer<NetSimpleMoveInput> inputBuffer, ref Movable mov, ref Rotation rot,
                ref Translation trans, in PredictedGhostComponent prediction) =>
            {
                if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                    return;

                NetSimpleMoveInput input;
                inputBuffer.GetDataAtTick(tick, out input);

                float3 camPoint = cameraTransform != null ? new float3(cameraTransform.position) : float3.zero;

                float3 dirFromCam = trans.Value - camPoint;

                dirFromCam.y = 0;

                var forward = math.normalize(dirFromCam);
                var right = math.cross(forward, new float3(0, 1, 0));

                var moveDir = (forward * input.vertical) + (right * -input.horizontal);

                mov.direction = moveDir;

                if (math.abs(input.horizontal + input.vertical) > 0.1f)
                {
                    rot.Value = quaternion.LookRotation(moveDir, new float3(0, 1, 0));
                }

            }).WithoutBurst().Run();
    }
}
