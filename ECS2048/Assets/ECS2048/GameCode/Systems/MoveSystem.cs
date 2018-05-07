using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class MoveSystem : JobComponentSystem
{
    struct Job : IJobProcessComponentData<PlayerInput, Position, BlockMarker>
    {
        public void Execute(ref PlayerInput input, ref Position pos, ref BlockMarker tag)
        {
            pos.Value.x += input.Direction.x;
            pos.Value.y += input.Direction.y;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new Job() {
        };
        return job.Schedule(this, 32, inputDeps);
    }
}
