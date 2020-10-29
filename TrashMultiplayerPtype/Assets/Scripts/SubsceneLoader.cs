using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Scenes;
using Unity.Transforms;


public class SubsceneLoader : ComponentSystem
{
    private SceneSystem sceneSystem;

    protected override void OnCreate()
    {
        sceneSystem = World.GetOrCreateSystem<SceneSystem>();
        RequireSingletonForUpdate<EnableSubsceneLoading>();
    }

    protected override void OnUpdate()
    {
        Entities
            .WithAll<Player>() //took out movable here
            .ForEach((ref Translation translation) =>
            {
                float loadDistance = SubsceneReferences.instance.loadDistance;
                foreach (SubScene subScene in SubsceneReferences.instance.subScenes)
                {
                    if (math.distance(translation.Value, subScene.transform.position) < loadDistance)
                    {
                        LoadSubScene(subScene);
                    }
                    else
                    {
                        UnloadSubScene(subScene);
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
