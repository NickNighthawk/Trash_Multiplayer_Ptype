using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Scenes;
using Unity.Transforms;
using Unity.NetCode;


public class NetSubsceneLoader : ComponentSystem
{
    private SceneSystem sceneSystem;

    protected override void OnCreate()
    {
        sceneSystem = World.GetOrCreateSystem<SceneSystem>();
        RequireSingletonForUpdate<EnableNetSubsceneLoading>();
    }

    protected override void OnUpdate()
    {
        Entities
            .WithAll<NetPlayer>() //took out movable here
            .ForEach((ref Translation translation) =>
            {
                float loadDistance = SubsceneReferences.instance.loadDistance;
                foreach (SubScene subScene in SubsceneReferences.instance.subScenes)
                {
                    if (math.distance(translation.Value, subScene.transform.position) < loadDistance)
                    {
                        LoadSubScene(subScene);
                        Debug.Log("Loading SubScene " + subScene.SceneName);
                    }
                    else
                    {
                        UnloadSubScene(subScene);
                        Debug.Log("Unloading SubScene " + subScene.SceneName);
                    }
                }

            });
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
