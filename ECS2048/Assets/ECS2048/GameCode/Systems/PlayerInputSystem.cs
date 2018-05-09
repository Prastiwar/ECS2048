using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class PlayerInputSystem : JobComponentSystem
{
    private readonly string horizontalAxis = "Horizontal";
    private readonly string verticalAxis = "Vertical";

    struct Job : IJobProcessComponentData<PlayerInput>
    {
        public bool4 canMove;

        public void Execute(ref PlayerInput pInput)
        {
            bool vertical = (canMove.z || canMove.w);
            bool horizontal = (canMove.x || canMove.y);
            float dir = 1 + Bootstrap.GameSettings.GridGap;

            pInput.Direction.x = horizontal && !vertical
                ? canMove.x ? -dir : dir
                : 0;

            pInput.Direction.y = vertical && !horizontal
                ? canMove.z ? -dir : dir
                : 0;
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new Job() {
            canMove = new bool4(Input.GetKeyDown(KeyCode.A),
                                Input.GetKeyDown(KeyCode.D),
                                Input.GetKeyDown(KeyCode.S),
                                Input.GetKeyDown(KeyCode.W))
    };
        return job.Schedule(this, 2, inputDeps);
    }
}
