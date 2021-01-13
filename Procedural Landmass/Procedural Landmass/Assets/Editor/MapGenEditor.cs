using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapGenEditor : Editor
{
    public override void OnInspectorGUI() {
        //get a reference to the map generator
        MapGenerator mapGen = (MapGenerator) target;

        if (DrawDefaultInspector()) {
            if (mapGen.autoUpdate)
                mapGen.GenerateMap();
        }

        if (GUILayout.Button("Generate")) {
            mapGen.GenerateMap();
        }
    }
}
