using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class BuildingTetrisBoard : TetrisBoardBasic
{
    #region Created Classes

    [Serializable]
    private class Zone
    {
        public Vector3Int[] zoneTiles;
    }

    #endregion

    #region References & Variables

    [SerializeField] private List<Zone> allRequiredZoneTypes;
    [SerializeField] private Tile requiredZoneTile;
    [SerializeField] private List<Zone> allForbiddenZoneTypes;
    [SerializeField] private Tile forbiddenZoneTile;
    [SerializeField] private int maxTileInForbiddenZone = 1;
    private int leftTilesInForbiddenZone;
    private int allRequiredZoneTilesCount;
    private int leftRequiredZoneTilesCount;

    [SerializeField] private Tilemap requiredAndForbiddenZonesTilemap;

    #endregion

    #region Intitialization

    protected override void Start()
    {
        base.Start();

        BuildTetrisController.Instance.OnGameStarted +=
            BuildingBuildingTetrisControllerOnBuildingTetrisGameStart;
    }

    private void BuildingBuildingTetrisControllerOnBuildingTetrisGameStart(object sender, EventArgs e)
    {
        StartGame();
    }

    #endregion

    #region Game Start

    protected override void StartGame()
    {
        base.StartGame();

        requiredAndForbiddenZonesTilemap.ClearAllTiles();

        InitializeZonesOnTilemap();

        leftTilesInForbiddenZone = maxTileInForbiddenZone;
    }

    private void InitializeZonesOnTilemap()
    {
        var chosenRequiredZone = ChooseRandomRequiredZoneAndPosition(out var requiredZoneTilemapPosition);

        foreach (var zoneTilePosition in chosenRequiredZone.zoneTiles)
        {
            var zoneTilemapTilePosition = requiredZoneTilemapPosition + zoneTilePosition;

            requiredAndForbiddenZonesTilemap.SetTile(zoneTilemapTilePosition, requiredZoneTile);
        }

        allRequiredZoneTilesCount = chosenRequiredZone.zoneTiles.Length;
        leftRequiredZoneTilesCount = allRequiredZoneTilesCount;

        var chosenForbiddenZone = ChooseRandomForbiddenZoneAndPosition(out var forbiddenZoneTilemapPosition);

        foreach (var zoneTilePosition in chosenForbiddenZone.zoneTiles)
        {
            var zoneTilemapTilePosition = forbiddenZoneTilemapPosition + zoneTilePosition;

            requiredAndForbiddenZonesTilemap.SetTile(zoneTilemapTilePosition, forbiddenZoneTile);
        }
    }

    private Zone ChooseRandomRequiredZoneAndPosition(out Vector3Int zoneTilemapPosition)
    {
        var randomRequiredZoneIndex = Random.Range(0, allRequiredZoneTypes.Count);
        var chosenRandomZone = allRequiredZoneTypes[randomRequiredZoneIndex];

        var zoneXPosition = Random.Range(FieldBounds.xMin, FieldBounds.xMax);
        var zoneYPosition = Random.Range(FieldBounds.yMin, FieldBounds.yMax);
        zoneTilemapPosition = new Vector3Int(zoneXPosition, zoneYPosition);

        var isRequiredZonePositionCorrect = false;
        var alreadyUsedTilemapPosition = new List<Vector3Int>();
        while (!isRequiredZonePositionCorrect)
        {
            while (alreadyUsedTilemapPosition.Contains(zoneTilemapPosition))
            {
                var newZoneXPosition = Random.Range(FieldBounds.xMin, FieldBounds.xMax);
                var newZoneYPosition = Random.Range(FieldBounds.yMin, FieldBounds.yMax);
                var newZoneTilemapPosition = new Vector3Int(newZoneXPosition, newZoneYPosition);

                zoneTilemapPosition = newZoneTilemapPosition;
            }

            alreadyUsedTilemapPosition.Add(zoneTilemapPosition);

            isRequiredZonePositionCorrect = true;
            foreach (var zoneTilePosition in chosenRandomZone.zoneTiles)
            {
                var zoneTilemapTilePosition = zoneTilemapPosition + zoneTilePosition;

                if (!FieldBounds.Contains((Vector2Int)zoneTilemapTilePosition))
                {
                    isRequiredZonePositionCorrect = false;
                    break;
                }
            }
        }

        return chosenRandomZone;
    }

    private Zone ChooseRandomForbiddenZoneAndPosition(out Vector3Int zoneTilemapPosition)
    {
        var randomRequiredZoneIndex = Random.Range(0, allForbiddenZoneTypes.Count);
        var chosenRandomZone = allForbiddenZoneTypes[randomRequiredZoneIndex];
        var isForbiddenZoneFits = false;
        var notFittingForbiddenZones = new List<Zone>();
        var fieldTiles = (boardSize.x - 1) * (boardSize.y - 1);

        var zoneXPosition = Random.Range(FieldBounds.xMin, FieldBounds.xMax);
        var zoneYPosition = Random.Range(FieldBounds.yMin, FieldBounds.yMax);
        zoneTilemapPosition = new Vector3Int(zoneXPosition, zoneYPosition);

        while (!isForbiddenZoneFits)
        {
            var isRequiredZonePositionCorrect = false;
            var alreadyUsedTilemapPosition = new List<Vector3Int>();

            while (!isRequiredZonePositionCorrect || alreadyUsedTilemapPosition.Count >= fieldTiles)
            {
                while (alreadyUsedTilemapPosition.Contains(zoneTilemapPosition))
                {
                    var newZoneXPosition = Random.Range(FieldBounds.xMin, FieldBounds.xMax);
                    var newZoneYPosition = Random.Range(FieldBounds.yMin, FieldBounds.yMax);
                    var newZoneTilemapPosition = new Vector3Int(newZoneXPosition, newZoneYPosition);

                    zoneTilemapPosition = newZoneTilemapPosition;
                }

                alreadyUsedTilemapPosition.Add(zoneTilemapPosition);

                isRequiredZonePositionCorrect = true;
                isForbiddenZoneFits = true;
                foreach (var zoneTilePosition in chosenRandomZone.zoneTiles)
                {
                    var zoneTilemapTilePosition = zoneTilemapPosition + zoneTilePosition;

                    if (!FieldBounds.Contains((Vector2Int)zoneTilemapTilePosition) ||
                        requiredAndForbiddenZonesTilemap.GetTile(zoneTilemapPosition) != null)
                    {
                        isRequiredZonePositionCorrect = false;
                        isForbiddenZoneFits = false;
                        break;
                    }
                }
            }

            alreadyUsedTilemapPosition.Clear();

            if (!isForbiddenZoneFits)
            {
                while (notFittingForbiddenZones.Contains(chosenRandomZone))
                {
                    randomRequiredZoneIndex = Random.Range(0, allForbiddenZoneTypes.Count);
                    chosenRandomZone = allForbiddenZoneTypes[randomRequiredZoneIndex];
                }

                notFittingForbiddenZones.Add(chosenRandomZone);
            }
        }

        return chosenRandomZone;
    }

    #endregion

    #region Figure Placement

    protected override void SetFigure(Figure figure, bool isFinalPosition)
    {
        base.SetFigure(figure, isFinalPosition);

        if (isFinalPosition)
            CheckPieceForZones(figure);
    }

    private void CheckPieceForZones(Figure figure)
    {
        var figureCells = figure.GetFigureCells();
        var figurePosition = figure.GetFigurePosition();

        foreach (var figureCellPosition in figureCells)
        {
            var boardTilePosition = figureCellPosition + figurePosition;

            if (requiredAndForbiddenZonesTilemap.HasTile(boardTilePosition))
            {
                var checkingTile = requiredAndForbiddenZonesTilemap.GetTile(boardTilePosition);
                if (checkingTile == requiredZoneTile)
                    leftRequiredZoneTilesCount--;
                else if (checkingTile == forbiddenZoneTile)
                    leftTilesInForbiddenZone--;
            }
        }

        Debug.Log($"required: {leftRequiredZoneTilesCount} forbidden: {leftTilesInForbiddenZone}");

        if (leftRequiredZoneTilesCount <= 0)
            EndGame(true);
        else if (leftTilesInForbiddenZone < 0)
            EndGame(false);
    }

    #endregion
}
