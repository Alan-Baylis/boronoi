using Assets;
using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(World))]
public class WorldEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var world = (World)target;
        if (GUILayout.Button("Build island"))
        {
            world.Build();
        }
    }
}