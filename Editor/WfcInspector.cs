using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Test))]
public class WfcInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Test myScript = (Test)target;
        if(GUILayout.Button("Create tilemap"))
        {
            myScript.CreateWFC();
            myScript.CreateTilemap();
        }
        if(GUILayout.Button("Save tilmap"))
        {
            myScript.SaveTilemap();
        }
    }
}
