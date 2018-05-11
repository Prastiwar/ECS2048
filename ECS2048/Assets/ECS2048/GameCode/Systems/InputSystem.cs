using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class InputSystem : JobComponentSystem
{
    private readonly string horizontalAxis = "Horizontal";
    private readonly string verticalAxis = "Vertical";

    [ComputeJobOptimization]
    struct Job : IJobProcessComponentData<Input>
    {
        public int moveDir;
        public float gridGrap;
        //public bool4 canMove;

        public void Execute(ref Input pInput)
        {
            //bool vertical = (canMove.z || canMove.w);
            //bool horizontal = (canMove.x || canMove.y);
            float dir = 1 + gridGrap;

            //pInput.NextStepValue.x = horizontal && !vertical
            //    ? canMove.x ? -dir : dir // Left : Right
            //    : 0;

            //pInput.NextStepValue.y = vertical && !horizontal
            //    ? canMove.z ? -dir : dir // Down : Up
            //    : 0;


            switch (moveDir)
            {
                case MoveDirection.Up:
                    pInput.NextStepValue.y = dir;
                    break;
                case MoveDirection.Left:
                    pInput.NextStepValue.x = -dir;
                    break;
                case MoveDirection.Down:
                    pInput.NextStepValue.y = -dir;
                    break;
                case MoveDirection.Right:
                    pInput.NextStepValue.x = dir;
                    break;
                default:
                    break;
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new Job() {
            moveDir = GetMoveDirection(UnityEngine.Input.inputString),
            gridGrap = Bootstrap.GameSettings.GridGap
            //canMove = new bool4(Input.GetKeyDown(KeyCode.A),
            //                    Input.GetKeyDown(KeyCode.D),
            //                    Input.GetKeyDown(KeyCode.S),
            //                    Input.GetKeyDown(KeyCode.W))
        };
        return job.Schedule(this, 2, inputDeps);
    }

    private int GetMoveDirection(string inputString)
    {
        switch (inputString)
        {
            case "w":
                return MoveDirection.Up;
            case "a":
                return MoveDirection.Left;
            case "s":
                return MoveDirection.Down;
            case "d":
                return MoveDirection.Right;
            default:
                return -1;
        }
    }
}
