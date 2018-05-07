using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public struct PlayerData
{
    public int Length;
    public ComponentDataArray<PlayerMarker> marker;
}

public struct BlockData
{
    public int Length;
    public ComponentDataArray<BlockMarker> marker;
}

public struct BlockCollisionData
{
    public int Length;
    [ReadOnly] public ComponentDataArray<BlockMarker> marker;
    [ReadOnly] public ComponentDataArray<Position> position;
    public ComponentDataArray<PlayerInput> input;
}
