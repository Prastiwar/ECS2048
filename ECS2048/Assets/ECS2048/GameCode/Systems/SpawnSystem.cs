using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using System.Linq;
using UnityEngine;

[UpdateAfter(typeof(MoveSystem))]
public class SpawnSystem : ComponentSystem
{
    [Inject] private PlayerData playerData;

    protected override void OnUpdate()
    {
        if (playerData.input[0].Value.x != 0 || playerData.input[0].Value.y != 0)
        {
            var input = playerData.input[0];

            //int randIndex = Random.Range(0, length);
            //Bootstrap.CreateBlock(EntityManager, freeFloor.ElementAt(randIndex).GridIndex);

            // there is no free space - gameover probably

        }
    }
}
