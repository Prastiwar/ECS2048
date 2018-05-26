using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace TP.ECS2048
{
    [UpdateAfter(typeof(InputSystem))]
    public class MoveSystem : ComponentSystem
    {
        [Inject] private BlockData blockData;
        [Inject] private PlayerData playerData;
        [Inject] private ScoreData scoreData;

        internal void Initialize()
        {
            for (int i = 0; i < blockData.Length; i++)
            {
                var block = blockData.Block[i];
                block.SelfArrayIndex = i;
                block.NextBlockArrayIndex = GetBlockIndex(true, block.PosIndex, i);
                block.PrevBlockArrayIndex = GetBlockIndex(false, block.PosIndex, i);
                blockData.Block[i] = block;
            }
        }

        protected override void OnUpdate()
        {
            if (playerData.Input[0].Direction == MoveDirection.Left)
            {
                for (int i = 0; i < blockData.Length; i++)
                {
                    if (!TryGetBlocks(false, i, out Block block, out Block prevBlock, out Block nextBlock))
                        continue;

                    if (!TrySwitchValues(ref prevBlock, ref block))
                        TrySwitchValues(ref block, ref nextBlock);

                    Move(false, true, prevBlock, block);
                }
            }
            else if (playerData.Input[0].Direction == MoveDirection.Right)
            {
                for (int i = blockData.Length - 1; i >= 0; i--)
                {
                    if (!TryGetBlocks(false, i, out Block block, out Block prevBlock, out Block nextBlock))
                        continue;

                    if (!TrySwitchValues(ref nextBlock, ref block))
                        TrySwitchValues(ref block, ref prevBlock);

                    Move(false, false, nextBlock, block);
                }
            }
            else if (playerData.Input[0].Direction == MoveDirection.Down)
            {
                for (int i = 0; i < blockData.Length; i++)
                {
                    if (!TryGetBlocks(true, i, out Block block, out Block prevBlock, out Block nextBlock))
                        continue;

                    if (!TrySwitchValues(ref prevBlock, ref block))
                        TrySwitchValues(ref block, ref nextBlock);

                    Move(true, true, prevBlock, block);
                }
            }
            else if (playerData.Input[0].Direction == MoveDirection.Up)
            {
                for (int i = blockData.Length - 1; i >= 0; i--)
                {
                    if (!TryGetBlocks(true, i, out Block block, out Block prevBlock, out Block nextBlock))
                        continue;

                    if (!TrySwitchValues(ref nextBlock, ref block))
                        TrySwitchValues(ref block, ref prevBlock);

                    Move(true, false, nextBlock, block);
                }
            }
        }

        private int2 GetBlockIndex(bool lookForNext, int2 checkedPos, int checkedIndex)
        {
            int2 index = -1;
            int2 needPos = math.select(checkedPos - 1, checkedPos + 1, lookForNext);

            for (int i = 0; i < blockData.Length; i++)
            {
                if (i == checkedIndex)
                    continue;

                // if index is as needed, and second parametr same as checked
                if (blockData.Block[i].PosIndex.x == needPos.x && blockData.Block[i].PosIndex.y == checkedPos.y)
                    index.x = i;
                if (blockData.Block[i].PosIndex.x == checkedPos.x && blockData.Block[i].PosIndex.y == needPos.y)
                    index.y = i;
            }
            return index;
        }

        private bool TryGetBlocks(bool checkY, int i, out Block block, out Block prevBlock, out Block nextBlock)
        {
            block = blockData.Block[i];
            prevBlock = Block.Null;
            nextBlock = Block.Null;

            if (block.Value <= 0) // it pretends 0 values to be checked in loop
                return false;

            TryGetPrevAndNextBlock(checkY, block, out prevBlock, out nextBlock);
            return true;
        }

        private void TryGetPrevAndNextBlock(bool checkY, Block block, out Block prevBlock, out Block nextBlock)
        {
            int prevIndex = math.select(block.PrevBlockArrayIndex.x, block.PrevBlockArrayIndex.y, checkY);
            int nextIndex = math.select(block.NextBlockArrayIndex.x, block.NextBlockArrayIndex.y, checkY);

            prevBlock = prevIndex >= 0 ? blockData.Block[prevIndex] : Block.Null;
            nextBlock = nextIndex >= 0 ? blockData.Block[nextIndex] : Block.Null;
        }

        private bool TrySwitchValues(ref Block blockToChange, ref Block blockToCompare)
        {
            if (blockToChange.Value == blockToCompare.Value)
            {
                blockToChange.Value *= 2;
                blockToCompare.Value = 0;

                var score = scoreData.ScoreHolder[0];
                score.Value += blockToChange.Value;
                scoreData.ScoreHolder[0] = score;

                blockToChange.Changed = true;
                blockToCompare.Changed = true;

                blockData.Block[blockToChange.SelfArrayIndex] = blockToChange;
                blockData.Block[blockToCompare.SelfArrayIndex] = blockToCompare;
                return true;
            }
            return false;
        }

        private void Move(bool checkY, bool checkPrevs, Block blockToCheck, Block blockToMove)
        {
            while (blockToCheck.Value == 0)
            {
                blockToCheck.Value = blockToMove.Value;
                blockToMove.Value = 0;

                blockToCheck.Changed = true;
                blockToMove.Changed = true;
                blockData.Block[blockToCheck.SelfArrayIndex] = blockToCheck;
                blockData.Block[blockToMove.SelfArrayIndex] = blockToMove;

                int prevIndex = math.select(blockToCheck.PrevBlockArrayIndex.x, blockToCheck.PrevBlockArrayIndex.y, checkY);
                int nextIndex = math.select(blockToCheck.NextBlockArrayIndex.x, blockToCheck.NextBlockArrayIndex.y, checkY);
                bool indexIsNotValid = math.select(nextIndex, prevIndex, checkPrevs) < 0;
                if (indexIsNotValid)
                    return;

                // flip (switch) blocks - continue moving
                blockToMove = blockToCheck;
                blockToCheck = blockData.Block[checkPrevs ? prevIndex : nextIndex];
            }
        }

    }
}
