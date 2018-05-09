using Unity.Entities;
using Unity.Jobs;

public class RemovalBarrier : BarrierSystem { }

[UpdateAfter(typeof(BlockCollisionSystem))]
public class BlockRemovalSystem : JobComponentSystem
{
    [Inject] private RemovalBarrier barrier;
    [Inject] private BlockRemovalData data;

    struct Job : IJob
    {
        public BlockRemovalData data;
        public EntityCommandBuffer buffer;

        public void Execute()
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (data.marker[i].Destroy)
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
