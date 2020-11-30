using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Unity.NetCode;
using Unity.Transforms;
using Unity.Mathematics;

namespace VertexFragment
{
    /// <summary>
    /// Main control system for networked player input.
    /// </summary>
    [UpdateInGroup(typeof(GhostPredictionSystemGroup))]
    public class NetPlayerControllerSystem : SystemBase
    {
        GhostPredictionSystemGroup m_GhostPredictionSystemGroup;

        protected override void OnCreate()
        {
            m_GhostPredictionSystemGroup = World.GetExistingSystem<GhostPredictionSystemGroup>();
        }

        protected override void OnUpdate()
        {
            var tick = m_GhostPredictionSystemGroup.PredictingTick;

            //var commandBuffer = new EntityCommandBuffer(Allocator.Temp).AsParallelWriter();

            Entities.WithAll<PlayerControllerComponent, NetPlayer>().
                ForEach((
                    Entity entity,
                    DynamicBuffer<NetPlayerInput> inputBuffer,
                    ref CharacterControllerComponent controller,
                    in PredictedGhostComponent prediction) =>
            {
                if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                    return;

                NetPlayerInput input;
                inputBuffer.GetDataAtTick(tick, out input);

                ProcessInput(ref controller, ref input);
            }).WithoutBurst().Run();

        }

        /// <summary>
        /// Processes the horizontal movement input from the player to move the entity along the xz plane.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="input"></param>
        private void ProcessInput(ref CharacterControllerComponent controller, ref NetPlayerInput input)
        {
            float movementX = input.horizontal;
            float movementZ = input.vertical;

            float3 dirFromCam = controller.CurrentPosition - input.cameraPosition;
            dirFromCam.y = 0;

            float3 forward = math.forward(controller.CurrentRotation);

            //use camera direction as forward if non-zero
            if (math.length(dirFromCam) > 0)
            {
                forward = math.normalize(dirFromCam);
            }

            float3 right = math.cross(forward, new float3(0, 1, 0));

            if (!MathUtils.IsZero(movementX) || !MathUtils.IsZero(movementZ))
            {
                controller.CurrentDirection = math.normalize((forward * movementZ) + (right * movementX));
                controller.CurrentMagnitude = input.isSprinting?1.5f:1f;
            }
            else
            {
                controller.CurrentMagnitude = 0.0f;
            }

            controller.Jump = input.isJumping;
        }
    }
}
