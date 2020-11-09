using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Scenes;
using Unity.Transforms;
using Unity.NetCode;
using Unity.Networking.Transport;
using Unity.Collections;
using System;

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
        int networkID = -1;

        var loadTarget = Entity.Null;

        var tick = m_GhostPredictionSystemGroup.PredictingTick;

        //Get NetworkID from NetSubscene
        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
        Entities
            .WithAll<EnableNetSubsceneLoading>()
            .ForEach((Entity ent, in GhostOwnerComponent ghostOwnerComponent, in PredictedGhostComponent prediction) =>
        {
            if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                return;

            networkID = ghostOwnerComponent.NetworkId;

            loadTarget = ent;

            Debug.Log(String.Format("Player found for subscene target {0} NetworkID: {1}", ent.Index, ghostOwnerComponent.NetworkId));

            commandBuffer.RemoveComponent<EnableNetSubsceneLoading>(ent);
        }).Run();
        commandBuffer.Playback(EntityManager);

        // Only run if connected to server
        if (networkID >= 0)
        {
            Entities
                .ForEach((in NetSubscene netSubscene) =>
                {
                    Debug.Log("Loading subscenes");
                    var playerTranslation =
                        World.EntityManager.GetComponentObject<Translation>(loadTarget);

                    foreach (SubScene subscene in netSubscene.SubScenes)
                    {
                        var isLoaded = sceneSystem.IsSceneLoaded(sceneSystem.GetSceneEntity(subscene.SceneGUID));

                        if (!isLoaded && math.distance(playerTranslation.Value, subscene.transform.position) <=
                            netSubscene.LoadDistance)
                        {
                            LoadSubScene(subscene);
                            Debug.Log("Loading SubScene " + subscene.SceneName);
                            return;
                        }

                        if (isLoaded && math.distance(playerTranslation.Value, subscene.transform.position) >
                            netSubscene.LoadDistance)
                        {
                            UnloadSubScene(subscene);
                            Debug.Log("Unloading SubScene " + subscene.SceneName);
                            return;
                        }
                    }

                }).WithoutBurst().Run();

        }

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
