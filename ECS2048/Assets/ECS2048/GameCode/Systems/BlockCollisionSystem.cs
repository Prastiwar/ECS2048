using Unity.Entities;
using Unity.Jobs;

[UpdateAfter(typeof(BorderCollisionSystem))]
public class BlockCollisionSystem : JobComponentSystem
{
    [Inject] BlockCollisionData data;

    struct Job : IJobParallelFor
    {
        public BlockCollisionData data;

        public void Execute(int index)
        {
            var actualInputVal = data.input[index];
            var actualBlock = data.marker[index];
            var nextBlockPos = data.position[index].Value;
            nextBlockPos.x += actualInputVal.Direction.x;
            nextBlockPos.y += actualInputVal.Direction.y;
            
            for (int i = 0; i < data.Length; i++)
            {
                if (index == i)
                    continue;
                
                var checkedBlock = data.marker[i];
                var checkedNextBlockPos = data.position[i].Value;
                checkedNextBlockPos.x += data.input[i].Direction.x;
                checkedNextBlockPos.y += data.input[i].Direction.y;

                if (nextBlockPos.x == checkedNextBlockPos.x
                    && nextBlockPos.y == checkedNextBlockPos.y)
                {
                    if (actualBlock.Value == checkedBlock.Value)
                    {
                        checkedBlock.Destroy = true;
                        actualBlock.Value *= 2;

                        data.marker[i] = checkedBlock;
                        data.marker[index] = actualBlock;
                    }
                    else
                    {
                        actualInputVal.Direction = 0;
                        data.input[index] = actualInputVal;
                        data.input[i] = actualInputVal;
                    }
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
