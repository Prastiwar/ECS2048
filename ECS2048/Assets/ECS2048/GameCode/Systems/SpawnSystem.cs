using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

[UpdateAfter(typeof(MoveSystem))]
public class SpawnSystem : ComponentSystem
{
    [Inject] private SpawnData spawnData;
    [Inject] private FloorData floorData;

    protected override void OnUpdate()
    {
        if (spawnData.input[0].NextStepValue.x != 0 || spawnData.input[0].NextStepValue.y != 0)
        {
            for (int i = 0; i < floorData.Length; i++)
            {
                if (floorData.marker[i].IsFree)
                {
                    var puc = PostUpdateCommands;
                    var tag = floorData.marker[i];
                    tag.IsFree = false;
                    floorData.marker[i] = tag;

                    Bootstrap.CreateBlock(EntityManager, tag.Pos);
                    return;
                }
            }

            // there are no free floor, probably game over

        }
    }
}
