using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WaveFunctionCollapse
{
    public class InputImageParameters
    {
        Vector2Int? bottomRightTileCoords = null;
        Vector2Int? topLeftTileCoords = null;
        BoundsInt inputTileMapBounds;
        TileBase[] inputTilemapTilesArray;
        public Queue<TileContainer> stackOfTiles = new Queue<TileContainer>();
        private int width = 0, height = 0;
        private Tilemap inputTilemap;

        public Queue<TileContainer> StackOftiles { get => stackOfTiles; set => stackOfTiles = value; }
        public int Height { get => height; }
        public int Width { get => width; }


        public InputImageParameters(Tilemap inputTilemap)
        {
            this.inputTilemap = inputTilemap;
            this.inputTileMapBounds = this.inputTilemap.cellBounds;
            this.inputTilemapTilesArray = this.inputTilemap.GetTilesBlock(this.inputTileMapBounds);
            ExtractNonEmptyTiles();
            VeryfyInputTiles();
        }

        private void VeryfyInputTiles()
        {
            if (topLeftTileCoords == null || bottomRightTileCoords == null)
            {
                throw new System.Exception("WFC: Input tIlemap is empty");
            }
            int minX = bottomRightTileCoords.Value.x;
            int maxX = topLeftTileCoords.Value.x;
            int minY = bottomRightTileCoords.Value.y;
            int maxY = topLeftTileCoords.Value.y;

            width = Mathf.Abs(maxX - minX) + 1;
            height = Mathf.Abs(maxY - minY) + 1;

            int tileCount = width * height;
            if (stackOfTiles.Count != tileCount)
            {
                throw new System.Exception("WFC: Tilemap has empty fields");
            }
            if (stackOfTiles.Any(tile => tile.X > maxX || tile.X < minX || tile.Y > maxY || tile.Y < minY))
            {
                throw new System.Exception("WFC: Tilemap image should be a filled rectangle");
            }
        }

        private void ExtractNonEmptyTiles()
        {
            for (int row = 0; row < inputTileMapBounds.size.y; row++)
            {
                for (int col = 0; col < inputTileMapBounds.size.x; col++)
                {
                    int index = col + (row * inputTileMapBounds.size.x);

                    TileBase tile = inputTilemapTilesArray[index];
                    if (bottomRightTileCoords == null && tile != null)
                    {
                        bottomRightTileCoords = new Vector2Int(col, row);
                    }
                    if (tile != null)
                    {
                        stackOfTiles.Enqueue(new TileContainer(tile, col, row));
                        topLeftTileCoords = new Vector2Int(col, row);
                    }
                }
            }
        }
    }
}