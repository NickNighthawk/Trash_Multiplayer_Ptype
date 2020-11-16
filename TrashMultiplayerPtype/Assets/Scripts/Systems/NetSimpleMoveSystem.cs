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

        //float maxCamDistance = 5f;
        float3 camDebugPos = new float3(0,2,2);

        // Update player movement
        Entities
            .WithAll<NetPlayer>()
            .ForEach((DynamicBuffer<NetSimpleMoveInput> inputBuffer, DynamicBuffer<CollisionBuffer> col, ref Movable mov, ref Rotation rot, ref Translation trans, in PredictedGhostComponent prediction) =>
        {
            if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                return;

            NetSimpleMoveInput input;
            inputBuffer.GetDataAtTick(tick, out input);

            if (col.IsEmpty == false)
                input.isGrounded = true;
            else
                input.isGrounded = false;

            //UnityEngine.Debug.Log("NetMove: Player position " + trans.Value);
            //UnityEngine.Debug.Log("NetMove: Camera position " + input.cameraPosition);
            //UnityEngine.Debug.Log(string.Format("NetMove: Input - Tick {0} - Horizontal {1} - Vertical {2} - CameraPosition {3}", input.Tick, input.horizontal, input.vertical, input.cameraPosition));

            float3 dirFromCam = trans.Value - input.cameraPosition;
            dirFromCam.y = 0;

            //UnityEngine.Debug.Log("NetMove: Direction from camera " + dirFromCam);

            float3 forward = math.forward(rot.Value);

            //use camera direction as forward if non-zero
            if (math.length(dirFromCam) > 0)
            {
                forward = math.normalize(dirFromCam);
                //UnityEngine.Debug.Log("NetMove: camera forward " + forward);
            }
            //else UnityEngine.Debug.Log("NetMove: player forward " + forward);

            float3 right = math.cross(forward, new float3(0, 1, 0));            //what does this do?
            //UnityEngine.Debug.Log("NetMove: right " + right);

            float3 jumpForce = new float3(0, 10, 0);
            float3 moveDir = (forward * input.vertical) + (right * -input.horizontal); //on ground, not jumping

            if (input.isGrounded && input.isJumping) //on ground, jumping
            {
                moveDir =+ jumpForce;
            }
            else if(!input.isGrounded && input.isJumping) //not on ground, jumping
            {
                moveDir = -new float3(0, 2, 0);
            }
            else if (!input.isGrounded && !input.isJumping) //not on ground, not jumping...
            {
                //could add a down force... (like gravity)
                moveDir =- new float3(0, 2, 0);
            }

            float3 jumpDir = (input.isJumping ? 1 : -1) * new float3(0, 1, 0);

            mov.direction = (moveDir + jumpDir);
            //UnityEngine.Debug.Log("NetMove: New move direction " + moveDir);

            if (math.abs(input.horizontal + input.vertical) > 0.1f)
            {
                rot.Value = quaternion.LookRotation(moveDir, new float3(0, 1, 0));
            }

        }).ScheduleParallel();
    }
}
