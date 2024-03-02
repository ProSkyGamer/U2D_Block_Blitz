using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class TetrisBoardBasic : MonoBehaviour
{
    public static event EventHandler OnGameOver;

    protected Tilemap boardTilemap;
    protected Figure activeFigure;
    [SerializeField] protected FigureData[] allFiguresData;
    [SerializeField] protected Vector3Int spawnPosition = new(-1, 8);
    [SerializeField] protected Vector2Int boardSize = new(10, 20);
    [SerializeField] protected bool isDeletingLines = true;

    [SerializeField] private Transform fullGameTransform;

    protected RectInt FieldBounds
    {
        get
        {
            var position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    private void Awake()
    {
        boardTilemap = GetComponentInChildren<Tilemap>();
        activeFigure = GetComponentInChildren<Figure>();

        for (var index = 0; index < allFiguresData.Length; index++) allFiguresData[index].Initialize();
    }

    protected virtual void Start()
    {
        activeFigure.OnFigureLocked += ActiveFigure_OnFigureLocked;
    }

    protected virtual void ActiveFigure_OnFigureLocked(object sender, EventArgs e)
    {
        SetPiece(activeFigure, true);

        if (isDeletingLines)
            ClearLines();

        SpawnPiece();
    }


    protected virtual void StartGame()
    {
        boardTilemap.ClearAllTiles();

        SpawnPiece();
    }

    private void SpawnPiece()
    {
        var randomIndex = Random.Range(0, allFiguresData.Length);
        var data = allFiguresData[randomIndex];

        activeFigure.Initialize(this, spawnPosition, data);
        if (IsValidPosition(activeFigure, spawnPosition))
            SetPiece(activeFigure, false);
        else
            GameOver();
    }

    protected virtual void GameOver()
    {
        OnGameOver?.Invoke(this, EventArgs.Empty);
    }

    public virtual void SetPiece(Figure figure, bool isFinalPosition)
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

    public void ClearPiece(Figure figure)
    {
        var figureCells = figure.GetFigureCells();
        var figurePosition = figure.GetFigurePosition();

        foreach (var figureCellTilePosition in figureCells)
        {
            var boardTilePosition = figureCellTilePosition + figurePosition;
            boardTilemap.SetTile(boardTilePosition, null);
        }
    }

    public bool IsValidPosition(Figure figure, Vector3Int newFigurePosition)
    {
        var figureCells = figure.GetFigureCells();

        foreach (var figureCellPosition in figureCells)
        {
            var newBoardTilePosition = figureCellPosition + newFigurePosition;

            if (!FieldBounds.Contains((Vector2Int)newBoardTilePosition)) return false;

            if (boardTilemap.HasTile(newBoardTilePosition))
                return false;
        }

        return true;
    }

    private void ClearLines()
    {
        var row = FieldBounds.yMin;

        while (row < FieldBounds.yMax)
            if (IsLineFull(row))
                LineClear(row);
            else row++;
    }

    private void LineClear(int row)
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

    public Vector2Int GetBoardSize()
    {
        return boardSize;
    }

    protected void Show()
    {
        fullGameTransform.gameObject.SetActive(true);
    }

    protected void Hide()
    {
        fullGameTransform.gameObject.SetActive(false);
    }
}
