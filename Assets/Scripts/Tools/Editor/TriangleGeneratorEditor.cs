using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TriangleGeneratorSO))]
public class TriangleSquareGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TriangleGeneratorSO script = (TriangleGeneratorSO)target;

        GUILayout.Space(10);
        if (GUILayout.Button("Generate Triangle (Edge = Square Side)", GUILayout.Height(35)))
        {
            script.Generate();
        }
    }
}