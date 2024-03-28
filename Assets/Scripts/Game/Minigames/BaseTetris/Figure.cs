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
    [SerializeField] protected float swipeDistanceForMovement = 50f;
    [SerializeField] protected float swipeDistanceForLockingPiece = 250f;

    //Change
    private TetrisBoardBasic connectedBoard;
    private FigureData figureData;
    private Vector3Int figureTilemapPosition;
    private readonly List<Vector3Int> figureCells = new();
    private int figureRotationIndex;

    private float stepTimer;
    private float lockTimer;

    private bool isGameActive;

    #endregion

    #region Initialization

    private void Awake()
    {
        connectedBoard = GetComponent<TetrisBoardBasic>();

        connectedBoard.OnGameStarted += ConnectedBoard_OnGameStarted;
        connectedBoard.OnGameOver += ConnectedBoard_OnGameOver;
    }

    private void ConnectedBoard_OnGameOver(object sender, TetrisBoardBasic.OnGameOverEventArgs e)
    {
        isGameActive = false;
    }

    private void ConnectedBoard_OnGameStarted(object sender, EventArgs e)
    {
        isGameActive = true;
    }

    public void InitializeFigure(Vector3Int startingPosition, FigureData newFigureData)
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

    protected virtual void Start()
    {
        GameInput.Instance.OnKeyboardMoveLeft += GameInput_OnKeyboardMoveLeft;
        GameInput.Instance.OnKeyboardMoveRight += GameInput_OnKeyboardMoveRight;
        GameInput.Instance.OnKeyboardMoveDown += GameInput_OnKeyboardMoveDown;
        GameInput.Instance.OnKeyboardLockFigure += GameInput_OnKeyboardLockFigure;
        GameInput.Instance.OnKeyboardRotateLeft += GameInput_OnKeyboardRotateLeft;
        GameInput.Instance.OnKeyboardRotateRight += GameInput_OnKeyboardRotateRight;

        GameInput.Instance.OnScreenSwipe += GameInput_OnScreenSwipe;
    }

    private void GameInput_OnScreenSwipe(object sender, GameInput.OnScreenSwipeEventArgs e)
    {
        if (!isGameActive) return;

        PerformActionBasedOnSwipe(e.swipeVector);
    }

    private void PerformActionBasedOnSwipe(Vector2 swipeVector)
    {
        Debug.Log($"{swipeVector.x} {swipeVector.y}");

        var hardLockMultiplier = 10;
        var swipeDirection = swipeVector.x > 0 && swipeVector.x > swipeDistanceForMovement ? Vector2Int.right :
            swipeVector.x < 0 && -swipeVector.x > swipeDistanceForMovement ? Vector2Int.left :
            swipeVector.y < 0 && -swipeVector.y > swipeDistanceForLockingPiece ? Vector2Int.down * hardLockMultiplier :
            swipeVector.y < 0 && -swipeVector.y > swipeDistanceForMovement ? Vector2Int.down : Vector2Int.zero;

        if (swipeDirection != Vector2Int.zero)
        {
            if (swipeDirection == Vector2Int.down * hardLockMultiplier)
            {
                HardDrop();
                return;
            }

            if (swipeDirection == Vector2Int.down && !isFallingDown) return;

            Move(swipeDirection);
        }
        else
        {
            var rotationDirection = swipeVector.x < 0 ? -1 : 1;
            Rotate(rotationDirection);
        }
    }

    private void GameInput_OnKeyboardMoveLeft(object sender, EventArgs e)
    {
        if (!isGameActive) return;

        Move(Vector2Int.left);
    }

    private void GameInput_OnKeyboardMoveRight(object sender, EventArgs e)
    {
        if (!isGameActive) return;

        Move(Vector2Int.right);
    }

    private void GameInput_OnKeyboardMoveDown(object sender, EventArgs e)
    {
        if (!isGameActive) return;
        if (!isFallingDown) return;

        Move(Vector2Int.down);
    }

    private void GameInput_OnKeyboardLockFigure(object sender, EventArgs e)
    {
        if (!isGameActive) return;

        HardDrop();
    }

    private void GameInput_OnKeyboardRotateLeft(object sender, EventArgs e)
    {
        if (!isGameActive) return;

        Rotate(-1);
    }

    private void GameInput_OnKeyboardRotateRight(object sender, EventArgs e)
    {
        if (!isGameActive) return;

        Rotate(1);
    }

    #endregion

    #region Update & Temp movement

    private void Update()
    {
        if (!isGameActive) return;

        lockTimer -= Time.deltaTime;

        if (isFallingDown)
            stepTimer -= Time.deltaTime;

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

    public Tile GetFigureRequiredTile()
    {
        return figureData.requiredFigureTile;
    }

    public Tile GetFigureForbiddenTile()
    {
        return figureData.forbiddenFigureTile;
    }

    #endregion
}
