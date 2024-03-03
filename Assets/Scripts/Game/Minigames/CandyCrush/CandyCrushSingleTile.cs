using System;
using UnityEngine;
using UnityEngine.UI;

public class CandyCrushSingleTile : MonoBehaviour
{
    public static event EventHandler<OnTryRemoveTileEventArgs> OnTryRemoveTile;

    public class OnTryRemoveTileEventArgs : EventArgs
    {
        public CandyCrushSingleTile removingTile;
    }

    public enum TileColor
    {
        Green,
        Yellow,
        Red
    }

    [SerializeField] private TileColor tileColor;
    private Vector2Int tileGridPosition;

    private Button tileButton;

    private void Awake()
    {
        tileButton = GetComponent<Button>();

        tileButton.onClick.AddListener(() =>
        {
            OnTryRemoveTile.Invoke(this, new OnTryRemoveTileEventArgs
            {
                removingTile = this
            });
        });
    }

    public void SetTilePosition(Vector2Int newTileGridPosition)
    {
        tileGridPosition = newTileGridPosition;
    }

    public TileColor GetTileColor()
    {
        return tileColor;
    }

    public Vector2Int GetTileGridPosition()
    {
        return tileGridPosition;
    }
}
