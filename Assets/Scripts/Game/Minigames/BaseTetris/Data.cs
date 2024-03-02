using System.Collections.Generic;
using UnityEngine;

public static class Data
{
    private static readonly float Cos = Mathf.Cos(Mathf.PI / 2f);
    private static readonly float Sin = Mathf.Sin(Mathf.PI / 2f);
    public static readonly float[] RotationMatrix = { Cos, Sin, -Sin, Cos };

    public static readonly Dictionary<FigureType, Vector2Int[]> Cells = new()
    {
        { FigureType.I, new[] { new(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1) } },
        {
            FigureType.J,
            new[] { new Vector2Int(-1, 1), new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0) }
        },
        {
            FigureType.L,
            new[] { new Vector2Int(1, 1), new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0) }
        },
        {
            FigureType.O,
            new[] { new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(0, 0), new Vector2Int(1, 0) }
        },
        {
            FigureType.S,
            new[] { new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(-1, 0), new Vector2Int(0, 0) }
        },
        {
            FigureType.T,
            new[] { new Vector2Int(0, 1), new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0) }
        },
        {
            FigureType.Z,
            new[] { new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(0, 0), new Vector2Int(1, 0) }
        }
    };

    private static readonly Vector2Int[,] WallKicksI =
    {
        { new(0, 0), new(-2, 0), new(1, 0), new(-2, -1), new(1, 2) },
        { new(0, 0), new(2, 0), new(-1, 0), new(2, 1), new(-1, -2) },
        { new(0, 0), new(-1, 0), new(2, 0), new(-1, 2), new(2, -1) },
        { new(0, 0), new(1, 0), new(-2, 0), new(1, -2), new(-2, 1) },
        { new(0, 0), new(2, 0), new(-1, 0), new(2, 1), new(-1, -2) },
        { new(0, 0), new(-2, 0), new(1, 0), new(-2, -1), new(1, 2) },
        { new(0, 0), new(1, 0), new(-2, 0), new(1, -2), new(-2, 1) },
        { new(0, 0), new(-1, 0), new(2, 0), new(-1, 2), new(2, -1) }
    };

    private static readonly Vector2Int[,] WallKicksJLOSTZ =
    {
        { new(0, 0), new(-1, 0), new(-1, 1), new(0, -2), new(-1, -2) },
        { new(0, 0), new(1, 0), new(1, -1), new(0, 2), new(1, 2) },
        { new(0, 0), new(1, 0), new(1, -1), new(0, 2), new(1, 2) },
        { new(0, 0), new(-1, 0), new(-1, 1), new(0, -2), new(-1, -2) },
        { new(0, 0), new(1, 0), new(1, 1), new(0, -2), new(1, -2) },
        { new(0, 0), new(-1, 0), new(-1, -1), new(0, 2), new(-1, 2) },
        { new(0, 0), new(-1, 0), new(-1, -1), new(0, 2), new(-1, 2) },
        { new(0, 0), new(1, 0), new(1, 1), new(0, -2), new(1, -2) }
    };

    public static readonly Dictionary<FigureType, Vector2Int[,]> WallKicks = new()
    {
        { FigureType.I, WallKicksI },
        { FigureType.J, WallKicksJLOSTZ },
        { FigureType.L, WallKicksJLOSTZ },
        { FigureType.O, WallKicksJLOSTZ },
        { FigureType.S, WallKicksJLOSTZ },
        { FigureType.T, WallKicksJLOSTZ },
        { FigureType.Z, WallKicksJLOSTZ }
    };
}
