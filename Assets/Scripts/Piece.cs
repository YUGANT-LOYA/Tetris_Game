using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }
    public int rotationIndex { get; private set; }

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.board = board;
        this.position = position;
        this.data = data;

        //This is initialization of the piece when it spawns in its default space. 
        this.rotationIndex = 0;

        //If cells array is null, so we will Initialize the array.
        if (this.cells == null)
        {
            this.cells = new Vector3Int[data.cells.Length];
        }

        //We will now copy the data.cell array into this array.
        for (int i = 0; i < data.cells.Length; i++)
        {
            this.cells[i] = (Vector3Int) data.cells[i];
        }
    }

    private void Update()
    {
        //Before starting the game, it clear the board.
        this.board.Clear(this);

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

        //This will set the following piece after any of the input taken from user.
        this.board.Set(this);
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
        }

        return valid;
    }

    private void RotationTetro(int direction)
    {
        //For Rotation of a Tetro in 0 to 4 possible ways.
        this.rotationIndex = Wrap(this.rotationIndex + direction, 0, 4);

        //We will not select "this.data.cells.Length" because it will make changes in the original cells and board, but we need changes in the copy of the cell we created, thats the reason we choose and edit in "this.cells.Length".
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3 cell = this.cells[i];
            //Local co-ordinates.
            int x , y;

            //There is a difference in rotation of I and O comparing with others, so we are using switch case for that.
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
                    x =Mathf.RoundToInt( (cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
            }

            this.cells[i] = new Vector3Int(x, y, 0);
        }
    }

    //Standard wrap utility maths function, to check whether it is inside the bound or outside the bound
    private int Wrap(int input, int min, int max)
    {
        if (input < min)
            return max - (min - input) % (max - min);
        else
            return min + (input - min) % (max - min);
    }

}