using System;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Scenes;
using Unity.Transforms;
using Unity.NetCode;
using UnityEngine.Rendering;
using Unity.Collections;
using VertexFragment;
using Hash128 = Unity.Entities.Hash128;

[UpdateInGroup(typeof(GhostPredictionSystemGroup))]
[AlwaysSynchronizeSystem]
public class NetSubsceneLoader : SystemBase
{
    SceneSystem sceneSystem;
    private EntityManager entityManager;
    GhostPredictionSystemGroup m_GhostPredictionSystemGroup;
    //ClientSimulationSystemGroup m_ClientSimulationSystemGroup;

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<NetworkIdComponent>();
        sceneSystem = World.GetExistingSystem<SceneSystem>();
        entityManager = World.EntityManager;
        m_GhostPredictionSystemGroup = World.GetExistingSystem<GhostPredictionSystemGroup>();
        //m_ClientSimulationSystemGroup = World.GetExistingSystem<ClientSimulationSystemGroup>();
    }
    protected override void OnUpdate()
    {
        var tick = m_GhostPredictionSystemGroup.PredictingTick;
        //var tick = m_ClientSimulationSystemGroup.InterpolationTick;
        var deltaTime = Time.DeltaTime;

        float3 playerPosition = float3.zero;
        bool newPosition = false;

        Entities
            .WithAll<CharacterControllerComponent>()
            .ForEach((Entity e, ref Translation trans, in PredictedGhostComponent prediction) =>
            {
                if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                    return;

                playerPosition = trans.Value;
                newPosition = true;

            }).Run();

        if (!newPosition) return;

        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        Entities
            .WithAll<EnableNetSubsceneLoading>()
            .ForEach((Entity e, in NetSubscene netSubscene, in PredictedGhostComponent prediction) =>
            {
                if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                    return;

                //Debug.Log("NetSubscene");
                foreach (SubScene subscene in netSubscene.SubScenes)
                {
                    var sceneEntity = sceneSystem.GetSceneEntity(subscene.SceneGUID);
                    if(HasComponent<SubSceneLoadRequest>(sceneEntity) || HasComponent<SubSceneUnloadRequest>(sceneEntity)) return;

                    var isLoaded = HasComponent<SubsceneLoaded>(sceneEntity);

                    //check if any player is in range
                    float distanceToPlayer = math.distance(playerPosition, subscene.transform.position);
                    bool shouldBeLoaded = distanceToPlayer <= netSubscene.LoadDistance;
                    //Debug.Log(String.Format("SubSceneLoading: Scene {0} distance from player {1} should be loaded {2} is loaded {3}", subscene.SceneName, distanceToPlayer, shouldBeLoaded, isLoaded));

                    //if a player is in range & scene isn't loaded, load it
                    if (!isLoaded && shouldBeLoaded)
                    {
                        commandBuffer.AddComponent(sceneEntity, new SubSceneLoadRequest(){SceneHash = subscene.SceneGUID});

                        //Debug.Log(String.Format("SubSceneLoading: adding SubSceneLoadRequest {0}", subscene.SceneName));
                        continue;
                    }

                    //if no player is in range & scene is loaded, unload it
                    if(isLoaded && !shouldBeLoaded)
                    {
                        commandBuffer.AddComponent(sceneEntity, new SubSceneUnloadRequest(){SceneHash = subscene.SceneGUID});
                        //Debug.Log(String.Format("SubSceneLoading: adding SubSceneUnloadRequest {0} ", subscene.SceneName));
                        continue;
                    }
                }

            }).WithoutBurst().Run();
        commandBuffer.Playback(EntityManager);

    }

}

public struct SubSceneLoadRequest : IComponentData {
    public Hash128 SceneHash;
}

public struct SubsceneLoaded : IComponentData
{
}

public struct SubSceneUnloadRequest : IComponentData {
    public Hash128 SceneHash;
}



[AlwaysUpdateSystem]
[UpdateInGroup(typeof(SceneSystemGroup))]
[UpdateBefore(typeof(SceneSystem))]
public class SubSceneRequestSystem : SystemBase {
    SceneSystem SceneSystem;

    protected override void OnCreate() {
        SceneSystem = World.GetExistingSystem<SceneSystem>();
    }

    protected override void OnUpdate() {
        Entities
            .ForEach((Entity e, ref SubSceneLoadRequest loadRequest) => {
                SceneSystem.LoadSceneAsync(loadRequest.SceneHash, new SceneSystem.LoadParameters { Flags = SceneLoadFlags.LoadAdditive });
                EntityManager.AddComponent<SubsceneLoaded>(e);
                //Debug.Log(String.Format("SubSceneRequestSystem: loading subscene {0} ", SceneSystem.GetSceneEntity(loadRequest.SceneHash).Index));
                EntityManager.RemoveComponent<SubSceneLoadRequest>(e);
            })
            .WithStructuralChanges()
            .WithoutBurst()
            .Run();

        Entities
            .ForEach((Entity e, ref SubSceneUnloadRequest unloadRequest) =>
            {
                EntityManager.RemoveComponent<SubsceneLoaded>(e);
                SceneSystem.UnloadScene(unloadRequest.SceneHash);
                //Debug.Log(String.Format("SubSceneRequestSystem: unloading subscene {0} ", SceneSystem.GetSceneEntity(unloadRequest.SceneHash).Index));
                EntityManager.RemoveComponent<SubSceneUnloadRequest>(e);
            })
            .WithStructuralChanges()
            .WithoutBurst()
            .Run();
    }
}
