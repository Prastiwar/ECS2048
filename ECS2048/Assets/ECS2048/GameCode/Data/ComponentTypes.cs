using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct PlayerMarker : IComponentData { }
public struct BlockMarker : IComponentData { }
public struct ScoreHolder : IComponentData { public int Value; }

public struct PlayerInput : IComponentData
{
    public float2 Direction;
}
