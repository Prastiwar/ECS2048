using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Rendering;
using UnityEngine;

namespace TP.ECS2048
{
    [UpdateAfter(typeof(SpawnSystem))]
    public class BlockLookSystem : ComponentSystem
    {
        [Inject] private BlockLookData data;

        protected override void OnUpdate()
        {
            for (int i = 0; i < data.Length; i++)
            {
                var block = data.Block[i];
                if (!block.Changed)
                    continue;

                block.Changed = false;
                data.Block[i] = block;
                int lookIndex = 0;
                int length = Bootstrap.GameSettings.BlockColors.Count;

                for (int j = 1; lookIndex < length; j++)
                {
                    if ((j * 2).IsPowOf2())
                    {
                        if (block.Value == j)
                        {
                            data.Look[i].material.SetColor("_EmissionColor", Bootstrap.GameSettings.BlockColors[lookIndex]);
                            break;
                        }
                        lookIndex++;
                    }
                }
            }
        }

    }
}
