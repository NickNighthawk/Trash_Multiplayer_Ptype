using System;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Scenes;
using Unity.Transforms;
using Unity.NetCode;
using UnityEngine.Rendering;
using Unity.Collections;

[UpdateInGroup(typeof(GhostPredictionSystemGroup))]
[UpdateAfter(typeof(NetSimpleMoveSystem))]
public class NetSubsceneLoader : SystemBase
{
    GhostPredictionSystemGroup m_GhostPredictionSystemGroup;

    SceneSystem sceneSystem;


    protected override void OnCreate()
    {
        m_GhostPredictionSystemGroup = World.GetExistingSystem<GhostPredictionSystemGroup>();
        RequireSingletonForUpdate<EnableNetSubsceneLoading>();
        sceneSystem = World.GetOrCreateSystem<SceneSystem>();

    }
    protected override void OnUpdate()
    {

        var tick = m_GhostPredictionSystemGroup.PredictingTick;
        var deltaTime = Time.DeltaTime;

        NativeList<float3> playerPositions = new NativeList<float3>(Allocator.Temp);

        Entities
            .WithAll<NetPlayer>()
            .ForEach((in Translation translation) =>
        {
            playerPositions.Add(translation.Value);
        }).Run();

        //Debug.Log(String.Format("SubSceneLoading: {0} Player positions", playerPositions.Length));

        Entities
            .ForEach((in NetSubscene netSubscene, in PredictedGhostComponent prediction) =>
            {
                if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                    return;

                foreach (SubScene subscene in netSubscene.SubScenes)
                {
                    var isLoaded = sceneSystem.IsSceneLoaded(sceneSystem.GetSceneEntity(subscene.SceneGUID));

                    var shouldBeLoaded = false;

                    //check if any player is in range
                    foreach (float3 playerPosition in playerPositions)
                    {
                        if(math.distance(playerPosition, subscene.transform.position) <=
                                           netSubscene.LoadDistance) shouldBeLoaded = true;
                    }

                    //if a player is in range & scene isn't loaded, load it
                    if (!isLoaded && shouldBeLoaded)
                    {
                        LoadSubScene(subscene);
                        //Debug.Log("Loading SubScene " + subscene.SceneName);
                        return;
                    }

                    //if no player is in range & scene is loaded, unload it
                    if(isLoaded && !shouldBeLoaded)
                    {
                        UnloadSubScene(subscene);
                        //Debug.Log("Unloading SubScene " + subscene.SceneName);
                        return;
                    }
                }

            }).WithoutBurst().WithStructuralChanges().Run();

        playerPositions.Dispose();
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
