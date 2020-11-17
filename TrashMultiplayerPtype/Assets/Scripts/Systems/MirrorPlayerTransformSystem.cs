using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;
using Cinemachine;

[BurstCompile]
[UpdateInGroup(typeof(PresentationSystemGroup))]
public class MirrorPlayerTransformSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<NetworkIdComponent>();
    }
    protected override void OnUpdate()
    {
        int localPlayerID = GetSingleton<NetworkIdComponent>().Value;

        if (Follow.instance == null)
        {
            Debug.Log("instance of follow is null");
            return;
        }

        float3 GOpos = Follow.instance.transform.position;
        quaternion GOrot = Follow.instance.transform.rotation;

        Entities
            .ForEach((Entity e, ref Translation trns, ref Rotation rot, ref PlayerID player) =>
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
