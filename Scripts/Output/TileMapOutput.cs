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


        public TextAsset levelJSON;

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
                                s = "T";
                                break;

                            case 2:
                                s = "U";
                                break;

                            case 3:
                                s = "M";
                                break;

                            case 4:
                                s = "GA";
                                break;

                            case 5:
                                s = "G";
                                break;

                            case 6:
                                s = "R";
                                break;

                            case 7:
                                s = "DA";
                                break;

                            case 8:
                                s = "HA";
                                break;
                        }

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
            File.WriteAllText(Application.dataPath + "/Resources/myLevel.JSON", strOutput);
        }
    }

}



