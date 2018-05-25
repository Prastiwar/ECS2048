using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using System;

[UpdateAfter(typeof(InputSystem))]
public class MoveSystem : ComponentSystem
{
    [Inject] private BlockData blockData;
    [Inject] private PlayerData playerData;

    public void SetIndexes()
    {
        for (int i = 0; i < blockData.Length; i++)
        {
            var block = blockData.Block[i];
            block.SelfIndex = i;
            block.NextBlockIndex = GetBlockIndex(true, block.PosIndex, i);
            block.PrevBlockIndex = GetBlockIndex(false, block.PosIndex, i);
            blockData.Block[i] = block;
        }
    }

    protected override void OnUpdate()
    {
        if (playerData.input[0].Value.x < 0)
        {
            // To Left - check from left to right
            for (int i = 0; i < blockData.Length; i++)
            {
                if (!TryGetBlocks(false, i, out Block block, out Block prevBlock, out Block nextBlock))
                    continue;

                if (!CheckValues(ref prevBlock, ref block))
                    CheckValues(ref block, ref nextBlock);

                Move(false, true, prevBlock, block);
            }
        }
        else if (playerData.input[0].Value.x > 0)
        {
            // To Right - check from right to left
            for (int i = blockData.Length - 1; i >= 0; i--)
            {
                if (!TryGetBlocks(false, i, out Block block, out Block prevBlock, out Block nextBlock))
                    continue;

                if (!CheckValues(ref nextBlock, ref block))
                    CheckValues(ref block, ref prevBlock);

                Move(false, false, nextBlock, block);
            }
        }
        else if (playerData.input[0].Value.y < 0)
        {
            // To Down - check from down(left) to up(right)
            for (int i = 0; i < blockData.Length; i++)
            {
                if (!TryGetBlocks(true, i, out Block block, out Block prevBlock, out Block nextBlock))
                    continue;

                if (!CheckValues(ref prevBlock, ref block))
                    CheckValues(ref block, ref nextBlock);

                Move(true, true, prevBlock, block);
            }
        }
        else if (playerData.input[0].Value.y > 0)
        {
            // To Up - check from up(right) to down(left)
            for (int i = blockData.Length - 1; i >= 0; i--)
            {
                if (!TryGetBlocks(true, i, out Block block, out Block prevBlock, out Block nextBlock))
                    continue;

                if (!CheckValues(ref nextBlock, ref block))
                    CheckValues(ref block, ref prevBlock);

                Move(true, false, nextBlock, block);
            }
        }
    }

    private int2 GetBlockIndex(bool lookForNext, int2 checkedPos, int checkedIndex)
    {
        int2 index = -1;
        int2 needPos = lookForNext ? checkedPos + 1 : checkedPos - 1;

        for (int i = 0; i < blockData.Length; i++)
        {
            if (i == checkedIndex)
                continue;

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

        if (block.Value <= 0) // pretends 0 values to be checked in loop
            return false;

        TryGetPrevAndNextBlock(checkY, block, out prevBlock, out nextBlock);
        return true;
    }

    private void TryGetPrevAndNextBlock(bool checkY, Block block, out Block prevBlock, out Block nextBlock)
    {
        int prevIndex = math.select(block.PrevBlockIndex.x, block.PrevBlockIndex.y, checkY);
        int nextIndex = math.select(block.NextBlockIndex.x, block.NextBlockIndex.y, checkY);

        prevBlock = prevIndex >= 0 ? blockData.Block[prevIndex] : Block.Null;
        nextBlock = nextIndex >= 0 ? blockData.Block[nextIndex] : Block.Null;
    }

    private bool CheckValues(ref Block blockToChange, ref Block blockToCompare)
    {
        if (blockToChange.Value == blockToCompare.Value)
        {
            blockToChange.Value *= 2;
            blockToCompare.Value = 0;

            blockData.Block[blockToChange.SelfIndex] = blockToChange;
            blockData.Block[blockToCompare.SelfIndex] = blockToCompare;
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

            blockData.Block[blockToCheck.SelfIndex] = blockToCheck;
            blockData.Block[blockToMove.SelfIndex] = blockToMove;

            int prevIndex = math.select(blockToCheck.PrevBlockIndex.x, blockToCheck.PrevBlockIndex.y, checkY);
            int nextIndex = math.select(blockToCheck.NextBlockIndex.x, blockToCheck.NextBlockIndex.y, checkY);
            bool indexIsNotValid = math.select(nextIndex, prevIndex, checkPrevs) < 0;
            if (indexIsNotValid)
                return;

            // flip (switch) blocks - continue moving
            blockToMove = blockToCheck;
            blockToCheck = blockData.Block[checkPrevs ? prevIndex : nextIndex];
        }
    }

}
