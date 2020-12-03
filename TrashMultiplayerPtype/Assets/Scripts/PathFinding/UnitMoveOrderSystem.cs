using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;

public class UnitMoveOrderSystem : ComponentSystem
{
    //when click, add params component to our entity. contains start and end pos
    //entity needs a convert to entity 
    protected override void OnUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Entities.ForEach((Entity e, ref Translation translation) =>
            {
                EntityManager.AddComponentData(e, new PathFindingParams
                {
                    startPosition = new int2(0, 0),
                    endPosition = new int2(3, 4),
                });
            });
        }
    }
}
