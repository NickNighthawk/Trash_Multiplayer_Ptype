using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;


[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public class HandleDisconnectSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        // var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        
        Entities
            .WithNone<SendRpcCommandRequestComponent>()
            .ForEach((Entity e, ref NetworkStreamDisconnected disconnect, ref NetworkIdComponent netID) =>
            {
                NetworkStreamDisconnectReason reason = disconnect.Reason;
                int sourceID = netID.Value;
                Debug.Log($"Player {sourceID} has disconnected due to reason: {reason}");

                Entities.ForEach((Entity ent, ref PlayerID playerId) =>
                {
                    if (playerId.playerID == sourceID)
                    {
                        //commandBuffer.DestroyEntity(ent);
                        PostUpdateCommands.DestroyEntity(ent);
                    }
                });
            });
        //commandBuffer.Playback(EntityManager);
    }
}


