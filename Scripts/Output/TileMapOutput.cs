using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

namespace WaveFunctionCollapse
{
    public class TileMapOutput : IOutputCreator<Tilemap>
    {
        private Tilemap outputImage;
        private ValuesManager<TileBase> valueManager;
        public Tilemap OutputImage => outputImage;

        public TileMapOutput(ValuesManager<TileBase> valueManager, Tilemap outputImage)
        {
            this.outputImage = outputImage;
            this.valueManager = valueManager;
        }

        public void CreateOutput(PatternManager manager, int[][] outputvalues, int width, int height)
        {
            if (outputvalues.Length == 0)
            {
                return;
            }
            this.outputImage.ClearAllTiles();

            int[][] valueGrid;
            valueGrid = manager.ConvertPatternToValues<TileBase>(outputvalues);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    TileBase tile = (TileBase)this.valueManager.GetValueFromIndex(valueGrid[row][col]).Value;
                    this.outputImage.SetTile(new Vector3Int(col, row, 0), tile);
                }
            }

            PrintGridToConsol(valueGrid, width, height);
            
            //When the algorithm is done generating the value grid,
            //it is time to generate a JSON file that is used as an input to generate a game level
            //in the "Dice Adventure" game
            SaveGridToJSON(valueGrid, width, height);
        }

        void PrintGridToConsol(int[][] valueGrid, int width, int height)
        {
            for (int row = 0; row < height; row++)
            {
                Debug.Log("ROW " + row + "___");
                string s = "";
                for (int col = 0; col < width; col++)
                {
                    s = s + valueGrid[row][col].ToString() + " "; 
                    
                }
                Debug.Log(s);
                Debug.Log("___\n");
            }
        }


        //JSON file and classes to generate the JSON output
        public TextAsset levelJSON;

        //every cell contains 
        //a code to indicate what object to be spawn
        [System.Serializable]
        public class JsonTile
        {
            public int x;
            public int y;
            public string code;
        }

        [System.Serializable]
        public class TileList
        {
            public JsonTile[] tile;
        }

        public TileList tileArray = new TileList();

        void SaveGridToJSON(int[][] valueGrid, int width, int height)
        {

            //the JSON output will contain the only the tiles 
            //that are not empty. A tile is empty if it is passable
            //and does not contain any object to be spawn
            List<JsonTile> tileList = new List<JsonTile>();
            for (int row = 0; row < height; row++)
            {
                
                Debug.Log("ROW " + row + "___");
                //string s = "";
                for (int col = 0; col < width; col++)
                {
                    //s = s + valueGrid[row][col].ToString() + " ";
                    if(valueGrid[row][col] != 0)
                    {
                        string s = "T";
                        switch(valueGrid[row][col])
                        {
                            case 1:
                            //TRAP
                                s = "T";
                                break;

                            case 2:
                            //Unpassable Tile
                                s = "U";
                                break;

                            case 3:
                            //Monster
                                s = "M";
                                break;

                            case 4:
                            //Giant Altar
                                s = "GA";
                                break;

                            case 5:
                            //Goal
                                s = "G";
                                break;

                            case 6:
                            //Rock
                                s = "R";
                                break;

                            case 7:
                            //Dwarf Altar
                                s = "DA";
                                break;

                            case 8:
                            //Human Altar
                                s = "HA";
                                break;
                        }
                           
                        //filling the cell info
                        JsonTile t = new JsonTile();
                        t.x = row;
                        t.y = col;
                        t.code = s;
                        tileList.Add(t);
                    }
                }
                //Debug.Log(s);
                Debug.Log("___\n");
            }
            
            tileArray.tile = tileList.ToArray();
            string strOutput = JsonUtility.ToJson(tileArray);
            //output the generated grid to the JSSON file "myLevel". This file 
            //will be used to generate the 3D game level
            File.WriteAllText(Application.dataPath + "/Resources/myLevel.JSON", strOutput);
        }
    }

}



