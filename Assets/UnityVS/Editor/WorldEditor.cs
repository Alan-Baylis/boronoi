using Assets;
using Assets.Scripts.Managers;
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

[CustomEditor(typeof(TurboForest))]
public class TurboForestEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var world = (TurboForest)target;
        var im = world.GetComponent<IslandManager>();
        if (GUILayout.Button("Generate Forest"))
        {
            world.GenerateForest(im.GetKdTree());
        }
    }
}