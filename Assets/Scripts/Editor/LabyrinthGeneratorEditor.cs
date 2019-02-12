using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LabyrinthGenerator))]
public class LabyrinthGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LabyrinthGenerator labGen = (LabyrinthGenerator)target;
        if (GUILayout.Button("Generate Labyrinth"))
        {
            labGen.GenerateLabyrinthButton();
        }
    }
}
