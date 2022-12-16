using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using WaveFunctionCollapse;
using System.Text;

public class Test : MonoBehaviour
{
    public Tilemap inputTilemap;
    public Tilemap outputTilemap;
    public int patternSize;
    public int maxIteration = 500;
    public int outputWidth = 5;
    public int outputHeight = 5;
    public bool equalWeights = false;
    ValuesManager<TileBase> valueManager;
    WFCCore core;
    PatternManager manager;
    TileMapOutput output;
    // Start is called before the first frame update
    void Start()
    {

        CreateWFC();

        /*StringBuilder builder = null;
        List<string> list = new List<string>();
        for(int row = 0; row < grid.Length; ++row)
        {
            builder = new StringBuilder();
            for(int col = 0; col < grid[0].Length; ++col)
            {
                builder.Append(valueManager.GetGridValuesIncludingOffset(col, row) + " ");
            }
            list.Add(builder.ToString());
        }
        list.Reverse();
        foreach(var item in list)
        {
            Debug.Log(item);
        }*/

    }

    public void CreateWFC()
    {
        InputReader reader = new InputReader(inputTilemap);
        var grid = reader.ReadInputToGrid();
        valueManager = new ValuesManager<TileBase>(grid);
        manager = new PatternManager(patternSize);
        manager.ProcessGrid(valueManager, equalWeights);
        core = new WFCCore(outputWidth, outputHeight, 500, manager);
    }

    public void CreateTilemap()
    {
        output = new TileMapOutput(valueManager, outputTilemap);
        var result = core.CreateOutputGrid();
        output.CreateOutput(manager, result, outputWidth, outputHeight);
    }
    // Update is called once per frame
    public void SaveTilemap()
    {
        if(output.OutputImage != null)
        {
            outputTilemap = output.OutputImage;
            GameObject objectToSave = outputTilemap.gameObject;

            PrefabUtility.SaveAsPrefabAsset(objectToSave, "Assets/Saved/output.prefab");
        }
    }
}
