using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    //tilemap Property.
    public Tilemap tilemap { get; private set; }

    //ActivePiece variables
    public Piece activePiece { get; private set; }

    //Structure from Tetromino script.
    public TetrominoData[] tetrominos;

    //Vector Position for spawning piece.
    public Vector3Int spawnPosition;

    //Bound of board size.
    public Vector2Int boardSize = new Vector2Int(10, 20);

    //Property of C# for calculating boundsize using RectInt.
    //RectInt is an In-built function which test the bounds for us automatically.
    public RectInt Bounds
    {
        get
        {
            //As the center or Zero is at mid of the board, so we will divide the board by 2 and in negative direction so we can get the lowest point of board bound.
            Vector2Int position = new Vector2Int(-this.boardSize.x / 2, -this.boardSize.y / 2);
            return new RectInt(position, this.boardSize);
        }
    }

    private void Awake()
    {
        //Taking reference of Tilemap in this script.
        //As we know Tilemap is child of the Board GameObject
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.activePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < this.tetrominos.Length; i++)
        {
            this.tetrominos[i].Initialize();
        }
    }

    private void Start()
    {
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        //To select random piece from the Tetrominodata.
        int random = Random.Range(0, this.tetrominos.Length);
        TetrominoData data = this.tetrominos[random];

        //Initialize method/Function used ans 3 parameter is passed for spawning.
        this.activePiece.Initialize(this, this.spawnPosition, data);
        Set(this.activePiece);
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            //Aquiring position of cells.
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            //Setting tile on Tilemap.
            this.tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            //Aquiring position of cells.
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            //Clearing  tile on Tilemap.
            this.tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        //Trieving data of bound from "RectInt Bound".
        RectInt bound = this.Bounds;
        //We need to check all tiles or cell for board boundary, as it may happen that 3 are inside the bound but one is outside.
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;
            //If the tileposition is out of bound, it will return false.
            if (!bound.Contains((Vector2Int)tilePosition))
            {
                return false;
            }
            if (this.tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }
        return true;
    }
}