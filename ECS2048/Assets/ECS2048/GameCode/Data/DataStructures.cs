using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct PlayerData
{
    public ComponentDataArray<PlayerMarker> marker;
}

public struct BlockData
{
    public ComponentDataArray<BlockMarker> marker;
}
