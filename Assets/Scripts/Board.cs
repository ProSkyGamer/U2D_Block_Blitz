using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    public TetrominoData[] tetrominoes;
    public Vector3Int spawnPosition = new(-1, 8);
    public Vector2Int boardSize = new(10, 20);

    public RectInt Bounds
    {
        get
        {
            var position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    public void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();

        for (var index = 0; index < tetrominoes.Length; index++) tetrominoes[index].Initialize();
    }

    private void Start()
    {
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        var randomIndex = Random.Range(0, tetrominoes.Length);
        var data = tetrominoes[randomIndex];

        activePiece.Initialize(this, spawnPosition, data);
        if (IsValidPosition(activePiece, spawnPosition))
            Set(activePiece);
        else
            GameOver();
    }

    private void GameOver()
    {
        tilemap.ClearAllTiles();
    }

    public void Set(Piece piece)
    {
        for (var i = 0; i < piece.cells.Length; i++)
        {
            var tilePosition = piece.cells[i] + piece.Position;
            tilemap.SetTile(tilePosition, piece.Data.tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (var i = 0; i < piece.cells.Length; i++)
        {
            var tilePosition = piece.cells[i] + piece.Position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        foreach (var pieceCell in piece.cells)
        {
            var tilePosition = pieceCell + position;

            if (!Bounds.Contains((Vector2Int)tilePosition)) return false;

            if (tilemap.HasTile(tilePosition))
                return false;
        }

        return true;
    }

    public void ClearLines()
    {
        var row = Bounds.yMin;

        while (row < Bounds.yMax)
            if (IsLineFull(row))
                LineClear(row);
            else row++;
    }

    private void LineClear(int row)
    {
        for (var i = Bounds.xMin; i < Bounds.xMax; i++)
        {
            var position = new Vector3Int(i, row);

            tilemap.SetTile(position, null);
        }

        while (row < Bounds.yMax)
        {
            for (var i = Bounds.xMin; i < Bounds.xMax; i++)
            {
                var position = new Vector3Int(i, row + 1);

                var tileAbove = tilemap.GetTile(position);
                position = new Vector3Int(i, row);
                tilemap.SetTile(position, tileAbove);
            }

            row++;
        }
    }

    private bool IsLineFull(int row)
    {
        for (var i = Bounds.xMin; i < Bounds.xMax; i++)
        {
            var position = new Vector3Int(i, row);

            if (!tilemap.HasTile(position)) return false;
        }

        return true;
    }
}
