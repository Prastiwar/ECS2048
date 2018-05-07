using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class PlayerInputSystem : JobComponentSystem
{
    private string horizontalAxis = "Horizontal";
    private string verticalAxis = "Vertical";

    struct Job : IJobProcessComponentData<PlayerInput>
    {
        public bool4 canMove;

        public void Execute(ref PlayerInput pInput)
        {
            var data = pInput;

            bool vertical = (canMove.z || canMove.w);
            bool horizontal = (canMove.x || canMove.y);
            float dir = 1 + Bootstrap.GameSettings.GridGap;

            data.Direction.x = horizontal && !vertical
                ? canMove.x ? -dir : dir
                : 0;

            data.Direction.y = vertical && !horizontal
                ? canMove.z ? -dir : dir
                : 0;

            pInput = data;
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
        return job.Schedule(this, 1, inputDeps);
    }
}
