using UnityEditor;
using UnityEngine;

/// <summary>
/// Automates the Unity Scene setup process after the SOLID Pure C# architectural refactor.
/// Salvages serialized configuration references from the old GameManager/BoardGenerator
/// and safely injects them into the new GameBootstrapper and BoardCameraFitter.
/// </summary>
public class SceneRefactorSetup : EditorWindow
{
    [MenuItem("Trigon/Setup Refactored Scene")]
    public static void ShowWindow()
    {
        GetWindow<SceneRefactorSetup>("Scene Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Update Scene to Pure C# Architecture", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "This script will automatically attach GameBootstrapper and BoardCameraFitter to your scene, " +
            "salvaging orphan variable references from before the strict SRP decomposition.", MessageType.Info);

        if (GUILayout.Button("Setup Architecture Components", GUILayout.Height(40)))
        {
            SetupScene();
        }
    }

    private static void SetupScene()
    {
        // 1. Locate GameManager
        var gameManager = FindAnyObjectByType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("[Setup] Could not find GameManager in the active scene!");
            return;
        }

        Undo.RecordObject(gameManager.gameObject, "Setup Architecture Components");

        // 2. Attach GameBootstrapper
        var bootstrapper = gameManager.gameObject.GetComponent<GameBootstrapper>();
        if (bootstrapper == null)
        {
            bootstrapper = Undo.AddComponent<GameBootstrapper>(gameManager.gameObject);
            Debug.Log("[Setup] Added GameBootstrapper to GameManager GameObject.");
        }

        // 3. Salvage Configs from GameManager
        var gmSo = new SerializedObject(gameManager);
        var bsSo = new SerializedObject(bootstrapper);

        // GameManager old fields
        var logicConfigProp = gmSo.FindProperty("logicConfig");
        var gameViewConfigProp = gmSo.FindProperty("gameViewConfig");
        var boardGeneratorProp = gmSo.FindProperty("boardGenerator");

        if (logicConfigProp != null && logicConfigProp.objectReferenceValue != null)
            bsSo.FindProperty("logicConfig").objectReferenceValue = logicConfigProp.objectReferenceValue;

        if (gameViewConfigProp != null && gameViewConfigProp.objectReferenceValue != null)
            bsSo.FindProperty("gameViewConfig").objectReferenceValue = gameViewConfigProp.objectReferenceValue;

        BoardGenerator bg = null;
        if (boardGeneratorProp != null && boardGeneratorProp.objectReferenceValue != null)
        {
            bsSo.FindProperty("boardGenerator").objectReferenceValue = boardGeneratorProp.objectReferenceValue;
            bg = boardGeneratorProp.objectReferenceValue as BoardGenerator;
        }
        else
        {
            bg = FindAnyObjectByType<BoardGenerator>();
            if (bg != null) bsSo.FindProperty("boardGenerator").objectReferenceValue = bg;
        }

        bsSo.ApplyModifiedProperties();

        // 4. Locate BoardGenerator & Attach Fitter
        if (bg != null)
        {
            Undo.RecordObject(bg.gameObject, "Setup Architecture Components");

            var cameraFitter = bg.gameObject.GetComponent<BoardCameraFitter>();
            if (cameraFitter == null)
            {
                cameraFitter = Undo.AddComponent<BoardCameraFitter>(bg.gameObject);
                Debug.Log("[Setup] Added BoardCameraFitter to BoardGenerator GameObject.");
            }

            var bgSo = new SerializedObject(bg);
            var cfSo = new SerializedObject(cameraFitter);

            // Salvage old camera/rect from BoardGenerator
            var cameraProp = bgSo.FindProperty("_camera");
            var targetRectProp = bgSo.FindProperty("_targetRect");

            if (cameraProp != null && cameraProp.objectReferenceValue != null)
                cfSo.FindProperty("_camera").objectReferenceValue = cameraProp.objectReferenceValue;
            else
                cfSo.FindProperty("_camera").objectReferenceValue = Camera.main;

            if (targetRectProp != null && targetRectProp.objectReferenceValue != null)
                cfSo.FindProperty("_targetRect").objectReferenceValue = targetRectProp.objectReferenceValue;

            cfSo.ApplyModifiedProperties();

            // Register fitter to BoardGenerator
            bgSo.FindProperty("_cameraFitter").objectReferenceValue = cameraFitter;
            bgSo.ApplyModifiedProperties();
        }

        Debug.Log("<color=green>[Success]</color> Perfect! Scene architectural linkages reconstructed successfully. You can now press Play.");
    }
}
