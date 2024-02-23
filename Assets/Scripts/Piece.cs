using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board Board { get; private set; }
    public TetrominoData Data { get; private set; }
    public Vector3Int Position { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public int RotationIndex { get; private set; }

    public float stepDelay = 1f;
    public float lockDelay = 0.5f;

    private float stepTime;
    private float lockTime;

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        Board = board;
        Position = position;
        Data = data;
        RotationIndex = 0;
        stepTime = Time.time + stepDelay;
        lockTime = 0f;

        if (cells == null)
            cells = new Vector3Int[data.cells.Length];

        for (var i = 0; i < data.cells.Length; i++) cells[i] = (Vector3Int)data.cells[i];
    }

    private void Update()
    {
        Board.Clear(this);

        lockTime += Time.deltaTime;

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

        if (Time.time >= stepTime)
            Step();

        Board.Set(this);
    }

    private void Step()
    {
        stepTime = Time.time + stepDelay;

        Move(Vector2Int.down);

        if (lockTime >= lockDelay)
            Lock();
    }

    private void Lock()
    {
        Board.Set(this);
        Board.ClearLines();
        Board.SpawnPiece();
    }

    private void HardDrop()
    {
        while (Move(Vector2Int.down)) continue;

        Lock();
    }

    private bool Move(Vector2Int translation)
    {
        var newPosition = Position + (Vector3Int)translation;

        if (Board.IsValidPosition(this, newPosition))
        {
            Position = newPosition;
            lockTime = 0f;
            return true;
        }

        return false;
    }

    private void Rotate(int direction)
    {
        var minRotationIndex = 0;
        var maxRotationIndex = 3;

        var originalRotation = RotationIndex;
        RotationIndex = RotationIndex + direction > maxRotationIndex ? minRotationIndex :
            RotationIndex + direction < minRotationIndex ? maxRotationIndex : RotationIndex + direction;

        ApplyRotationMatrix(direction);

        if (!TestWallKicks(RotationIndex, direction))
        {
            RotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
    }

    private void ApplyRotationMatrix(int direction)
    {
        for (var i = 0; i < cells.Length; i++)
        {
            var cell = (Vector3)cells[i];

            int x, y;

            switch (Data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt(cell.x * global::Data.RotationMatrix[0] * direction +
                                        cell.y * global::Data.RotationMatrix[1] * direction);
                    y = Mathf.CeilToInt(cell.x * global::Data.RotationMatrix[2] * direction +
                                        cell.y * global::Data.RotationMatrix[3] * direction);
                    break;
                default:
                    x = Mathf.RoundToInt(cell.x * global::Data.RotationMatrix[0] * direction +
                                         cell.y * global::Data.RotationMatrix[1] * direction);
                    y = Mathf.RoundToInt(cell.x * global::Data.RotationMatrix[2] * direction +
                                         cell.y * global::Data.RotationMatrix[3] * direction);
                    break;
            }

            cells[i] = new Vector3Int(x, y);
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        var wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (var i = 0; i < Data.WallKicks.GetLength(1); i++)
        {
            var translation = Data.WallKicks[wallKickIndex, i];

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
        var maxWallKickIndex = Data.WallKicks.GetLength(0);

        return wallKickIndex > maxWallKickIndex ? minWallKickIndex :
            wallKickIndex < minWallKickIndex ? maxWallKickIndex :
            wallKickIndex;
    }
}
