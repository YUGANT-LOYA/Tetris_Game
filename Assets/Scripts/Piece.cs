using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }
    public int rotationIndex { get; private set; }
    public float stepDelay = 0.7f;
    public float lockDelay = 0.5f;

    private float stepTime;
    private float lockTime;

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.board = board;
        this.position = position;
        this.data = data;
        //This is initialization of the new piece when it spawns in its default space position.
        this.rotationIndex = 0;

        this.stepTime = Time.time + this.stepDelay;
        this.lockTime = 0f;

        //If cells array is null, so we will Initialize the array.
        if (this.cells == null)
        {
            this.cells = new Vector3Int[data.cells.Length];
        }

        //We will now copy the data.cell array into this array.
        for (int i = 0; i < data.cells.Length; i++)
        {
            this.cells[i] = (Vector3Int)data.cells[i];
        }
    }

    private void Update()
    {
        //Before starting the game, it clear the board.
        this.board.Clear(this);
        //This will increase with time to keep track of time, when to lock.
        this.lockTime += Time.deltaTime;

        //Input from User/Player.
        if (Input.GetKeyDown(KeyCode.A))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Move(Vector2Int.right);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Move(Vector2Int.down);
        }

        //HardDrop Inputs
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }

        //Rotation Input
        if (Input.GetKeyDown(KeyCode.Q))
        {
            RotationTetro(-1);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            RotationTetro(1);
        }
        //This function will update, and keep track of when the normal time exceeds the stepTime.
        //and then the Step() function will be called.
        if(Time.time >= this.stepTime)
        {
            Step();
        }

        //This will set the following piece after any of the input taken from user.
        this.board.Set(this);
    }

    private void Step()
    {
        this.stepTime = Time.time + this.stepDelay; 
        //This will make out Tetro Piece continously move downwards.
        Move(Vector2Int.down);
        //This Statement will lock the piece, if the piece is not moving more than lockDelay.
        if(this.lockTime >= this.lockDelay)
        {
            Lock();
        }
    }
    //This will lock the current piece and spawn new piece.
    private void Lock()
    {
        this.board.Set(this);
        this.board.ClearLines();
        this.board.SpawnPiece();
    }

    //This function will continues to move down until, it reaches it's lowest limit of the board.
    private void HardDrop()
    {
        while (Move(Vector2Int.down))
        {
            continue;
        }
    }

    //Move the Piece from left to right under the bound of board.
    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = this.position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = this.board.IsValidPosition(this, newPosition);

        if (valid)
        {
            this.position = newPosition;

            //This will reset every time whenever there is movement, until it exceeds the lockTime.
            this.lockTime = 0f;
        }

        return valid;
    }

    private void RotationTetro(int direction)
    {
        //We saved our original rotation which is used to revert the process.
        int originalRotation = this.rotationIndex;
        //For Rotation of a Tetro in 0 to 4 possible ways.
        this.rotationIndex = Wrap(this.rotationIndex + direction, 0, 4);

        ApplyRotationMatrix(direction);

        //This checks if our rotation fails so it will revert back to its original Rotation.
        if (!TestWallKicks(this.rotationIndex, direction))
        {
            this.rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
    }

    private void ApplyRotationMatrix(int direction)
    {
        //We will not select "this.data.cells.Length" because it will make changes in the original cells and board, but we need changes in the copy of the cell we created, thats the reason we choose and edit in "this.cells.Length".
        for (int i = 0; i < this.cells.Length; i++)
        {
            //We are using Vector3 instead of Vector3Int becoz as we know the standard rotation of the Tetro I and Tetro O is different than others, they have some offset value which make the number as float, that is the reason of Vector3.
            Vector3 cell = this.cells[i];
            //Local co-ordinates.
            int x, y;

            //There is a difference in rotation of I and O comparing with others, so we are using switch case for that.
            //Complex Concept of Rotation.
            switch (this.data.tetromino)
            {
                case Tetromino.I:

                case Tetromino.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;

                default:
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
            }

            this.cells[i] = new Vector3Int(x, y, 0);
        }
    }
    //We should know in which RotationIndex we are and in which direction we need to rotate for TestWallKicks.
    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < this.data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = this.data.wallKicks[wallKickIndex, i];

            if (Move(translation))
            {
                return true;
            }
        }
        return false;
    }
    //There is a pattern in finding WallKickIndex.
    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        //This is pattern for index, we have to select every alternative. The pattern is on Tetris Wikipedia.   
        int wallKickIndex = rotationIndex * 2;

        if (rotationDirection < 0)
        {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, this.data.wallKicks.GetLength(0));
    }

    //Standard wrap utility maths function, to check whether it is inside the bound or outside the bound.
    private int Wrap(int input, int min, int max)
    {
        if (input < min)
            return max - (min - input) % (max - min);
        else
            return min + (input - min) % (max - min);
    }
}