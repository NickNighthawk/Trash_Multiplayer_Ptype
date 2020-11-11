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

        float3 playerPosition = Follow.instance.transform.position;

        Entities
            .ForEach((in NetSubscene netSubscene, in PredictedGhostComponent prediction) =>
            {
                if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                    return;

                foreach (SubScene subscene in netSubscene.SubScenes)
                {
                    var isLoaded = sceneSystem.IsSceneLoaded(sceneSystem.GetSceneEntity(subscene.SceneGUID));

                    if (!isLoaded && math.distance(playerPosition, subscene.transform.position) <= netSubscene.LoadDistance)
                    {
                        LoadSubScene(subscene);
                        Debug.Log("Loading SubScene " + subscene.SceneName);
                        return;
                    }

                    if(isLoaded && math.distance(playerPosition, subscene.transform.position) > netSubscene.LoadDistance)
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
