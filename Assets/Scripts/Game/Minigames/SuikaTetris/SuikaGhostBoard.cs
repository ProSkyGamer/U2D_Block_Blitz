using UnityEngine;

public class SuikaGhostBoard : GhostBoard
{
    protected override void DropGhostFigure()
    {
        base.DropGhostFigure();

        var figureTile = followingFigure.GetFigureTile();
        var boardTilemap = followingBoard.GetTilemap();

        var minPriority = -100;
        var currentChosenFigurePriority = minPriority;
        var currentConnectedTilePosition = Vector3Int.zero;

        foreach (var figureCellPosition in ghostFigureCells)
        {
            var figureCellTilemapPosition = ghostPieceTilemapPosition + figureCellPosition;

            if (boardTilemap.GetTile(figureCellTilemapPosition + new Vector3Int(1, 0)) == figureTile ||
                boardTilemap.GetTile(figureCellTilemapPosition + new Vector3Int(0, 1)) == figureTile ||
                boardTilemap.GetTile(figureCellTilemapPosition + new Vector3Int(-1, 0)) == figureTile ||
                boardTilemap.GetTile(figureCellTilemapPosition + new Vector3Int(0, -1)) == figureTile)
            {
                var newTilePriority = figureCellPosition.x - figureCellPosition.y;
                if (currentChosenFigurePriority <= newTilePriority)
                {
                    currentChosenFigurePriority = newTilePriority;
                    currentConnectedTilePosition = figureCellTilemapPosition;
                }
            }
        }

        if (currentChosenFigurePriority != minPriority)
        {
            var minPiecePriority = -100;
            var connectedPiecePriority = minPiecePriority;
            var newConnectPiece = Vector3Int.zero;
            foreach (var figureCellPosition in ghostFigureCells)
            {
                var figureCellTilemapPosition = ghostPieceTilemapPosition + figureCellPosition;
                if (figureCellTilemapPosition == currentConnectedTilePosition) continue;

                if (currentConnectedTilePosition == figureCellTilemapPosition + new Vector3Int(1, 0))
                {
                    connectedPiecePriority = 0;
                    newConnectPiece = figureCellTilemapPosition;
                }
                else if (currentConnectedTilePosition == figureCellTilemapPosition + new Vector3Int(0, -1) &&
                         connectedPiecePriority < -1)
                {
                    connectedPiecePriority = -1;
                    newConnectPiece = figureCellTilemapPosition;
                }
                else if (currentConnectedTilePosition == figureCellTilemapPosition + new Vector3Int(-1, 0) &&
                         connectedPiecePriority < -2)
                {
                    connectedPiecePriority = -2;
                    newConnectPiece = figureCellTilemapPosition;
                }
                else if (currentConnectedTilePosition == figureCellTilemapPosition + new Vector3Int(0, 1) &&
                         connectedPiecePriority < -3)
                {
                    connectedPiecePriority = -3;
                    newConnectPiece = figureCellTilemapPosition;
                }
            }

            if (connectedPiecePriority != minPiecePriority)
                for (var i = 0; i < ghostFigureCells.Count; i++)
                {
                    var ghostFigureCellTilemapPosition = ghostPieceTilemapPosition + ghostFigureCells[i];

                    if (ghostFigureCellTilemapPosition == currentConnectedTilePosition ||
                        ghostFigureCellTilemapPosition == newConnectPiece) continue;

                    ghostFigureCells.RemoveAt(i);
                    i--;
                }
        }
    }
}
