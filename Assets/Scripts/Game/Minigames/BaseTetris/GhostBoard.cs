using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GhostBoard : MonoBehaviour
{
    #region Events

    public event EventHandler OnGhostPiecePlaced;

    #endregion

    #region References & Variables

    [SerializeField] protected Tile ghostTile;
    [SerializeField] protected TetrisBoardBasic followingBoard;
    [SerializeField] protected Figure followingFigure;

    protected Tilemap ghostTilemap;

    protected readonly List<Vector3Int> ghostFigureCells = new();
    protected Vector3Int ghostPieceTilemapPosition;

    private bool isFirstUpdate = true;

    #endregion

    #region Initialization

    private void Awake()
    {
        ghostTilemap = GetComponentInChildren<Tilemap>();
    }

    private void Start()
    {
        followingFigure.OnFigureMoved += FollowingFigure_OnFigureMoved;
    }

    private void Update()
    {
        if (isFirstUpdate)
        {
            isFirstUpdate = false;
            followingBoard.AddFollowingGhostBoard(this);
        }
    }

    private void FollowingFigure_OnFigureMoved(object sender, EventArgs e)
    {
        ClearPreviousGhostFigure();
        CopyNewFigure();
        DropGhostFigure();
        SetGhostFigure();
    }

    #endregion

    #region GHost Figure Methods

    private void ClearPreviousGhostFigure()
    {
        foreach (var ghostCellPosition in ghostFigureCells)
        {
            var tilePosition = ghostCellPosition + ghostPieceTilemapPosition;
            ghostTilemap.SetTile(tilePosition, null);
        }
    }

    private void CopyNewFigure()
    {
        var figureCells = followingFigure.GetFigureCells();
        ghostFigureCells.Clear();
        ghostFigureCells.AddRange(figureCells);
    }

    protected virtual void DropGhostFigure()
    {
        var position = followingFigure.GetFigurePosition();

        var currentRow = position.y;
        var bottom = -followingBoard.GetBoardSize().y / 2 - 1;

        for (var i = currentRow; i >= bottom; i--)
        {
            position.y = i;

            if (followingBoard.IsValidPosition(ghostFigureCells, position))
                ghostPieceTilemapPosition = position;
            else
                break;
        }
    }

    protected virtual void SetGhostFigure()
    {
        foreach (var ghostFigureCellPosition in ghostFigureCells)
        {
            var tilePosition = ghostFigureCellPosition + ghostPieceTilemapPosition;
            ghostTilemap.SetTile(tilePosition, ghostTile);
        }

        OnGhostPiecePlaced?.Invoke(this, EventArgs.Empty);
    }

    #endregion
}
