using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using WaveFunctionCollapse;
using System;
using System.Linq;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    public Tilemap inputTilemap;
    public Tilemap outputTilemap;
    public int patternSize;
    public int maxIteration = 500;
    public int outputWidth = 10;
    public int outputHeight = 10;
    public bool equalWeights = false;
    ValuesManager<TileBase> valueManager;
    WFCCore core;
    PatternManager manager;
    TileMapOutput output;

    void Start()
    {
        CreateWFC();

        //code to test if finidng neighbour works
        /*foreach (Direction dir in Enum.GetValues(typeof(Direction)))
        {
            Debug.Log(manager.GetPossibleNeighboursForPatternInDirection(0, dir));
        }*/
        /* */
    }

    //method for creating wfc
    public void CreateWFC()
    {
        InputReader reader = new InputReader(inputTilemap);
        var grid = reader.ReadInputToGrid();
        /*        for (int row=0; row<grid.Length;row++)
                {
                    for (int col = 0; col < grid[0].Length; col++)
                    {
                        Debug.Log("Row: " + row + "Col: " + col + " tile name " + grid[row][col].Value.name);
                    }
                }*/

        valueManager = new ValuesManager<TileBase>(grid);
        manager = new PatternManager(patternSize);
        manager.ProcessGrid(valueManager, equalWeights);
        core = new WFCCore(outputWidth, outputHeight, maxIteration, manager);
    }

    public void CreateTilemap()
    {
        output = new TileMapOutput(valueManager, outputTilemap);
        var result = core.CreateOutputGrid(manager);//passed in extra manager here
        output.CreateOutput(manager, result, outputWidth, outputHeight);
    }


    public void SaveTilemap()
    {
        if (output.OutputImage != null)
        {
            outputTilemap = output.OutputImage;
            GameObject objectToSave = outputTilemap.gameObject;

/*            PrefabUtility.SaveAsPrefabAsset(objectToSave, "Assets/Saved/output.prefab");
*/        }
    }
}

