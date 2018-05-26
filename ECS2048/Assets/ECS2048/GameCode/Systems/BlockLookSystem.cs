using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Unity.Rendering;

namespace TP.ECS2048
{
    [UpdateAfter(typeof(SpawnSystem))]
    public class BlockLookSystem : ComponentSystem
    {
        [Inject] private BlockEntityData data;

        protected override void OnUpdate()
        {
            //for (int i = 0; i < data.Length; i++)
            //{
                //var entity = data.Entity[i];
                //if (data.Block[i].Value == 2)
                //{
                //    PostUpdateCommands.SetSharedComponent(entity, Bootstrap.BlockLook);
                //}
                //else if (data.Block[i].Value == 4)
                //{
                //    PostUpdateCommands.SetSharedComponent(entity, Bootstrap.BlockLook4);
                //}
                //else if (data.Block[i].Value == 8)
                //{
                //    PostUpdateCommands.SetSharedComponent(entity, Bootstrap.BlockLook8);
                //}
                //else if (data.Block[i].Value == 16)
                //{
                //    PostUpdateCommands.SetSharedComponent(entity, Bootstrap.BlockLook16);
                //}
            //}
        }

    }
}
