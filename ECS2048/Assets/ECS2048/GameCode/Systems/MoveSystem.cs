using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

[UpdateAfter(typeof(BlockRemovalSystem))]
public class MoveSystem : JobComponentSystem
{
    [ComputeJobOptimization]
    struct Job : IJobProcessComponentData<Input, Position, BlockMarker>
    {
        public void Execute(ref Input input, ref Position pos, ref BlockMarker tag)
        {
            if (input.NextStepValue.x != 0)
            {
                pos.Value.x += input.NextStepValue.x;
            }
            if (input.NextStepValue.y != 0)
            {
                pos.Value.y += input.NextStepValue.y;
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return new Job().Schedule(this, 16, inputDeps);
    }
}
