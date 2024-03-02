using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Figure : MonoBehaviour
{
    #region Events

    public event EventHandler OnFigureLocked;
    public event EventHandler OnTryingToMoveFigure;
    public event EventHandler OnFigureMoved;

    #endregion

    #region References & Variables

    [SerializeField] protected bool isFallingDown = true;
    [SerializeField] protected float stepDelay = 1f;

    [SerializeField] protected bool isHasLockDelay = true;
    [SerializeField] protected float lockDelay = 0.5f;

    //Change
    private TetrisBoardBasic connectedBoard;
    private FigureData figureData;
    private Vector3Int figureTilemapPosition;
    private readonly List<Vector3Int> figureCells = new();
    private int figureRotationIndex;

    private float stepTimer;
    private float lockTimer;

    #endregion

    #region Initialization

    private void Awake()
    {
        connectedBoard = GetComponent<TetrisBoardBasic>();
    }

    public void Initialize(Vector3Int startingPosition, FigureData newFigureData)
    {
        figureTilemapPosition = startingPosition;
        figureData = newFigureData;

        figureRotationIndex = 0;

        stepTimer = stepDelay;
        lockTimer = lockDelay;

        figureCells.Clear();
        foreach (var figureCellPosition in newFigureData.figureCells)
            figureCells.Add((Vector3Int)figureCellPosition);

        OnFigureMoved?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    #region Update & Temp movement

    private void Update()
    {
        lockTimer -= Time.deltaTime;

        if (isFallingDown)
            stepTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Q))
            Rotate(-1);
        else if (Input.GetKeyDown(KeyCode.E)) Rotate(1);

        if (Input.GetKeyDown(KeyCode.A))
            Move(Vector2Int.left);
        else if (Input.GetKeyDown(KeyCode.D)) Move(Vector2Int.right);

        if (Input.GetKeyDown(KeyCode.S))
            Step();

        if (Input.GetKeyDown(KeyCode.Space))
            HardDrop();

        if (stepTimer <= 0 && isFallingDown)
            Step();
    }

    #endregion

    #region Figure Methods

    protected void Move(Vector2Int deltaMovement)
    {
        OnTryingToMoveFigure?.Invoke(this, EventArgs.Empty);

        var newFigurePosition =
            GetMovedPosition(figureCells, figureTilemapPosition, deltaMovement, out var _);

        figureTilemapPosition = newFigurePosition;

        OnFigureMoved?.Invoke(this, EventArgs.Empty);
    }

    protected void Step()
    {
        stepTimer = stepDelay;

        OnTryingToMoveFigure?.Invoke(this, EventArgs.Empty);

        var newFigurePosition =
            GetMovedPosition(figureCells, figureTilemapPosition, Vector2Int.down, out var isMoved);

        figureTilemapPosition = newFigurePosition;

        if (!isMoved && ((lockTimer <= 0 && isHasLockDelay) || !isHasLockDelay))
            Lock();
        else
            OnFigureMoved?.Invoke(this, EventArgs.Empty);
    }

    protected void HardDrop()
    {
        OnTryingToMoveFigure?.Invoke(this, EventArgs.Empty);

        var isMoved = true;
        var lastTilemapPosition = figureTilemapPosition;
        while (isMoved)
            lastTilemapPosition = GetMovedPosition(figureCells, lastTilemapPosition, Vector2Int.down, out isMoved);

        figureTilemapPosition = lastTilemapPosition;

        Lock();
    }

    private void Lock()
    {
        OnFigureLocked?.Invoke(this, EventArgs.Empty);
    }

    protected void Rotate(int direction)
    {
        OnTryingToMoveFigure?.Invoke(this, EventArgs.Empty);

        var minRotationIndex = 0;
        var maxRotationIndex = 3;

        figureRotationIndex = figureRotationIndex + direction > maxRotationIndex ? minRotationIndex :
            figureRotationIndex + direction < minRotationIndex ? maxRotationIndex : figureRotationIndex + direction;

        var rotatedFigureCells = GetRotatedFigureCells(figureCells, direction);
        TryApplyWallKicksTest(rotatedFigureCells, figureRotationIndex, out var isSuccessful,
            out var newFigureTilemapPosition);

        if (isSuccessful)
        {
            figureCells.Clear();
            figureCells.AddRange(rotatedFigureCells);

            figureTilemapPosition = newFigureTilemapPosition;
        }

        OnFigureMoved?.Invoke(this, EventArgs.Empty);
    }

    private void TryApplyWallKicksTest(List<Vector3Int> rotatedFigureCells, int rotationIndex, out bool isSuccessful,
        out Vector3Int newFigureTilemapPosition)
    {
        isSuccessful = false;

        var wallKickIndex = GetWallKickIndex(rotationIndex);
        newFigureTilemapPosition = figureTilemapPosition;

        for (var i = 0; i < figureData.figureWallKicks.GetLength(1); i++)
        {
            var translation = figureData.figureWallKicks[wallKickIndex, i];

            newFigureTilemapPosition =
                GetMovedPosition(rotatedFigureCells, figureTilemapPosition, translation, out var isMoved);

            if (isMoved)
            {
                isSuccessful = true;
                return;
            }
        }
    }

    #endregion

    #region Get Figure Data

    private Vector3Int GetMovedPosition(List<Vector3Int> movingCells, Vector3Int oldFigureTilemapPosition,
        Vector2Int deltaMovement, out bool isMoved)
    {
        isMoved = false;
        var newFigureTilemapPosition = oldFigureTilemapPosition + (Vector3Int)deltaMovement;

        if (!connectedBoard.IsValidPosition(movingCells, newFigureTilemapPosition)) return oldFigureTilemapPosition;

        isMoved = true;
        return newFigureTilemapPosition;
    }

    private int GetWallKickIndex(int rotationIndex)
    {
        var wallKickIndex = rotationIndex * 2;
        if (rotationIndex < 0) wallKickIndex--;

        var minWallKickIndex = 0;
        var maxWallKickIndex = figureData.figureWallKicks.GetLength(0);

        return wallKickIndex > maxWallKickIndex ? minWallKickIndex :
            wallKickIndex < minWallKickIndex ? maxWallKickIndex :
            wallKickIndex;
    }

    private List<Vector3Int> GetRotatedFigureCells(List<Vector3Int> rotatingCells, int direction)
    {
        var rotatedFigure = new List<Vector3Int>();

        foreach (var rotatingCell in rotatingCells)
        {
            var cell = (Vector3)rotatingCell;

            int x, y;

            switch (figureData.figureType)
            {
                case FigureType.I:
                case FigureType.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt(cell.x * Data.RotationMatrix[0] * direction +
                                        cell.y * Data.RotationMatrix[1] * direction);
                    y = Mathf.CeilToInt(cell.x * Data.RotationMatrix[2] * direction +
                                        cell.y * Data.RotationMatrix[3] * direction);
                    break;
                default:
                    x = Mathf.RoundToInt(cell.x * Data.RotationMatrix[0] * direction +
                                         cell.y * Data.RotationMatrix[1] * direction);
                    y = Mathf.RoundToInt(cell.x * Data.RotationMatrix[2] * direction +
                                         cell.y * Data.RotationMatrix[3] * direction);
                    break;
            }

            var newCellPosition = new Vector3Int(x, y);
            rotatedFigure.Add(newCellPosition);
        }

        return rotatedFigure;
    }

    public List<Vector3Int> GetFigureCells()
    {
        return figureCells;
    }

    public Vector3Int GetFigurePosition()
    {
        return figureTilemapPosition;
    }

    public Tile GetFigureTile()
    {
        return figureData.figureTile;
    }

    #endregion
}
