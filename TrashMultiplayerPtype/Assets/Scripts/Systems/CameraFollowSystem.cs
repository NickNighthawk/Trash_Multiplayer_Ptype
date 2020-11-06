using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateInGroup(typeof(ClientSimulationSystemGroup))]
[AlwaysSynchronizeSystem]
public class CameraFollowSystem : SystemBase
{
    GhostPredictionSystemGroup m_GhostPredictionSystemGroup;
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<NetworkIdComponent>();
        RequireSingletonForUpdate<Follow>();
        m_GhostPredictionSystemGroup = World.GetExistingSystem<GhostPredictionSystemGroup>();
    }
    protected override void OnUpdate()
    {
        //var playerEntity = GetSingleton<CommandTargetComponent>().targetEntity;
        int localPlayerID = GetSingleton<NetworkIdComponent>().Value;
        //Debug.Log("Local player id: " + localPlayerID);

        var tick = m_GhostPredictionSystemGroup.PredictingTick;

        /*
        if (Follow.instance == null)
        {
            Debug.Log("instance of follow is null");
            return;
        }
        float3 GOpos = Follow.instance.transform.position;
        quaternion GOrot = Follow.instance.transform.rotation;
        */

        float3 GOpos = float3.zero;
        quaternion GOrot = quaternion.identity;

        Entities
            .WithAll<NetPlayer>()
            .ForEach((in Translation translation, in Rotation rot, in PredictedGhostComponent prediction) =>
            {
                if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                    return;

                GOpos = translation.Value;
                GOrot = rot.Value;

            }).WithoutBurst().Run();

        Entities
            .ForEach((Entity e, ref Translation trns, ref Rotation rot, ref EntityToFollow player) =>
            {
                if (player.playerID == localPlayerID)
                {
                    GOpos.x = trns.Value.x;
                    GOpos.y = trns.Value.y;
                    GOpos.z = trns.Value.z;
                    GOrot = rot.Value;
                }
            }).Run();
    }
}
