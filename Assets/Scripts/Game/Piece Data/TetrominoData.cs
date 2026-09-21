using UnityEngine;
using UnityEngine.Tilemaps;
using static TetrominoType;

[CreateAssetMenu(menuName = "Tetris/Tetromino")]
public class TetrominoData : ScriptableObject
{
    public TileBase ghostTile; // Semi-transparent version of the tile
    public TetrominoType tetromino;
    public TileBase tile;                    // The visual tile
    public Vector2Int[] cells;               // Shape data (relative positions)
}