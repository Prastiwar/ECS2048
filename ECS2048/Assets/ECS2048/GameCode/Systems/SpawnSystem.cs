using Unity.Entities;
using UnityEngine;

namespace TP.ECS2048
{
    [UpdateAfter(typeof(MoveSystem))]
    public class SpawnSystem : ComponentSystem
    {
        [Inject] private PlayerData playerData;
        [Inject] private BlockData blockData;
        private int maxIterations;

        internal void Initialize(int maxIterations)
        {
            this.maxIterations = maxIterations;
        }

        protected override void OnUpdate()
        {
            if (playerData.Input[0].Direction != 0)
            {
                int randValue = Random.Range(0, 2);
                int randIndex = Random.Range(0, blockData.Length);
                int i = 0;

                while (blockData.Block[randIndex].Value != 0)
                {
                    if (i >= maxIterations)
                    {
                        if (!TryGetFreeIndex(ref randIndex))
                        {
                            GameOver();
                            break;
                        }
                    }
                    randIndex = Random.Range(0, blockData.Length);
                    i++;
                }
                var newBlock = blockData.Block[randIndex];
                int startValue = randValue == 0 ? 2 : 4;
                newBlock.Value = startValue;
                newBlock.Changed = true;
                blockData.Block[randIndex] = newBlock;
            }
        }

        private bool TryGetFreeIndex(ref int randIndex)
        {
            for (int i = 0; i < blockData.Length; i++)
            {
                if (blockData.Block[i].Value == 0)
                {
                    randIndex = i;
                    return true;
                }
            }
            return false;
        }

        private void GameOver()
        {
            Bootstrap.GameOver();
        }
    }
}
