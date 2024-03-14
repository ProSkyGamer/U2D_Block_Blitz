using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SameGameUI : MinigameBase
{
    #region Events

    public static event EventHandler OnCloseButtonPressed;

    #endregion

    #region Variables & References

    [SerializeField] private Button closeButton;

    [SerializeField] private Grid allTileButtonsGrid;
    [SerializeField] private List<Transform> allTilesButtonPrefabs;
    [SerializeField] private Vector2Int fieldSize = new(9, 9);
    [SerializeField] private Transform timerTilesGrid;
    [SerializeField] private Transform timerTilePrefab;
    private readonly List<Image> allTimerTiles = new();
    [SerializeField] private List<Sprite> timerTileSpritesPrefabs;
    private SameGameSingleTile[,] tilesField;

    private RectInt FieldBounds
    {
        get
        {
            var position = new Vector2Int(-fieldSize.x / 2, -fieldSize.y / 2);
            return new RectInt(position, fieldSize);
        }
    }

    #endregion

    #region Initialization

    private void Awake()
    {
        closeButton.onClick.AddListener(() =>
        {
            OnCloseButtonPressed?.Invoke(this, EventArgs.Empty);
            Hide();
            isGameStarted = false;
        });
    }

    private void InitializeField()
    {
        if (tilesField != null)
            for (var i = 0; i < fieldSize.y; i++)
            for (var j = 0; j < fieldSize.x; j++)
            {
                if (tilesField[i, j] == null) continue;

                Destroy(tilesField[i, j].gameObject);
            }

        tilesField = new SameGameSingleTile[fieldSize.y, fieldSize.x];

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
                var newCandyCrushSingleTile = newTileButtonTransform.GetComponent<SameGameSingleTile>();
                newCandyCrushSingleTile.SetTilePosition(new Vector2Int(currentRow, currentColumn));

                tilesField[currentRow, currentColumn] = newCandyCrushSingleTile;

                currentColumn++;
            }

            currentRow++;
            currentColumn = 0;
        }
    }

    private void InitializeTimerTiles()
    {
        var timerTilesTransform = timerTilesGrid.GetComponentsInChildren<Transform>();

        foreach (var timerTileTransform in timerTilesTransform)
        {
            if (timerTileTransform == timerTilesGrid) continue;

            Destroy(timerTileTransform.gameObject);
        }

        allTimerTiles.Clear();

        var timerTilesCount = Mathf.CeilToInt(maxGameTime / timerTileSpritesPrefabs.Count);

        for (var i = 0; i < timerTilesCount; i++)
        {
            var newTimerTileTransform = Instantiate(timerTilePrefab, timerTilesGrid);
            var newTimerTileImage = newTimerTileTransform.GetComponent<Image>();
            allTimerTiles.Add(newTimerTileImage);
        }
    }

    #endregion

    #region Start & Subscribed Events

    private void Start()
    {
        ChooseMinigameUI.OnPlayCandyCrushButtonPressed += ChooseMinigameUI_OnPlayCandyCrushButtonPressed;

        SameGameSingleTile.OnTryRemoveTile += CandyCrushSingleTile_OnTryRemoveTile;

        OnTimerChanged += SameGameUI_OnTimerChanged;
    }

    private void SameGameUI_OnTimerChanged(object sender, OnTimerChangedEventArgs e)
    {
        ChangeTimerTiles(e.newTimeInt);
    }

    private void ChooseMinigameUI_OnPlayCandyCrushButtonPressed(object sender, EventArgs e)
    {
        Show();
        StartGame();
    }

    protected override void StartGame()
    {
        base.StartGame();

        InitializeField();
        InitializeTimerTiles();
    }

    private void CandyCrushSingleTile_OnTryRemoveTile(object sender, SameGameSingleTile.OnTryRemoveTileEventArgs e)
    {
        var removingTile = e.removingTile;
        var removingTileColor = removingTile.GetTileColor();

        var removingTiles = new List<SameGameSingleTile>();
        var notCheckedRemovingTiles = new List<SameGameSingleTile>();
        notCheckedRemovingTiles.Add(removingTile);
        removingTiles.Add(removingTile);

        while (notCheckedRemovingTiles.Count > 0)
        {
            var newFoundTiles = TryGetConnectedTilesByColor(notCheckedRemovingTiles[0], removingTileColor);

            for (var i = 0; i < newFoundTiles.Count; i++)
            {
                if (!removingTiles.Contains(newFoundTiles[i])) continue;

                newFoundTiles.RemoveAt(i);
                i--;
            }

            removingTiles.AddRange(newFoundTiles);
            notCheckedRemovingTiles.AddRange(newFoundTiles);

            notCheckedRemovingTiles.RemoveAt(0);
        }

        var minRemovingTilesCount = 2;
        if (removingTiles.Count >= minRemovingTilesCount)
        {
            foreach (var thisRemovingTile in removingTiles) Destroy(thisRemovingTile.gameObject);

            var currentRow = 0;
            var currentColumn = -1;
            for (var i = FieldBounds.xMin; i < FieldBounds.xMax; i++)
            {
                for (var j = FieldBounds.yMin; j < FieldBounds.yMax; j++)
                {
                    currentColumn++;

                    if (tilesField[currentRow, currentColumn] == null) continue;

                    if (!removingTiles.Contains(tilesField[currentRow, currentColumn])) continue;

                    tilesField[currentRow, currentColumn] = null;
                }

                currentRow++;
                currentColumn = -1;
            }

            OffsetTopTiles();
            OffsetVerticalLines();
            UpdateFieldVisual();

            if (!IsAnyBreakableTilesLeft()) EndGame(IsAnyTileLeft() ? GameOverReason.Failed : GameOverReason.Win);
        }
    }

    private List<SameGameSingleTile> TryGetConnectedTilesByColor(SameGameSingleTile checkingTile,
        SameGameSingleTile.TileColor removingTileColor)
    {
        var foundTiles = new List<SameGameSingleTile>();
        var checkingTileGridPosition = checkingTile.GetTileGridPosition();

        var checkingPositionsDelta = new List<Vector2Int>
        {
            new(0, 1), new(0, -1), new(1, 0), new(-1, 0)
        };

        foreach (var checkingPositionDelta in checkingPositionsDelta)
        {
            var newCheckingTileGridPosition = checkingTileGridPosition + checkingPositionDelta;

            if (IsInGridBounds(newCheckingTileGridPosition))
            {
                var tryingTile = tilesField[newCheckingTileGridPosition.x, newCheckingTileGridPosition.y];
                if (tryingTile != null && CheckIsTileColorMatching(tryingTile, removingTileColor))
                    foundTiles.Add(tryingTile);
            }
        }

        return foundTiles;
    }

    private bool CheckIsTileColorMatching(SameGameSingleTile firstTile, SameGameSingleTile.TileColor tileColor)
    {
        return firstTile.GetTileColor() == tileColor;
    }

    private bool IsInGridBounds(Vector2Int gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.x < fieldSize.x &&
               gridPosition.y >= 0 && gridPosition.y < fieldSize.y;
    }

    #endregion

    private void ChangeTimerTiles(int newGameTimerInt)
    {
        var currentTileInt = newGameTimerInt % timerTileSpritesPrefabs.Count;
        var currentTileSprite = timerTileSpritesPrefabs[currentTileInt];
        if (currentTileInt == 0)
        {
            Destroy(allTimerTiles[0]);
            allTimerTiles.RemoveAt(0);
        }

        if (allTimerTiles.Count == 0) return;

        var currentTopTimerTile = allTimerTiles[0];

        currentTopTimerTile.sprite = currentTileSprite;
    }

    #region Offseting Tiles

    private void OffsetTopTiles()
    {
        for (var i = 0; i < fieldSize.y; i++)
        {
            var isColumnComplete = false;
            for (var j = 0; j < fieldSize.x && !isColumnComplete; j++)
            {
                if (tilesField[i, j] != null) continue;

                SameGameSingleTile closestFoundTile = null;

                for (var k = j + 1; k < fieldSize.x; k++)
                    if (tilesField[i, k] == null)
                    {
                        if (k == fieldSize.x - 1)
                        {
                            isColumnComplete = true;
                            break;
                        }
                    }
                    else
                    {
                        closestFoundTile = tilesField[i, k];
                        tilesField[i, k] = null;
                        break;
                    }

                if (closestFoundTile != null)
                {
                    tilesField[i, j] = closestFoundTile;
                    closestFoundTile.SetTilePosition(new Vector2Int(i, j));
                }
            }
        }
    }

    private void OffsetVerticalLines()
    {
        for (var i = 0; i < fieldSize.y; i++)
        {
            var isLineEmpty = true;
            for (var j = 0; j < fieldSize.x; j++)
                if (tilesField[i, j] != null)
                {
                    isLineEmpty = false;
                    break;
                }

            if (isLineEmpty)
                for (var k = i + 1; k < fieldSize.y; k++)
                {
                    var isFoundLineEmpty = true;
                    for (var j = 0; j < fieldSize.x; j++)
                        if (tilesField[k, j] != null)
                        {
                            isFoundLineEmpty = false;
                            break;
                        }

                    if (!isFoundLineEmpty)
                    {
                        for (var j = 0; j < fieldSize.x; j++)
                            if (tilesField[k, j] != null)
                            {
                                tilesField[i, j] = tilesField[k, j];
                                tilesField[k, j] = null;
                                tilesField[i, j].SetTilePosition(new Vector2Int(i, j));
                            }

                        break;
                    }

                    if (k == fieldSize.y - 1)
                        return;
                }
        }
    }

    private void UpdateFieldVisual()
    {
        var currentRow = 0;
        var currentColumn = -1;

        for (var i = FieldBounds.xMin; i < FieldBounds.xMax; i++)
        {
            for (var j = FieldBounds.yMin; j < FieldBounds.yMax; j++)
            {
                currentColumn++;

                if (tilesField[currentRow, currentColumn] == null) continue;

                var cellPosition = new Vector3Int(i, j);
                var cellWorldPosition = allTileButtonsGrid.CellToWorld(cellPosition);

                tilesField[currentRow, currentColumn].transform.position = cellWorldPosition;
            }

            currentRow++;
            currentColumn = -1;
        }
    }

    #endregion

    #region Game Ending

    private bool IsAnyBreakableTilesLeft()
    {
        for (var i = 0; i < fieldSize.y; i++)
        {
            var isColumnComplete = false;
            for (var j = 0; j < fieldSize.x && !isColumnComplete; j++)
                if (tilesField[i, j] == null)
                {
                    if (j == 0)
                        return false;
                }
                else
                {
                    var connectedTiles = TryGetConnectedTilesByColor(tilesField[i, j], tilesField[i, j].GetTileColor());
                    if (connectedTiles.Count > 0)
                        return true;
                }
        }

        return false;
    }

    private bool IsAnyTileLeft()
    {
        return tilesField[0, 0] != null;
    }

    #endregion

    #region Visual

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    #endregion
}
