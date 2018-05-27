using Unity.Entities;
using UnityEngine;

namespace TP.ECS2048
{
    //[UpdateBefore(typeof(HideBlockSystem))]
    public class BlockLookSystem : ComponentSystem
    {
        [Inject] private BlockEntityData data;

        protected override void OnUpdate()
        {
            for (int i = 0; i < data.Length; i++)
            {
                var block = data.Block[i];
                if (!block.Changed)
                    continue;

                block.Changed = false;
                data.Block[i] = block;
                var entity = data.Entity[i];
                int lookIndex = 0;
                int length = Bootstrap.BlockLooks.Count;

                for (int j = 1; lookIndex < length; j++)
                {
                    if ((j * 2).IsPowOf2())
                    {
                        if (data.Block[i].Value == j)
                        {
                            //PostUpdateCommands.SetSharedComponent(entity, Bootstrap.BlockLooks[lookIndex]); // bugs everything
                            break;
                        }
                        lookIndex++;
                    }
                }
            }
        }

    }
}
