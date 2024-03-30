using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class TetrisBoardBasic : MonoBehaviour
{
    #region Events

    public event EventHandler OnGameStarted;
    public event EventHandler<OnGameOverEventArgs> OnGameOver;

    public class OnGameOverEventArgs : EventArgs
    {
        public bool isWin;
    }

    #endregion

    #region References & Variables

    [SerializeField] private MinigameBase minigameBase;

    [SerializeField] protected FigureData[] allFiguresData;
    [SerializeField] protected Vector3Int spawnPosition = new(-1, 8);
    [SerializeField] protected Vector2Int boardSize = new(10, 20);
    [SerializeField] protected bool isDeletingLines = true;
    [SerializeField] protected bool isUsingNotSpawningTopLinesCount;
    [SerializeField] protected int notSpawningTopLinesCount = 4;

    protected Tilemap boardTilemap;
    [SerializeField] protected Tilemap nextActiveFigureTilemap;

    private FigureData previousActiveFigureData;
    private FigureData currentActiveFigureData;
    private Figure activeFigure;
    private FigureData nextActiveFigureData;

    private GhostBoard ghostBoard;

    private bool isGameActive;

    protected RectInt FieldBounds
    {
        get
        {
            var position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    protected RectInt FieldRequiredAndNeededBounds
    {
        get
        {
            var position = new Vector2Int(-boardSize.x / 2,
                (-boardSize.y + (isUsingNotSpawningTopLinesCount ? notSpawningTopLinesCount : 0)) / 2);
            var newBoardSize = new Vector2Int(boardSize.x,
                boardSize.y - (isUsingNotSpawningTopLinesCount ? notSpawningTopLinesCount * 2 : 0));
            return new RectInt(position, newBoardSize);
        }
    }

    #endregion

    #region Initialization

    private void Awake()
    {
        boardTilemap = GetComponentInChildren<Tilemap>();
        activeFigure = GetComponentInChildren<Figure>();

        for (var index = 0; index < allFiguresData.Length; index++) allFiguresData[index].Initialize();
    }

    protected virtual void Start()
    {
        activeFigure.OnFigureLocked += ActiveFigure_OnFigureLocked;

        activeFigure.OnTryingToMoveFigure += ActiveFigure_OnTryingToMoveFigure;
        activeFigure.OnFigureMoved += ActiveFigure_OnFigureMoved;

        minigameBase.OnGameOver += MinigameBase_OnGameOver;
    }

    private void MinigameBase_OnGameOver(object sender, MinigameBase.OnGameOverEventArgs e)
    {
        isGameActive = false;
    }

    private void ActiveFigure_OnFigureMoved(object sender, EventArgs e)
    {
        if (ghostBoard != null) return;

        SetFigure(activeFigure, false);
    }

    private void ActiveFigure_OnTryingToMoveFigure(object sender, EventArgs e)
    {
        ClearPiece(activeFigure);
    }

    protected virtual void ActiveFigure_OnFigureLocked(object sender, EventArgs e)
    {
        SetFigure(activeFigure, true);

        if (isDeletingLines)
            ClearFullLines();

        SpawnPiece();
    }

    #endregion

    #region Game Start & End Methods

    protected virtual void StartGame()
    {
        boardTilemap.ClearAllTiles();

        isGameActive = true;
        OnGameStarted?.Invoke(this, EventArgs.Empty);

        SpawnPiece();
    }

    protected void EndGame(bool isWin)
    {
        isGameActive = false;

        OnGameOver?.Invoke(this, new OnGameOverEventArgs
        {
            isWin = isWin
        });
    }

    #endregion

    #region Pieces Spawn

    protected virtual void SpawnPiece()
    {
        if (!isGameActive) return;

        if (nextActiveFigureTilemap != null)
            nextActiveFigureTilemap.ClearAllTiles();

        previousActiveFigureData = currentActiveFigureData;
        if (previousActiveFigureData.figureCells == null)
        {
            var currentFigureRandomIndex = Random.Range(0, allFiguresData.Length);

            currentActiveFigureData = allFiguresData[currentFigureRandomIndex];
        }
        else
        {
            currentActiveFigureData = nextActiveFigureData;
        }

        do
        {
            var nextFigureRandomIndex = Random.Range(0, allFiguresData.Length);
            nextActiveFigureData = allFiguresData[nextFigureRandomIndex];
        } while (previousActiveFigureData.figureCells != null &&
                 nextActiveFigureData.figureCells == previousActiveFigureData.figureCells);

        var newFigureCells = new List<Vector3Int>();
        foreach (var newFigureCell in currentActiveFigureData.figureCells)
            newFigureCells.Add((Vector3Int)newFigureCell);

        if (!IsValidPosition(newFigureCells, spawnPosition))
            EndGame(false);

        activeFigure.InitializeFigure(spawnPosition, currentActiveFigureData);

        if (nextActiveFigureTilemap != null)
            foreach (var nextActiveFigureDataCellPosition in nextActiveFigureData.figureCells)
                nextActiveFigureTilemap.SetTile((Vector3Int)nextActiveFigureDataCellPosition,
                    nextActiveFigureData.figureTile);
    }

    #endregion

    #region Figure Placement

    protected virtual void SetFigure(Figure figure, bool isFinalPosition)
    {
        var figureCells = figure.GetFigureCells();
        var figurePosition = figure.GetFigurePosition();
        var figureTile = figure.GetFigureTile();

        foreach (var figureCellTilePosition in figureCells)
        {
            var boardTilePosition = figureCellTilePosition + figurePosition;
            boardTilemap.SetTile(boardTilePosition, figureTile);
        }
    }

    private void ClearPiece(Figure figure)
    {
        var figureCells = figure.GetFigureCells();
        var figurePosition = figure.GetFigurePosition();

        foreach (var figureCellTilePosition in figureCells)
        {
            var boardTilePosition = figureCellTilePosition + figurePosition;
            boardTilemap.SetTile(boardTilePosition, null);
        }
    }

    public bool IsValidPosition(List<Vector3Int> figureCells, Vector3Int newFigurePosition)
    {
        foreach (var figureCellPosition in figureCells)
        {
            var newBoardTilePosition = figureCellPosition + newFigurePosition;

            if (!FieldBounds.Contains((Vector2Int)newBoardTilePosition)) return false;

            if (boardTilemap.HasTile(newBoardTilePosition))
                return false;
        }

        return true;
    }

    #endregion

    #region Clearing Lines

    private void ClearFullLines()
    {
        var row = FieldBounds.yMin;

        while (row < FieldBounds.yMax)
            if (IsLineFull(row))
                ClearLineByRowNumber(row);
            else row++;
    }

    private void ClearLineByRowNumber(int row)
    {
        for (var i = FieldBounds.xMin; i < FieldBounds.xMax; i++)
        {
            var position = new Vector3Int(i, row);

            boardTilemap.SetTile(position, null);
        }

        while (row < FieldBounds.yMax)
        {
            for (var i = FieldBounds.xMin; i < FieldBounds.xMax; i++)
            {
                var position = new Vector3Int(i, row + 1);

                var tileAbove = boardTilemap.GetTile(position);
                position = new Vector3Int(i, row);
                boardTilemap.SetTile(position, tileAbove);
            }

            row++;
        }
    }

    private bool IsLineFull(int row)
    {
        for (var i = FieldBounds.xMin; i < FieldBounds.xMax; i++)
        {
            var position = new Vector3Int(i, row);

            if (!boardTilemap.HasTile(position)) return false;
        }

        return true;
    }

    #endregion

    #region Get Board Info

    public Vector2Int GetBoardSize()
    {
        return boardSize;
    }

    #endregion

    #region Ghost Board

    public void AddFollowingGhostBoard(GhostBoard followingGhostBoard)
    {
        if (ghostBoard != null) return;

        activeFigure.OnFigureMoved -= ActiveFigure_OnFigureMoved;

        ghostBoard = followingGhostBoard;
        followingGhostBoard.OnGhostPiecePlaced += FollowingGhostBoard_OnGhostPiecePlaced;
    }

    private void FollowingGhostBoard_OnGhostPiecePlaced(object sender, EventArgs e)
    {
        SetFigure(activeFigure, false);
    }

    public Tilemap GetTilemap()
    {
        return boardTilemap;
    }

    public virtual bool GetGameResult()
    {
        return false;
    }

    #endregion
}
