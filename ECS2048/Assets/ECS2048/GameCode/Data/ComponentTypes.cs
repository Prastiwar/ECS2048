using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct PlayerMarker : IComponentData { }
public struct BlockMarker : IComponentData { public int Value; }
public struct TextUI : IComponentData { public int Index; }
public struct Removable : IComponentData { public bool1 Destroy; }
public struct FloorMarker : IComponentData { public float3 Pos; public bool1 IsFree; }
public struct ScoreHolder : IComponentData { public int Score; }

public struct Input : IComponentData
{
    public float2 NextStepValue;
}
