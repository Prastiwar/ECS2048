using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class InputSystem : JobComponentSystem
{
    [ComputeJobOptimization]
    struct Job : IJobProcessComponentData<Input>
    {
        public int moveDir;

        public void Execute(ref Input pInput)
        {
            switch (moveDir)
            {
                case MoveDirection.Up:
                    pInput.Value.y = 1;
                    break;
                case MoveDirection.Left:
                    pInput.Value.x = -1;
                    break;
                case MoveDirection.Down:
                    pInput.Value.y = -1;
                    break;
                case MoveDirection.Right:
                    pInput.Value.x = 1;
                    break;
                default:
                    pInput.Value = 0;
                    break;
            }
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new Job() {
            //moveDir = 
            moveDir = GetMoveDirection(UnityEngine.Input.inputString)
        };
        return job.Schedule(this, 2, inputDeps);
    }

    private int GetMoveDirection(string inputString)
    {
        // use keycode instead of string
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
