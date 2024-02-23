using System.Collections.Generic;
using UnityEngine;

public class SuikaMinigameController : MonoBehaviour
{
    #region Variables & References

    [SerializeField] private Vector2Int minigameFieldSize = new(7, 7);
    [SerializeField] private Transform allBlocksHolderTransform;
    private BlockSingle[,] allFieldBlocks;
    private bool[,] allFieldBlocksBool;

    [SerializeField] private Vector2Int spawningFieldSize = new(7, 3);
    [SerializeField] private Transform spawningBlockHolderTransform;
    private BlockSingle[,] allSpawningFieldBlock;

    [SerializeField] private List<FigureSingle> allAvailableFigures = new();

    private FigureSingle currentChosenFigure;
    private Vector2Int currentChosenFigurePosition;

    #endregion

    #region Initialization

    private void Awake()
    {
        allFieldBlocksBool = new bool[minigameFieldSize.y, minigameFieldSize.x];
        InitializeField(ref allFieldBlocks, minigameFieldSize, allBlocksHolderTransform);
        InitializeField(ref allSpawningFieldBlock, spawningFieldSize, spawningBlockHolderTransform);

        currentChosenFigure = ChooseRandomFigure();
        currentChosenFigurePosition = new Vector2Int(0, spawningFieldSize.y - 1);
        UpdateSpawningFieldVisual();
    }

    private void InitializeField(ref BlockSingle[,] fieldReference, Vector2Int fieldSize,
        Transform fieldBlocksHolderTransform)
    {
        fieldReference = new BlockSingle[fieldSize.y, fieldSize.x];

        var allBlocks = fieldBlocksHolderTransform.GetComponentsInChildren<BlockSingle>();
        var allBlocksList = new List<BlockSingle>();
        allBlocksList.AddRange(allBlocks);

        if (allBlocksList.Count < fieldSize.x * fieldSize.y)
            Debug.LogError("Field contain less blocks than should!");

        for (var i = 0; i < fieldSize.y; i++)
        for (var j = 0; j < fieldSize.x; j++)
        {
            fieldReference[i, j] = allBlocksList[0];
            allBlocksList.RemoveAt(0);
        }
    }

    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            MoveCurrentChosenFigure(true);
        else if (Input.GetKeyDown(KeyCode.D))
            MoveCurrentChosenFigure(false);
        else if (Input.GetKeyDown(KeyCode.Space))
            DropCurrentChosenFigure();
        /*else if (Input.GetKeyDown(KeyCode.R))
        {
            var figureAvailableBlockCount = currentChosenFigure.GetFigureSize();

            var figureAvailableBlocks = new bool[figureAvailableBlockCount, figureAvailableBlockCount];
            
            for (int i = 0; i < figureAvailableBlockCount; i++)
            {
                for (int j = 0; j < figureAvailableBlockCount; j++)
                {
                    figureAvailableBlocks[i, j] = true;
                    
                    if (currentChosenFigurePosition.x > 0)
                    {
                        if(currentChosenFigurePosition.x < spawningFieldSize.x - 1 ||
                           currentChosenFigurePosition.x < spawningFieldSize.x - 2)
                    }
                    else if (currentChosenFigurePosition.x == 0)
                    {
                        
                    }
                }
            }
            
            currentChosenFigure.RotateBlock(false, );
        }*/
    }

    private FigureSingle ChooseRandomFigure()
    {
        var maxFiguresCount = allAvailableFigures.Count;

        var randomFigureIndex = Random.Range(0, maxFiguresCount);

        var randomFigure = allAvailableFigures[randomFigureIndex];

        return randomFigure;
    }

    private void MoveCurrentChosenFigure(bool isLeft)
    {
        if (currentChosenFigure == null) return;

        var currentChosenFigureBlocks = currentChosenFigure.GetBlockFigure();
        for (var i = 0; i < currentChosenFigureBlocks.GetLength(0); i++)
        for (var j = 0; j < currentChosenFigureBlocks.GetLength(1); j++)
        {
            if (currentChosenFigureBlocks[i, j] == null) continue;

            if (isLeft)
            {
                if ((j == 0 && currentChosenFigurePosition.x <= 0) ||
                    (j == 1 && currentChosenFigurePosition.x <= -1) ||
                    (j == 2 && currentChosenFigurePosition.x <= -2)) return;
            }
            else
            {
                if ((j == 2 && currentChosenFigurePosition.x >= spawningFieldSize.x - 3) ||
                    (j == 1 && currentChosenFigurePosition.x >= spawningFieldSize.x - 2) ||
                    (j == 0 && currentChosenFigurePosition.x >= spawningFieldSize.x - 1)) return;
            }
        }

        if (isLeft)
            currentChosenFigurePosition.x--;
        else
            currentChosenFigurePosition.x++;

        UpdateSpawningFieldVisual();
    }

    private void UpdateSpawningFieldVisual()
    {
        var currentFigure = currentChosenFigure.GetBlockFigure();
        for (var i = 0; i < spawningFieldSize.y; i++)
        for (var j = 0; j < spawningFieldSize.x; j++)
            allSpawningFieldBlock[i, j].RemoveStoredBlock();

        var figureRowsCount = currentFigure.GetLength(0);
        var figureRowsLength = currentFigure.GetLength(1);

        for (var i = 0; i < figureRowsCount; i++)
        for (var j = 0; j < figureRowsLength; j++)
        {
            if (currentFigure[i, j] == null) continue;

            allSpawningFieldBlock[spawningFieldSize.y - figureRowsCount + i,
                j + currentChosenFigurePosition.x].AddStoredBlock(BlockSingle.BlockColor.Blue, null);
        }
    }

    private void DropCurrentChosenFigure()
    {
        if (currentChosenFigure == null) return;

        var isPlaceFound = false;
        var currentFigureBlocks = currentChosenFigure.GetBlockFigure();
        var currentDroppingPosition = new Vector2Int(currentChosenFigurePosition.x, minigameFieldSize.y - 1);
        while (!isPlaceFound)
            if (!IsCurrentRowPositionAvailableToDropFigure(currentDroppingPosition, currentFigureBlocks,
                    out var isColumnFull))
            {
                if (isColumnFull)
                {
                    Debug.Log("Column full");
                    EndGame();
                    return;
                }

                currentDroppingPosition.y--;
            }
            else
            {
                isPlaceFound = true;
                currentChosenFigure = ChooseRandomFigure();
                currentChosenFigurePosition = new Vector2Int(0, spawningFieldSize.y - 1);
                UpdateSpawningFieldVisual();
            }

        UpdateCurrentFieldVisual(currentDroppingPosition, currentFigureBlocks);
    }

    private bool IsCurrentRowPositionAvailableToDropFigure(Vector2Int dropPosition, BlockSingle[,] figureBlocks,
        out bool isColumnFull)
    {
        Debug.Log("Started checking");
        isColumnFull = false;
        var figureCurrentRow = 2;
        var figureCurrentColumn = 0;

        for (var i = dropPosition.y - 2; i <= dropPosition.y; i++)
        {
            for (var j = dropPosition.x; j <= dropPosition.x + 2; j++)
            {
                if (figureBlocks[figureCurrentRow, figureCurrentColumn] == null)
                {
                    figureCurrentColumn++;
                    continue;
                }

                if (i < 0)
                {
                    Debug.Log(i);
                    isColumnFull = true;
                    return false;
                }

                if (allFieldBlocks[i, j].IsHasStoredBlock()) return false;

                figureCurrentColumn++;
            }

            figureCurrentRow--;
            figureCurrentColumn = 0;
        }

        return true;
    }

    private void UpdateCurrentFieldVisual(Vector2Int dropPosition, BlockSingle[,] figureBlocks)
    {
        var figureCurrentRow = 0;
        var figureCurrentColumn = 0;

        for (var i = 0; i < figureBlocks.GetLength(0); i++)
        for (var j = 0; j < figureBlocks.GetLength(1); j++)
            Debug.Log(figureBlocks[i, j] == null);

        for (var i = dropPosition.y - 2; i <= dropPosition.y; i++)
        {
            for (var j = dropPosition.x; j <= dropPosition.x + 2; j++)
            {
                if (figureBlocks[figureCurrentRow, figureCurrentColumn] == null)
                {
                    figureCurrentColumn++;
                    continue;
                }

                allFieldBlocks[i, j].AddStoredBlock(BlockSingle.BlockColor.Blue, null);

                figureCurrentColumn++;
            }

            figureCurrentRow++;
            figureCurrentColumn = 0;
        }
    }

    private void EndGame()
    {
        currentChosenFigure = null;
    }
}
