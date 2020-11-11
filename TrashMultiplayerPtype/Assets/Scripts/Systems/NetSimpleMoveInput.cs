
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;

public struct NetSimpleMoveInput : ICommandData
{
    public uint Tick {get; set;}
    public int horizontal;
    public int vertical;
    public float3 cameraPosition;
}

[UpdateInGroup(typeof(ClientSimulationSystemGroup))]
[AlwaysSynchronizeSystem]
public class SampleNetSimpleMoveInput : SystemBase
{
    ClientSimulationSystemGroup m_ClientSimulationSystemGroup;

    private float3 cameraPosition = new float3(0, 2, 2);
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<NetworkIdComponent>();
        RequireSingletonForUpdate<EnableNetSimpleMove>();
        m_ClientSimulationSystemGroup = World.GetExistingSystem<ClientSimulationSystemGroup>();
    }

    protected override void OnUpdate()
    {
        var localInput = GetSingleton<CommandTargetComponent>().targetEntity;

        var mainCam = Camera.main;

        if (mainCam == null)
        {
            UnityEngine.Debug.Log("Camera not found, defaulting to " + cameraPosition);
        }
        else
        {
            cameraPosition = mainCam.transform.position;
            //UnityEngine.Debug.Log("Input Camera position " + cameraPosition);
        }

        if (localInput == Entity.Null)
        {
            var localPlayerId = GetSingleton<NetworkIdComponent>().Value;
            var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
            var commandTargetEntity = GetSingletonEntity<CommandTargetComponent>();
            Entities.WithAll<NetMovementComponent>().WithNone<NetSimpleMoveInput>().ForEach((Entity ent, ref GhostOwnerComponent ghostOwner) =>
            {
                if (ghostOwner.NetworkId == localPlayerId)
                {
                    commandBuffer.AddBuffer<NetSimpleMoveInput>(ent);
                    commandBuffer.SetComponent(commandTargetEntity, new CommandTargetComponent {targetEntity = ent});
                }
            }).Run();
            commandBuffer.Playback(EntityManager);
            return;
        }
        var input = default(NetSimpleMoveInput);
        input.Tick = m_ClientSimulationSystemGroup.ServerTick;
        if (Input.GetKey("a"))
            input.horizontal -= 1;
        if (Input.GetKey("d"))
            input.horizontal += 1;
        if (Input.GetKey("s"))
            input.vertical -= 1;
        if (Input.GetKey("w"))
            input.vertical += 1;

        input.cameraPosition = cameraPosition;

        var inputBuffer = EntityManager.GetBuffer<NetSimpleMoveInput>(localInput);
        inputBuffer.AddCommandData(input);
    }
}
