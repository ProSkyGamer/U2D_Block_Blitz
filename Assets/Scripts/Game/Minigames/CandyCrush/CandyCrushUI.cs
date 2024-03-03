using System.Collections.Generic;
using UnityEngine;

public class CandyCrushUI : MonoBehaviour
{
    [SerializeField] private Grid allTileButtonsGrid;
    [SerializeField] private List<Transform> allTilesButtonPrefabs;
    [SerializeField] private Vector2Int fieldSize = new(9, 9);
    private CandyCrushSingleTile[,] tilesField;

    private RectInt FieldBounds
    {
        get
        {
            var position = new Vector2Int(-fieldSize.x / 2, -fieldSize.y / 2);
            return new RectInt(position, fieldSize);
        }
    }

    private void Awake()
    {
        InitializeField();
    }

    private void InitializeField()
    {
        tilesField = new CandyCrushSingleTile[fieldSize.y, fieldSize.x];

        var currentRow = 0;
        var currentColumn = 0;

        for (var i = FieldBounds.xMin; i < FieldBounds.xMax; i++)
        {
            for (var j = FieldBounds.yMin; j < FieldBounds.yMax; j++)
            {
                var cellPosition = new Vector3Int(i, j);
                var cellWorldPosition = allTileButtonsGrid.CellToWorld(cellPosition);

                var chosenTileIndex = Random.Range(0, allTilesButtonPrefabs.Count);
                var chosenTileButton = allTilesButtonPrefabs[chosenTileIndex];

                var newTileButtonTransform = Instantiate(chosenTileButton, cellWorldPosition,
                    Quaternion.identity, allTileButtonsGrid.transform);
                var newCandyCrushSingleTile = newTileButtonTransform.GetComponent<CandyCrushSingleTile>();
                newCandyCrushSingleTile.SetTilePosition(new Vector2Int(currentRow, currentColumn));

                tilesField[currentRow, currentColumn] = newCandyCrushSingleTile;

                currentColumn++;
            }

            currentRow++;
            currentColumn = 0;
        }
    }

    private void Start()
    {
        CandyCrushSingleTile.OnTryRemoveTile += CandyCrushSingleTile_OnTryRemoveTile;
    }

    private void CandyCrushSingleTile_OnTryRemoveTile(object sender, CandyCrushSingleTile.OnTryRemoveTileEventArgs e)
    {
        var removingTile = e.removingTile;
        var removingTileColor = removingTile.GetTileColor();

        var removingTiles = new List<CandyCrushSingleTile>();
        var notCheckedRemovingTiles = new List<CandyCrushSingleTile>();
        notCheckedRemovingTiles.Add(removingTile);
        removingTiles.Add(removingTile);

        while (notCheckedRemovingTiles.Count > 0)
        {
            var checkingTile = notCheckedRemovingTiles[0];

            var checkingTileGridPosition = checkingTile.GetTileGridPosition();
            Debug.Log($"{checkingTileGridPosition}");
            var newCheckingTileGridPosition = checkingTileGridPosition + new Vector2Int(0, 1);

            if (newCheckingTileGridPosition.x >= 0 && newCheckingTileGridPosition.x < fieldSize.x &&
                newCheckingTileGridPosition.y >= 0 && newCheckingTileGridPosition.y < fieldSize.y)
            {
                var newFoundTile = tilesField[newCheckingTileGridPosition.x, newCheckingTileGridPosition.y];
                var newFoundTileColor = newFoundTile.GetTileColor();

                if (removingTileColor == newFoundTileColor && !removingTiles.Contains(newFoundTile))
                {
                    notCheckedRemovingTiles.Add(newFoundTile);
                    removingTiles.Add(newFoundTile);
                }
            }

            newCheckingTileGridPosition = checkingTileGridPosition + new Vector2Int(1, 0);

            if (newCheckingTileGridPosition.x >= 0 && newCheckingTileGridPosition.x < fieldSize.x &&
                newCheckingTileGridPosition.y >= 0 && newCheckingTileGridPosition.y < fieldSize.y)
            {
                var newFoundTile = tilesField[newCheckingTileGridPosition.x, newCheckingTileGridPosition.y];
                var newFoundTileColor = newFoundTile.GetTileColor();

                if (removingTileColor == newFoundTileColor && !removingTiles.Contains(newFoundTile))
                {
                    notCheckedRemovingTiles.Add(newFoundTile);
                    removingTiles.Add(newFoundTile);
                }
            }

            newCheckingTileGridPosition = checkingTileGridPosition + new Vector2Int(-1, 0);

            if (newCheckingTileGridPosition.x >= 0 && newCheckingTileGridPosition.x < fieldSize.x &&
                newCheckingTileGridPosition.y >= 0 && newCheckingTileGridPosition.y < fieldSize.y)
            {
                var newFoundTile = tilesField[newCheckingTileGridPosition.x, newCheckingTileGridPosition.y];
                var newFoundTileColor = newFoundTile.GetTileColor();

                if (removingTileColor == newFoundTileColor && !removingTiles.Contains(newFoundTile))
                {
                    notCheckedRemovingTiles.Add(newFoundTile);
                    removingTiles.Add(newFoundTile);
                }
            }

            newCheckingTileGridPosition = checkingTileGridPosition + new Vector2Int(0, -1);

            if (newCheckingTileGridPosition.x >= 0 && newCheckingTileGridPosition.x < fieldSize.x &&
                newCheckingTileGridPosition.y >= 0 && newCheckingTileGridPosition.y < fieldSize.y)
            {
                var newFoundTile = tilesField[newCheckingTileGridPosition.x, newCheckingTileGridPosition.y];
                var newFoundTileColor = newFoundTile.GetTileColor();

                if (removingTileColor == newFoundTileColor && !removingTiles.Contains(newFoundTile))
                {
                    notCheckedRemovingTiles.Add(newFoundTile);
                    removingTiles.Add(newFoundTile);
                }
            }

            notCheckedRemovingTiles.RemoveAt(0);
        }

        var minRemovingTilesCount = 2;
        if (removingTiles.Count >= minRemovingTilesCount)
            foreach (var thisRemovingTile in removingTiles)
                Destroy(thisRemovingTile.gameObject);
    }
}
