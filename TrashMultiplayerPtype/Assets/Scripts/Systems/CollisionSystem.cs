using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

public class CollisionSystem : SystemBase
{
    private struct CollisionSystemJob : ICollisionEventsJob
    {
        public BufferFromEntity<CollisionBuffer> collisions;
        public void Execute(CollisionEvent collisionEvent)
        {
            //check if entity has a collision buffer
            if (collisions.HasComponent(collisionEvent.EntityA))
                collisions[collisionEvent.EntityA].Add(new CollisionBuffer() { entity = collisionEvent.EntityB });
            if (collisions.HasComponent(collisionEvent.EntityB))
                collisions[collisionEvent.EntityB].Add(new CollisionBuffer() { entity = collisionEvent.EntityA });

        }
    }


    protected override void OnUpdate()
    {
        var pw = World.GetOrCreateSystem<BuildPhysicsWorld>().PhysicsWorld;
        var sim = World.GetOrCreateSystem<StepPhysicsWorld>().Simulation;

        Entities.ForEach((DynamicBuffer<CollisionBuffer> collisions) =>
        {
            collisions.Clear(); //clears collisions every frame 
        }).Run();

        var colJobHandle = new CollisionSystemJob()
        {
            collisions = GetBufferFromEntity<CollisionBuffer>()
        }.Schedule(sim, ref pw, this.Dependency);
        colJobHandle.Complete();
    }

}
