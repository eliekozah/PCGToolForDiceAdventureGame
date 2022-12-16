using Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WaveFunctionCollapse
{
    public class InputReader : IInputReader<TileBase>
    {
        private Tilemap _inputTilemap;

        public InputReader(Tilemap input)
        {
            _inputTilemap = input;
        }

        public IValue<TileBase>[][] ReadInputToGrid()
        {
            var grid = ReadInputTileMap();
            TileBaseValue[][] gridOfValues = null;
            if (grid != null)
            {
                gridOfValues = MyCollectionExtension.CreateJaggedArray<TileBaseValue[][]>(grid.Length, grid[0].Length);
                for (int row = 0; row < grid.Length; row++)
                {
                    for (int col = 0; col < grid[0].Length; col++)
                    {
                        gridOfValues[row][col] = new TileBaseValue(grid[row][col]);
                    }
                }
            }
            return gridOfValues;
        }

        private TileBase[][] ReadInputTileMap()
        {
            InputImageParameters imagerParameters = new InputImageParameters(_inputTilemap);
            return CreateTileBasedGrid(imagerParameters);
        }

        private TileBase[][] CreateTileBasedGrid(InputImageParameters imagerParameters)
        {
            TileBase[][] girdOfInputTiles = null;
            girdOfInputTiles = MyCollectionExtension.CreateJaggedArray<TileBase[][]>(imagerParameters.Height, imagerParameters.Width);
            for (int row = 0; row < imagerParameters.Height; row++)
            {
                for (int col = 0; col < imagerParameters.Width; col++)
                {
                    girdOfInputTiles[row][col] = imagerParameters.stackOfTiles.Dequeue().Tile;
                }
            }
            return girdOfInputTiles;
        }

    }
}