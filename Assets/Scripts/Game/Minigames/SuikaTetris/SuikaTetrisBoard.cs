using System;
using UnityEngine;

public class SuikaTetrisBoard : TetrisBoardBasic
{
    public static event EventHandler<OnScoreChangedEventArgs> OnScoreChanged;

    public class OnScoreChangedEventArgs : EventArgs
    {
        public int newScore;
    }

    [SerializeField] private int minWinScore = 30;

    [SerializeField] private int scoreForDroppedFigure = 1;
    [SerializeField] private int scoreForCombinedFigure = 3;

    private int currentScore;

    protected override void Start()
    {
        base.Start();

        SuikaTetrisController.Instance.OnGameStarted += SuikaTetrisController_OnSuikaTetrisGameStart;
    }

    private void SuikaTetrisController_OnSuikaTetrisGameStart(object sender, EventArgs e)
    {
        StartGame();
    }

    protected override void StartGame()
    {
        base.StartGame();

        currentScore = 0;
        OnScoreChanged?.Invoke(this, new OnScoreChangedEventArgs
        {
            newScore = currentScore
        });
    }

    protected override void SetFigure(Figure figure, bool isFinalPosition)
    {
        if (isFinalPosition)
            TryCombineFigures(figure);
        else
            base.SetFigure(figure, isFinalPosition);
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
            currentScore += scoreForCombinedFigure;

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
            currentScore += scoreForDroppedFigure;

            foreach (var figureCellTilePosition in figureCells)
            {
                var boardTilePosition = figureCellTilePosition + figurePosition;
                boardTilemap.SetTile(boardTilePosition, figureTile);
            }
        }

        OnScoreChanged?.Invoke(this, new OnScoreChangedEventArgs
        {
            newScore = currentScore
        });
    }

    public override bool GetGameResult()
    {
        return currentScore >= minWinScore;
    }
}
