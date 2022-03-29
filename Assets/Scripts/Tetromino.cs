using System;
using UnityEngine;
using UnityEngine.Tilemaps;

//Created enum to store all custom and standard blocks which we will be using in game and named them accordingly.
public enum Tetromino
{
    I,
    O,
    T,
    J,
    L,
    S,
    Z,
}

[Serializable]
public struct TetrominoData
{
    //Created an instance/object to get access of enum Tetromino.
    public Tetromino tetromino;

    //Instance of Tile.
    public Tile tile;

    //2D array for storing cell position vectors and shapes.
    public Vector2Int[] cells { get; private set; }

    //We have Vector2Int[] cells that we are assigning based on static data .
    public void Initialize()
    {
        this.cells = Data.Cells[this.tetromino];
    }
}