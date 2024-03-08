using System;
using UnityEngine;
using UnityEngine.UI;

public class SameGameSingleTile : MonoBehaviour
{
    #region Enums

    public enum TileColor
    {
        Green,
        Yellow,
        Red
    }

    #endregion

    #region Event & Events Args

    public static event EventHandler<OnTryRemoveTileEventArgs> OnTryRemoveTile;

    public class OnTryRemoveTileEventArgs : EventArgs
    {
        public SameGameSingleTile removingTile;
    }

    #endregion

    #region Variables & References

    [SerializeField] private TileColor tileColor;
    private Vector2Int tileGridPosition;

    private Button tileButton;

    #endregion

    #region Initialization

    private void Awake()
    {
        tileButton = GetComponent<Button>();

        tileButton.onClick.AddListener(() =>
        {
            OnTryRemoveTile?.Invoke(this, new OnTryRemoveTileEventArgs
            {
                removingTile = this
            });
        });
    }

    #endregion

    #region Tile Position

    public void SetTilePosition(Vector2Int newTileGridPosition)
    {
        tileGridPosition = newTileGridPosition;
    }

    public Vector2Int GetTileGridPosition()
    {
        return tileGridPosition;
    }

    #endregion

    #region Tile Color

    public TileColor GetTileColor()
    {
        return tileColor;
    }

    #endregion
}
