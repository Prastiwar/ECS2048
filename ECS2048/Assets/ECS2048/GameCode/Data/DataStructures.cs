using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public struct MoveDirection
{
    public const int Up = 0;
    public const int Left = 1;
    public const int Down = 2;
    public const int Right = 3;
}

public struct FloorData
{
    public int Length;
    public ComponentDataArray<FloorMarker> marker;
}

public struct TextData
{
    public int Length;
    [ReadOnly] public ComponentDataArray<TextUI> text;
    [ReadOnly] public ComponentDataArray<BlockMarker> marker;
    [ReadOnly] public ComponentDataArray<Position> position;
}

public struct SpawnData
{
    [ReadOnly] public ComponentDataArray<Input> input;
}

public struct BlockData
{
    public int Length;
    public ComponentDataArray<BlockMarker> marker;
}

public struct RemovalData
{
    public int Length;
    [ReadOnly] public ComponentDataArray<Removable> tag;
    [ReadOnly] public EntityArray entity;
}

public struct TextRemovalData
{
    public int Length;
    [ReadOnly] public ComponentDataArray<TextUI> text;
    [ReadOnly] public ComponentDataArray<Removable> tag;
    [ReadOnly] public EntityArray entity;
}

public struct BlockCollisionData
{
    public int Length;
    [ReadOnly] public ComponentDataArray<Position> position;
    public ComponentDataArray<BlockMarker> marker;
    public ComponentDataArray<Removable> removeTag;
    public ComponentDataArray<Input> input;
}
