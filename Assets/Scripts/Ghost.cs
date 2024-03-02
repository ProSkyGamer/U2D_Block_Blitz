using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
    [SerializeField] private Tile ghostTile;

    [FormerlySerializedAs("followingTetrisBasic")] [FormerlySerializedAs("followingBoard")]
    [SerializeField] private TetrisBoardBasic followingTetrisBoardBasic;

    [FormerlySerializedAs("trackingPiece")] [SerializeField] private Figure trackingFigure;

    private Tilemap ghostTilemap;
    private Vector3Int[] ghostCells;
    private Vector3Int ghostPiecePosition;

    private void Awake()
    {
        ghostTilemap = GetComponentInChildren<Tilemap>();
        ghostCells = new Vector3Int[4];
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
        foreach (var ghostCellPosition in ghostCells)
        {
            var tilePosition = ghostCellPosition + ghostPiecePosition;
            ghostTilemap.SetTile(tilePosition, null);
        }
    }

    private void Copy()
    {
        for (var i = 0; i < ghostCells.Length; i++) ghostCells[i] = trackingFigure.GetFigureCells()[i];
    }

    private void Drop()
    {
        var position = trackingFigure.GetFigurePosition();

        var currentRow = position.y;
        var bottom = -followingTetrisBoardBasic.GetBoardSize().y / 2 - 1;

        followingTetrisBoardBasic.ClearPiece(trackingFigure);

        for (var i = currentRow; i >= bottom; i--)
        {
            position.y = i;

            if (followingTetrisBoardBasic.IsValidPosition(trackingFigure, position))
                ghostPiecePosition = position;
            else
                break;
        }

        followingTetrisBoardBasic.SetPiece(trackingFigure, false);
    }

    private void Set()
    {
        for (var i = 0; i < ghostCells.Length; i++)
        {
            var tilePosition = ghostCells[i] + ghostPiecePosition;
            ghostTilemap.SetTile(tilePosition, ghostTile);
        }
    }
}
