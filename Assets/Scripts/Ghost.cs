using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
    public Tile tile;
    public Board board;
    public Piece trackingPiece;

    public Vector3Int[] cells { get; private set; }
    public Tilemap tileMap { get; private set; }
    public Vector3Int position { get; private set; }

    public void Awake()
    {
        this.tileMap = GetComponentInChildren<Tilemap>();
        this.cells = new Vector3Int[4];
    }

    private void LateUpdate()
    {
        Clear();
        Copy();
        Drop();
        Set();
    }

    private void Clear()
    {
        //Same as set , except it will not set "this.tile", it will provide null value to make it clear.
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3Int tilePosition = this.cells[i] + this.position;
            this.tileMap.SetTile(tilePosition, null);
        }
    }

    private void Copy()
    {
        //this will copy the tetro in this.cells from trackingPiece of the main board.
        for (int i = 0; i < this.cells.Length; i++)
        {
            this.cells[i] = this.trackingPiece.cells[i];
        }
    }

    private void Drop()
    {
        //Keeping track of position of Piece.
        Vector3Int position = this.trackingPiece.position;
        int current = position.y;
        //as the center of board is (0,0) so the bottom most point is....
        int bottom = -this.board.boardSize.y / 2 - 1;

        this.board.Clear(this.trackingPiece);

        for (int row = current; row >= bottom; row--)
        {
            if (this.board.IsValidPosition(this.trackingPiece, position))
            {
                this.position = position;
            }
            else
                break;
        }

        this.board.Set(this.trackingPiece);
    }

    private void Set()
    {
        //Set all the tiles
        for (int i = 0; i < this.cells.Length; i++)
        {
            Vector3Int tilePosition = this.cells[i] + this.position;
            this.tileMap.SetTile(tilePosition, this.tile);
        }
    }
}