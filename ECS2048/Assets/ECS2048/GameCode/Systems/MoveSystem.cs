using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

[UpdateAfter(typeof(BlockRemovalSystem))]
public class MoveSystem : JobComponentSystem
{
    struct Job : IJobProcessComponentData<PlayerInput, Position, BlockMarker>
    {
        public void Execute(ref PlayerInput input, ref Position pos, ref BlockMarker tag)
        {
            if (input.Direction.x != 0)
            {
                pos.Value.x += input.Direction.x;
            }
            if (input.Direction.y != 0)
            {
                pos.Value.y += input.Direction.y;
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return new Job().Schedule(this, 16, inputDeps);
    }
}
