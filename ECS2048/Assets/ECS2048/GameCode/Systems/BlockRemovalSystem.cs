using Unity.Entities;
using Unity.Jobs;

public class RemovalBarrier : BarrierSystem { }

[UpdateAfter(typeof(BlockCollisionSystem))]
public class BlockRemovalSystem : JobComponentSystem
{
    [Inject] private RemovalBarrier barrier;
    [Inject] private RemovalData data;

    [ComputeJobOptimization]
    struct Job : IJob
    {
        public RemovalData data;
        public EntityCommandBuffer buffer;

        public void Execute()
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (data.tag[i].Destroy)
                {
                    buffer.DestroyEntity(data.entity[i]);
                }
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new Job() {
            data = data,
            buffer = barrier.CreateCommandBuffer()
        };
        return job.Schedule(inputDeps);
    }
}
