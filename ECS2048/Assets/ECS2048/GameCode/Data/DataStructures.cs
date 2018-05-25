using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public struct FloorData
{
    public int Length;
    public ComponentDataArray<FloorMarker> marker;
}

public struct TextData
{
    public int Length;
    [ReadOnly] public ComponentDataArray<TextUI> text;
    [ReadOnly] public ComponentDataArray<Block> block;
    [ReadOnly] public ComponentDataArray<Position> position;
}

public struct PlayerData
{
    [ReadOnly] public ComponentDataArray<PlayerMarker> marker;
    public ComponentDataArray<Input> input;
}

public struct BlockData
{
    public int Length;
    public ComponentDataArray<Block> Block;
}
