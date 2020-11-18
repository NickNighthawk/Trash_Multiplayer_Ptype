using System;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Scenes;
using Unity.Transforms;
using Unity.NetCode;
using UnityEngine.Rendering;
using Unity.Collections;

[UpdateInGroup(typeof(ClientSimulationSystemGroup))]
[AlwaysSynchronizeSystem]
public class NetSubsceneLoader : SystemBase
{
    SceneSystem sceneSystem;
    GhostPredictionSystemGroup m_GhostPredictionSystemGroup;
    //ClientSimulationSystemGroup m_ClientSimulationSystemGroup;

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<NetworkIdComponent>();
        sceneSystem = World.GetOrCreateSystem<SceneSystem>();
        m_GhostPredictionSystemGroup = World.GetExistingSystem<GhostPredictionSystemGroup>();
        //m_ClientSimulationSystemGroup = World.GetExistingSystem<ClientSimulationSystemGroup>();
    }
    protected override void OnUpdate()
    {
        var tick = m_GhostPredictionSystemGroup.PredictingTick;
        //var tick = m_ClientSimulationSystemGroup.InterpolationTick;
        var deltaTime = Time.DeltaTime;

        float3 playerPosition = float3.zero;

        Entities
            .WithNone<NetPlayer>()
            .WithAll<NetPlayerControllerComponent>()
            .ForEach((Entity e, ref Translation trans, in PredictedGhostComponent prediction) =>
            {
                if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                    return;

                playerPosition = trans.Value;

            }).Run();


        Entities
            .WithAll<EnableNetSubsceneLoading>()
            .ForEach((Entity e, in NetSubscene netSubscene, in PredictedGhostComponent prediction) =>
            {
                if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                    return;

                //Debug.Log("NetSubscene");
                foreach (SubScene subscene in netSubscene.SubScenes)
                {
                    var isLoaded = sceneSystem.IsSceneLoaded(sceneSystem.GetSceneEntity(subscene.SceneGUID));

                    //check if any player is in range
                    float distanceToPlayer = math.distance(playerPosition, subscene.transform.position);
                    bool shouldBeLoaded = distanceToPlayer <= netSubscene.LoadDistance;
                    Debug.Log(String.Format("SubSceneLoading: Scene {0} distance from player {1} should be loaded {2} is loaded {3}", subscene.SceneName, distanceToPlayer, shouldBeLoaded, isLoaded));

                    //if a player is in range & scene isn't loaded, load it
                    if (!isLoaded && shouldBeLoaded)
                    {
                        LoadSubScene(subscene);
                        Debug.Log(String.Format("SubSceneLoading: Loading subscene {0}", subscene.SceneName));
                        return;
                    }

                    //if no player is in range & scene is loaded, unload it
                    if(isLoaded && !shouldBeLoaded)
                    {
                        UnloadSubScene(subscene);
                        Debug.Log(String.Format("SubSceneLoading: Unloading subscene {0} ", subscene.SceneName));
                        return;
                    }
                }

            }).WithoutBurst().WithStructuralChanges().Run();
    }

    private void LoadSubScene(SubScene subScene)
    {
        sceneSystem.LoadSceneAsync(subScene.SceneGUID);
    }

    private void UnloadSubScene(SubScene subScene)
    {
        sceneSystem.UnloadScene(subScene.SceneGUID);
    }
}
