using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum FigureType
{
    I,
    O,
    T,
    J,
    L,
    S,
    Z
}

[Serializable]
public struct FigureData
{
    public FigureType figureType;
    public Tile figureTile;
    public Tile requiredFigureTile;
    public Tile forbiddenFigureTile;
    public Vector2Int[] figureCells;
    public Vector2Int[,] figureWallKicks;

    public void Initialize()
    {
        //figureCells = Data.Cells[figureType];
        figureWallKicks = Data.WallKicks[figureType];
    }
}
