using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using Unity.NetCode;

[UpdateInGroup(typeof(GhostPredictionSystemGroup))]
public class NetMovableSystem : SystemBase
{
    GhostPredictionSystemGroup m_GhostPredictionSystemGroup;

    protected override void OnCreate()
    {
        m_GhostPredictionSystemGroup = World.GetExistingSystem<GhostPredictionSystemGroup>();
    }

    protected override void OnUpdate()
    {
        var tick = m_GhostPredictionSystemGroup.PredictingTick;

        Entities
            .WithAll<NetPlayer>()
            .ForEach((ref PhysicsVelocity physVel, ref PhysicsMass physMass, ref Rotation rot, in Movable mov, in PredictedGhostComponent prediction) =>
        {
            if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                return;

            var step = mov.direction * mov.speed;
            physVel.Linear += step;
            physMass.InverseInertia.x = 0f;
            physMass.InverseInertia.z = 0f;
        }).ScheduleParallel();
    }
}
