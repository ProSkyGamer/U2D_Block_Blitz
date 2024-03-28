using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingTetrisGhostBoard : GhostBoard
{
    [SerializeField] private Tile requiredGhostTile;
    [SerializeField] private Tile forbiddenGhostTile;

    [SerializeField] private Tile requiredTile;
    [SerializeField] private Tile forbiddenTile;

    [SerializeField] private Tilemap requiredAndForbiddenTilemap;

    protected override void SetGhostFigure()
    {
        base.SetGhostFigure();

        foreach (var ghostFigureCellPosition in ghostFigureCells)
        {
            var tilePosition = ghostFigureCellPosition + ghostPieceTilemapPosition;


            var requiredAndForbiddenTile = requiredAndForbiddenTilemap.GetTile(tilePosition);

            ghostTilemap.SetTile(tilePosition,
                requiredAndForbiddenTile == requiredTile ? requiredGhostTile :
                requiredAndForbiddenTile == forbiddenTile ? forbiddenGhostTile : ghostTile);
        }
    }
}
