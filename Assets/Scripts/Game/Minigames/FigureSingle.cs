using System;
using System.Collections.Generic;
using UnityEngine;

public class FigureSingle : MonoBehaviour
{
    [Serializable]
    public class FigureRow
    {
        public List<BlockSingle> figureRow;
    }

    [SerializeField] private List<FigureRow> blockFigure = new();
    [SerializeField] private List<FigureRow> combinedBlockFigure = new();

    public BlockSingle[,] GetBlockFigure()
    {
        var blockFigureArray = new BlockSingle[blockFigure.Count, blockFigure.Count];

        for (var i = 0; i < blockFigure.Count; i++)
        for (var j = 0; j < blockFigure.Count; j++)
            blockFigureArray[i, j] = blockFigure[i].figureRow[j];

        return blockFigureArray;
    }

    public void RotateBlock(bool isCounterClockWise, bool[,] availableBlocks)
    {
        var oldFigure = new BlockSingle[blockFigure.Count, blockFigure.Count];

        for (var i = 0; i < blockFigure.Count; i++)
        for (var j = 0; j < blockFigure.Count; j++)
            oldFigure[i, j] = blockFigure[i].figureRow[j];

        var newFigure = new BlockSingle[blockFigure.Count, blockFigure.Count];

        if (isCounterClockWise)
            for (var i = 0; i < oldFigure.GetLength(0); i++)
            for (var j = 0; j < oldFigure.GetLength(1); j++)
            {
                if (oldFigure[i, j] == null) continue;

                newFigure[j, i > 1 ? i - 2 : i < 1 ? i + 2 : i] = oldFigure[i, j];
            }
        else
            for (var i = 0; i < oldFigure.GetLength(0); i++)
            for (var j = 0; j < oldFigure.GetLength(1); j++)
            {
                if (oldFigure[i, j] == null) continue;

                newFigure[j > 1 ? j - 2 : j < 1 ? j + 2 : j, i] = oldFigure[i, j];
            }

        for (var i = 0; i < newFigure.GetLength(0); i++)
        for (var j = 0; j < newFigure.GetLength(1); j++)
        {
            if (newFigure[i, j] == null) continue;

            if (!availableBlocks[i, j]) return;
        }

        var newFigureList = new List<FigureRow>();

        for (var i = 0; i < newFigure.GetLength(0); i++)
        {
            newFigureList.Add(new FigureRow());
            for (var j = 0; j < newFigure.GetLength(1); j++) newFigureList[i].figureRow.Add(newFigure[i, j]);
        }

        blockFigure = newFigureList;
    }

    public int GetFigureSize()
    {
        return blockFigure.Count;
    }
}
