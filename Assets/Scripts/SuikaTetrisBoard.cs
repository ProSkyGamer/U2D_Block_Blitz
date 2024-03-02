using System;
using UnityEngine;

public class SuikaTetrisBoard : TetrisBoardBasic
{
    protected override void Start()
    {
        base.Start();

        SuikaMinigameController.OnTetrisGameStart += TetrisMinigameControllerOnOnTetrisGameStart;
        SuikaMinigameController.OnTetrisGameClose += TetrisMinigameController_OnTetrisGameClose;

        Hide();
    }

    private void TetrisMinigameController_OnTetrisGameClose(object sender, EventArgs e)
    {
        Hide();
    }

    private void TetrisMinigameControllerOnOnTetrisGameStart(object sender, EventArgs e)
    {
        Show();
        StartGame();
    }

    public override void SetPiece(Figure figure, bool isFinalPosition)
    {
        if (isFinalPosition)
            TryCombineFigures(figure);
        else
            base.SetPiece(figure, isFinalPosition);
    }

    private void TryCombineFigures(Figure figure)
    {
        var figureCells = figure.GetFigureCells();
        var figurePosition = figure.GetFigurePosition();
        var figureTile = figure.GetFigureTile();

        var minPriority = -100;
        var currentChosenFigurePriority = minPriority;
        var currentConnectedTilePosition = Vector3Int.zero;

        foreach (var figureCellPosition in figureCells)
        {
            var figureCellTilemapPosition = figurePosition + figureCellPosition;

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
            foreach (var figureCellPosition in figureCells)
            {
                var figureCellTilemapPosition = figurePosition + figureCellPosition;
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
            {
                boardTilemap.SetTile(currentConnectedTilePosition, figureTile);
                boardTilemap.SetTile(newConnectPiece, figureTile);
            }
        }
        else
        {
            foreach (var figureCellTilePosition in figureCells)
            {
                var boardTilePosition = figureCellTilePosition + figurePosition;
                boardTilemap.SetTile(boardTilePosition, figureTile);
            }
        }
    }
}
