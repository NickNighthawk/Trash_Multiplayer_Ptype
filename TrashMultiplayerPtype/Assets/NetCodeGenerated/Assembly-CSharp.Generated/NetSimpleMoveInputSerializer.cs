//THIS FILE IS AUTOGENERATED BY GHOSTCOMPILER. DON'T MODIFY OR ALTER.
using AOT;
using Unity.Burst;
using Unity.Networking.Transport;
using Unity.Entities;
using Unity.Collections;
using Unity.NetCode;
using Unity.Transforms;
using Unity.Mathematics;


namespace Assembly_CSharp.Generated
{
    public struct NetSimpleMoveInputSerializer : ICommandDataSerializer<NetSimpleMoveInput>
    {
        public void Serialize(ref DataStreamWriter writer, in NetSimpleMoveInput data)
        {
            writer.WriteInt((int) data.horizontal);
            writer.WriteInt((int) data.vertical);
            writer.WriteFloat(data.cameraPosition.x);
            writer.WriteFloat(data.cameraPosition.y);
            writer.WriteFloat(data.cameraPosition.z);
        }

        public void Deserialize(ref DataStreamReader reader, ref NetSimpleMoveInput data)
        {
            data.horizontal = (int) reader.ReadInt();
            data.vertical = (int) reader.ReadInt();
            data.cameraPosition.x = reader.ReadFloat();
            data.cameraPosition.y = reader.ReadFloat();
            data.cameraPosition.z = reader.ReadFloat();
        }

        public void Serialize(ref DataStreamWriter writer, in NetSimpleMoveInput data, in NetSimpleMoveInput baseline, NetworkCompressionModel compressionModel)
        {
            writer.WritePackedIntDelta((int) data.horizontal, (int) baseline.horizontal, compressionModel);
            writer.WritePackedIntDelta((int) data.vertical, (int) baseline.vertical, compressionModel);
            writer.WritePackedFloatDelta(data.cameraPosition.x, baseline.cameraPosition.x, compressionModel);
            writer.WritePackedFloatDelta(data.cameraPosition.y, baseline.cameraPosition.y, compressionModel);
            writer.WritePackedFloatDelta(data.cameraPosition.z, baseline.cameraPosition.z, compressionModel);
        }

        public void Deserialize(ref DataStreamReader reader, ref NetSimpleMoveInput data, in NetSimpleMoveInput baseline, NetworkCompressionModel compressionModel)
        {
            data.horizontal = (int) reader.ReadPackedIntDelta((int) baseline.horizontal, compressionModel);
            data.vertical = (int) reader.ReadPackedIntDelta((int) baseline.vertical, compressionModel);
            data.cameraPosition.x = reader.ReadPackedFloatDelta(baseline.cameraPosition.x, compressionModel);
            data.cameraPosition.y = reader.ReadPackedFloatDelta(baseline.cameraPosition.y, compressionModel);
            data.cameraPosition.z = reader.ReadPackedFloatDelta(baseline.cameraPosition.z, compressionModel);
        }
    }
    public class NetSimpleMoveInputSendCommandSystem : CommandSendSystem<NetSimpleMoveInputSerializer, NetSimpleMoveInput>
    {
        [BurstCompile]
        struct SendJob : IJobEntityBatch
        {
            public SendJobData data;
            public void Execute(ArchetypeChunk chunk, int orderIndex)
            {
                data.Execute(chunk, orderIndex);
            }
        }
        protected override void OnUpdate()
        {
            var sendJob = new SendJob{data = InitJobData()};
            ScheduleJobData(sendJob);
        }
    }
    public class NetSimpleMoveInputReceiveCommandSystem : CommandReceiveSystem<NetSimpleMoveInputSerializer, NetSimpleMoveInput>
    {
        [BurstCompile]
        struct ReceiveJob : IJobEntityBatch
        {
            public ReceiveJobData data;
            public void Execute(ArchetypeChunk chunk, int orderIndex)
            {
                data.Execute(chunk, orderIndex);
            }
        }
        protected override void OnUpdate()
        {
            var recvJob = new ReceiveJob{data = InitJobData()};
            ScheduleJobData(recvJob);
        }
    }
}
