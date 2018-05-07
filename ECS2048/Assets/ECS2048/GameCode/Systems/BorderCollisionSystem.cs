using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateAfter(typeof(PlayerInputSystem))]
public class BorderCollisionSystem : JobComponentSystem
{
    [Inject] private BlockCollisionData data;

    struct Job : IJobParallelFor
    {
        public BlockCollisionData data;
        public float4 border;

        public void Execute(int index)
        {
            var inputVal = data.input[index];
            var actualPos = data.position[index];
            var nextPos = actualPos.Value;
            nextPos.x += inputVal.Direction.x;
            nextPos.y += inputVal.Direction.y;

            if (nextPos.x < border.x || nextPos.x > border.z)
            {
                inputVal.Direction.x = 0;
                data.input[index] = inputVal;
            }
            if (nextPos.y < border.y || nextPos.y > border.w)
            {
                inputVal.Direction.y = 0;
                data.input[index] = inputVal;
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new Job() {
            data = data,
            border = Bootstrap.Border
        };
        return job.Schedule(data.Length, 64, inputDeps);
    }
}
