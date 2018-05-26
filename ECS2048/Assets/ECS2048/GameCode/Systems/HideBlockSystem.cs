using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;

namespace TP.ECS2048
{
    public class HideBlockSystem : JobComponentSystem
    {
        [ComputeJobOptimization]
        public struct Job : IJobProcessComponentData<Block, Position>
        {
            public void Execute([ReadOnly]ref Block block, ref Position pos)
            {
                pos.Value.z = math.select(-1, 1, block.Value == 0);
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new Job();
            return job.Schedule(this, 32, inputDeps);
        }

    }
}
