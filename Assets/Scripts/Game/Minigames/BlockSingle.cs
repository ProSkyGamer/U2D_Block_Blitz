using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockSingle : MonoBehaviour
{
    #region Enums

    public enum ConnectedBlocks
    {
        Up,
        Right,
        Left,
        Bottom
    }

    public enum BlockColor
    {
        Red,
        Blue,
        Yellow
    }

    #endregion

    [SerializeField] private bool isStoredBlockCanBeRemoved;

    [SerializeField] private List<ConnectedBlocks> allConnectedBlocks = new();
    [SerializeField] private BlockColor storedBlockColor;
    private bool isHasStoredBlock;

    private Image blockImage;

    public void AddStoredBlock(BlockColor blockColor, List<ConnectedBlocks> connectedBlocks)
    {
        if (isHasStoredBlock) return;

        isHasStoredBlock = true;
        storedBlockColor = blockColor;
        /*allConnectedBlocks.AddRange(connectedBlocks);*/

        UpdateBlockVisual();
    }

    public void RemoveStoredBlock()
    {
        if (!isStoredBlockCanBeRemoved) return;

        isHasStoredBlock = false;

        UpdateBlockVisual();
    }

    private void UpdateBlockVisual()
    {
        if (blockImage == null)
            blockImage = GetComponent<Image>();

        blockImage.color = isHasStoredBlock ? Color.blue : Color.black;
    }

    public bool IsHasStoredBlock()
    {
        return isHasStoredBlock;
    }
}
