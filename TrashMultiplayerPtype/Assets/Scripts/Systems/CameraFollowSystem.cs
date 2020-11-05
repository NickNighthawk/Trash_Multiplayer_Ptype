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
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<NetworkIdComponent>();
    }
    protected override void OnUpdate()
    {
        //var playerEntity = GetSingleton<CommandTargetComponent>().targetEntity;
        int localPlayerID = GetSingleton<NetworkIdComponent>().Value;
        //Debug.Log("Local player id: " + localPlayerID);

        if (Follow.instance == null)
        {
            Debug.Log("instance of follow is null");
            return;
        }
        float3 GOpos = Follow.instance.transform.position;
        quaternion GOrot = Follow.instance.transform.rotation;

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

        Follow.instance.transform.position = GOpos;
        Follow.instance.transform.rotation = GOrot;
    }
}
