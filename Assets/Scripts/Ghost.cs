using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
    public Tile tile;
    public Board board;
    public Piece trackingPiece;

    public Tilemap tilemap { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        cells = new Vector3Int[4];
    }

    private void LateUpdate()
    {
        Clear();
        Copy();
        Drop();
        Set();
    }

    private void Clear()
    {
        for (var i = 0; i < cells.Length; i++)
        {
            var tilePosition = cells[i] + position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    private void Copy()
    {
        for (var i = 0; i < cells.Length; i++) cells[i] = trackingPiece.cells[i];
    }

    private void Drop()
    {
        var position = trackingPiece.Position;

        var currentRow = position.y;
        var bottom = -board.boardSize.y / 2 - 1;

        board.Clear(trackingPiece);

        for (var i = currentRow; i >= bottom; i--)
        {
            position.y = i;

            if (board.IsValidPosition(trackingPiece, position))
                this.position = position;
            else
                break;
        }

        board.Set(trackingPiece);
    }

    private void Set()
    {
        for (var i = 0; i < cells.Length; i++)
        {
            var tilePosition = cells[i] + position;
            tilemap.SetTile(tilePosition, tile);
        }
    }
}
