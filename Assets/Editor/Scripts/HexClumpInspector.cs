using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HexClump))]
public class HexClumpInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Randomize Clump", GUILayout.Width(150)))
        {
            HexClump clump = target as HexClump;
            clump.CreateClump();
        }
    }
}
