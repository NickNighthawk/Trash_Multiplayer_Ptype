using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Scenes;
using Unity.Transforms;
using Unity.NetCode;

[UpdateInGroup(typeof(GhostPredictionSystemGroup))]
[UpdateAfter(typeof(NetSimpleMoveSystem))]
public class NetSubsceneLoader : SystemBase
{
    GhostPredictionSystemGroup m_GhostPredictionSystemGroup;

    SceneSystem sceneSystem;

    private SubsceneReference subsceneRef;

    protected override void OnCreate()
    {
        m_GhostPredictionSystemGroup = World.GetExistingSystem<GhostPredictionSystemGroup>();
        RequireSingletonForUpdate<EnableNetSubsceneLoading>();
        RequireSingletonForUpdate<SubsceneReference>();
        sceneSystem = World.GetOrCreateSystem<SceneSystem>();
    }
    protected override void OnUpdate()
    {
        var tick = m_GhostPredictionSystemGroup.PredictingTick;
        var deltaTime = Time.DeltaTime;

        subsceneRef = World.EntityManager.GetComponentData<SubsceneReference>(World.EntityManager
            .CreateEntityQuery(typeof(SubsceneReference)).GetSingletonEntity());

        // Update player movement
        Entities
            .WithAll<NetPlayer>()
            .ForEach((ref Translation trans, in PredictedGhostComponent prediction) =>
            {
                if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                    return;

                float loadDistance = subsceneRef.LoadDistance;

                foreach (SubScene subscene in subsceneRef.SubScenes)
                {
                    var isLoaded = sceneSystem.IsSceneLoaded(sceneSystem.GetSceneEntity(subscene.SceneGUID));

                    if (!isLoaded && math.distance(trans.Value, subscene.transform.position) <= loadDistance)
                    {
                        LoadSubScene(subscene);
                        Debug.Log("Loading SubScene " + subscene.SceneName);
                        return;
                    }

                    if(isLoaded && math.distance(trans.Value, subscene.transform.position) > loadDistance)
                    {
                        UnloadSubScene(subscene);
                        Debug.Log("Unloading SubScene " + subscene.SceneName);
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
