using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    //tilemap Property.
    public Tilemap tilemap { get; private set; }
    //ActivePiece variable.s
    public Piece activePiece { get; private set; } 
    //Structure from Tetromino script.
    public TetrominoData[] tetrominos;

    public Vector3Int spawnPosition;


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

}
