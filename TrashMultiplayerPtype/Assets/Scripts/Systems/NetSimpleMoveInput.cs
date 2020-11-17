
using System;
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
    public bool isSprinting;
    public bool isJumping;
    public bool isGrounded;
    public bool isCrouching;

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
            //UnityEngine.Debug.Log("Camera not found, defaulting to " + cameraPosition);
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

        //input.isJumping = (Input.GetKey(KeyCode.Space));

        if (Input.GetKey(KeyCode.Space))
            input.isJumping = true;
        if (Input.GetKeyUp(KeyCode.Space))
            input.isJumping = false;

        if (Input.GetKey(KeyCode.LeftShift))
                    input.isSprinting = true;
        if (Input.GetKeyUp(KeyCode.LeftShift))
                    input.isSprinting = false;

        // Crouching
        if (Input.GetKey(KeyCode.LeftControl))
            input.isCrouching = true;
        if (Input.GetKeyUp(KeyCode.LeftControl))
            input.isCrouching = false;

        input.cameraPosition = cameraPosition;

        //if(Input.anyKey) Debug.Log(String.Format("Input system; {0}", input));

        var inputBuffer = EntityManager.GetBuffer<NetSimpleMoveInput>(localInput);
        inputBuffer.AddCommandData(input);
    }
}
