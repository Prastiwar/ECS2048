using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace TP.ECS2048
{
    [UpdateBefore(typeof(MoveSystem))]
    public class InputSystem : JobComponentSystem
    {
        [ComputeJobOptimization]
        struct Job : IJobProcessComponentData<Input>
        {
            public bool4 keysDown;

            public void Execute(ref Input pInput)
            {
                int dir = 0;

                if (keysDown.x)
                    dir = MoveDirection.Up;
                else if (keysDown.y)
                    dir = MoveDirection.Down;
                else if (keysDown.z)
                    dir = MoveDirection.Left;
                else if (keysDown.w)
                    dir = MoveDirection.Right;

                pInput.Direction = dir;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new Job() {
                keysDown = new bool4(UnityEngine.Input.GetKeyDown(KeyCode.W),
                                     UnityEngine.Input.GetKeyDown(KeyCode.S),
                                     UnityEngine.Input.GetKeyDown(KeyCode.A),
                                     UnityEngine.Input.GetKeyDown(KeyCode.D))
            };
            return job.Schedule(this, 4, inputDeps);
        }
    }
}
