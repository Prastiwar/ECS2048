using Unity.Entities;
using Unity.Jobs;

[UpdateAfter(typeof(BorderCollisionSystem))]
public class BlockCollisionSystem : JobComponentSystem
{
    [Inject] BlockCollisionData data;

    [ComputeJobOptimization]
    struct Job : IJobParallelFor
    {
        public BlockCollisionData data;

        public void Execute(int index)
        {
            var actualInputVal = data.input[index];
            var actualBlock = data.marker[index];
            var nextBlockPos = data.position[index].Value;
            nextBlockPos.x += actualInputVal.NextStepValue.x;
            nextBlockPos.y += actualInputVal.NextStepValue.y;
            
            for (int i = 0; i < data.Length; i++)
            {
                if (index == i)
                    continue;
                
                var checkedNextBlockPos = data.position[i].Value;
                checkedNextBlockPos.x += data.input[i].NextStepValue.x;
                checkedNextBlockPos.y += data.input[i].NextStepValue.y;

                if (nextBlockPos.x == checkedNextBlockPos.x
                    && nextBlockPos.y == checkedNextBlockPos.y)
                {
                    if (actualBlock.Value == data.marker[i].Value)
                    {
                        var checkedBlockRemoveTag = data.removeTag[i];
                        checkedBlockRemoveTag.Destroy = true;
                        actualBlock.Value *= 2;

                        data.removeTag[i] = checkedBlockRemoveTag;
                        data.marker[index] = actualBlock;
                    }
                    else
                    {
                        actualInputVal.NextStepValue = 0;
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
