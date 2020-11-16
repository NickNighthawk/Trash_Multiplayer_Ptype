using Unity.Entities;
using Unity.NetCode;
#if UNITY_EDITOR
using Unity.NetCode.Editor;
#endif

public class NetCodeBootstrap : ClientServerBootstrap
{
    public override bool Initialize(string defaultWorldName)
    {
        var sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (sceneName == "NetSimpleMove" ||
            sceneName == "NetSimpleMove2" ||
            sceneName.StartsWith("Net"))
            return base.Initialize(defaultWorldName);

        var world = new World(defaultWorldName);
        World.DefaultGameObjectInjectionWorld = world;

        var systems = DefaultWorldInitialization.GetAllSystems(WorldSystemFilterFlags.Default);
        GenerateSystemLists(systems);

        DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(world, DefaultWorldSystems);
#if !UNITY_DOTSRUNTIME
        ScriptBehaviourUpdateOrder.AddWorldToCurrentPlayerLoop(world);
#endif
        return true;
    }
}
