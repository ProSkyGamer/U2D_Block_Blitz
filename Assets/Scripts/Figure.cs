using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Figure : MonoBehaviour
{
    public event EventHandler OnFigureLocked;

    [SerializeField] protected float stepDelay = 1f;
    [SerializeField] protected bool isFallingDown = true;

    [SerializeField] protected float lockDelay = 0.5f;
    [SerializeField] protected bool isHasLockDelay = true;

    protected TetrisBoardBasic tetrisBoardBasicThatPlacedOn;
    protected FigureData figureData;
    protected Vector3Int figurePosition;
    protected Vector3Int[] figureCells;
    protected int figureRotationIndex;

    protected float stepTimer;
    protected float lockTimer;

    public virtual void Initialize(TetrisBoardBasic tetrisBoardBasic, Vector3Int position, FigureData data)
    {
        tetrisBoardBasicThatPlacedOn = tetrisBoardBasic;
        figurePosition = position;
        figureData = data;
        figureRotationIndex = 0;
        stepTimer = stepDelay;
        lockTimer = lockDelay;

        if (figureCells == null)
            figureCells = new Vector3Int[data.figureCells.Length];

        for (var i = 0; i < data.figureCells.Length; i++) figureCells[i] = (Vector3Int)data.figureCells[i];
    }

    private void Update()
    {
        tetrisBoardBasicThatPlacedOn.ClearPiece(this);

        lockTimer -= Time.deltaTime;

        if (isFallingDown)
            stepTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Q))
            Rotate(-1);
        else if (Input.GetKeyDown(KeyCode.E)) Rotate(1);

        if (Input.GetKeyDown(KeyCode.A))
            Move(Vector2Int.left);
        else if (Input.GetKeyDown(KeyCode.D)) Move(Vector2Int.right);

        if (Input.GetKeyDown(KeyCode.S))
            Move(Vector2Int.down);

        if (Input.GetKeyDown(KeyCode.Space))
            HardDrop();

        if (stepTimer <= 0 && isFallingDown)
            Step();

        tetrisBoardBasicThatPlacedOn.SetPiece(this, false);
    }

    private void Step()
    {
        stepTimer = stepDelay;

        Move(Vector2Int.down);

        if ((lockTimer <= 0 && isHasLockDelay) || !isHasLockDelay)
            Lock();
    }

    private void Lock()
    {
        OnFigureLocked?.Invoke(this, EventArgs.Empty);
    }

    protected void HardDrop()
    {
        while (Move(Vector2Int.down)) continue;

        Lock();
    }

    protected bool Move(Vector2Int translation)
    {
        var newFigurePosition = figurePosition + (Vector3Int)translation;

        if (tetrisBoardBasicThatPlacedOn.IsValidPosition(this, newFigurePosition))
        {
            figurePosition = newFigurePosition;
            lockTimer = lockDelay;
            return true;
        }

        return false;
    }

    protected void Rotate(int direction)
    {
        var minRotationIndex = 0;
        var maxRotationIndex = 3;

        var originalRotation = figureRotationIndex;
        figureRotationIndex = figureRotationIndex + direction > maxRotationIndex ? minRotationIndex :
            figureRotationIndex + direction < minRotationIndex ? maxRotationIndex : figureRotationIndex + direction;

        ApplyRotationMatrix(direction);

        if (!TestWallKicks(figureRotationIndex, direction))
        {
            figureRotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
    }

    private void ApplyRotationMatrix(int direction)
    {
        for (var i = 0; i < figureCells.Length; i++)
        {
            var cell = (Vector3)figureCells[i];

            int x, y;

            switch (figureData.figureType)
            {
                case FigureType.I:
                case FigureType.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt(cell.x * Data.RotationMatrix[0] * direction +
                                        cell.y * Data.RotationMatrix[1] * direction);
                    y = Mathf.CeilToInt(cell.x * Data.RotationMatrix[2] * direction +
                                        cell.y * Data.RotationMatrix[3] * direction);
                    break;
                default:
                    x = Mathf.RoundToInt(cell.x * Data.RotationMatrix[0] * direction +
                                         cell.y * Data.RotationMatrix[1] * direction);
                    y = Mathf.RoundToInt(cell.x * Data.RotationMatrix[2] * direction +
                                         cell.y * Data.RotationMatrix[3] * direction);
                    break;
            }

            figureCells[i] = new Vector3Int(x, y);
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        var wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (var i = 0; i < figureData.figureWallKicks.GetLength(1); i++)
        {
            var translation = figureData.figureWallKicks[wallKickIndex, i];

            if (Move(translation))
                return true;
        }

        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        var wallKickIndex = rotationIndex * 2;
        if (rotationIndex < 0) wallKickIndex--;

        var minWallKickIndex = 0;
        var maxWallKickIndex = figureData.figureWallKicks.GetLength(0);

        return wallKickIndex > maxWallKickIndex ? minWallKickIndex :
            wallKickIndex < minWallKickIndex ? maxWallKickIndex :
            wallKickIndex;
    }

    public Vector3Int[] GetFigureCells()
    {
        return figureCells;
    }

    public Vector3Int GetFigurePosition()
    {
        return figurePosition;
    }

    public Tile GetFigureTile()
    {
        return figureData.figureTile;
    }
}
