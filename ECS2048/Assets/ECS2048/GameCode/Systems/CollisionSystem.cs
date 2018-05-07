using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(BorderCollisionSystem))]
public class CollisionSystem : JobComponentSystem
{
    [Inject] BlockCollisionData data;

    struct Job : IJobParallelFor
    {
        public BlockCollisionData data;

        public void Execute(int index)
        {
            var actualBlockPos = data.position[index];
            var inputVal = data.input[index];

            var nextBlockPos = actualBlockPos.Value;
            nextBlockPos.x += inputVal.Direction.x;
            nextBlockPos.y += inputVal.Direction.y;

            // Checked for border collision - on different job, so input is corrected
            // Check for same next pos
            for (int i = 0; i < data.Length; i++)
            {
                if (i == index)
                    continue;

                var checkedBlockPos = data.position[i];
                var checkedNextBlockPos = checkedBlockPos.Value;
                checkedNextBlockPos.x += inputVal.Direction.x;
                checkedNextBlockPos.y += inputVal.Direction.y;

                if (actualBlockPos.Value.x == checkedNextBlockPos.x
                    && actualBlockPos.Value.y == checkedNextBlockPos.y)
                {
                    // Check for same value || correct input
                }
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new Job() {
            data = data
        };
        return job.Schedule(data.Length, 64, inputDeps);
    }
}
