using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct PlayerMarker : IComponentData { }
public struct TextUI : IComponentData { public int Index; }
public struct FloorMarker : IComponentData { }
public struct ScoreHolder : IComponentData { public int Score; }
public struct Input : IComponentData { public int Direction; }

public struct MoveDirection
{
    public const int Up = 2;
    public const int Left = -1;
    public const int Down = -2;
    public const int Right = 1;
}

public struct Block : IComponentData
{
    public int Value;
    public int SelfIndex;
    public int2 PosIndex;
    public int2 NextBlockIndex;
    public int2 PrevBlockIndex;

    public static Block Null {
        get {
            return new Block() {
                SelfIndex = -1,
                NextBlockIndex = -1,
                PrevBlockIndex = -1,
                Value = -1,
                PosIndex = -1
            };
        }
    }
}
