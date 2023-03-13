using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransferEditor : EditorWindow {
    private GameObject selectedObject;
    private string[] sceneNames;
    private int selectedSceneIndex;

    [MenuItem("Window/Level Transfer &l")]
    public static void ShowWindow() {
        GetWindow<LevelTransferEditor>("Level Transfer");
    }

    private void OnGUI() {
        selectedObject = Selection.activeGameObject;

        string currentScene = SceneManager.GetActiveScene().name;

        int sceneCount = SceneManager.sceneCountInBuildSettings;
        sceneNames = new string[sceneCount];
        for (int i = 0; i < sceneCount; i++) {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            sceneNames[i] = System.IO.Path.GetFileNameWithoutExtension(scenePath);
        }

        GUILayout.Label("Selected Object: " + (selectedObject != null ? selectedObject.name : "None"));
        GUILayout.Label("Current Scene: " + currentScene);
        selectedSceneIndex = EditorGUILayout.Popup("Transfer Object to Scene:", selectedSceneIndex, sceneNames);

        if (selectedObject != null && GUILayout.Button("Transfer Object")) {
            string selectedScene = sceneNames[selectedSceneIndex];

            bool sceneExists = false;
            for (int i = 0; i < sceneCount; i++) {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                if (System.IO.Path.GetFileNameWithoutExtension(scenePath) == selectedScene) {
                    sceneExists = true;
                    break;
                }
            }

            if (sceneExists)
            {
                CreatePreafab();

                TransferAnotherScene(selectedScene);
            }
            else {
                EditorUtility.DisplayDialog("Scene not found", "The selected scene could not be found.", "OK");
            }
        }
    }

    private static void TransferAnotherScene(string selectedScene)
    {
        string prefabPath = "Assets/Resources/Levels/" + Selection.activeGameObject.name + ".prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;

        if (prefab != null)
        {
            EditorSceneManager.OpenScene($"Assets/Scenes/{selectedScene}.unity", OpenSceneMode.Single);

            GameObject instance = PrefabUtility.LoadPrefabContents(prefabPath);

            PrefabUtility.InstantiatePrefab(prefab);

            PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
            PrefabUtility.UnloadPrefabContents(instance);
            
        }
    }

    [Obsolete("Obsolete")]
    private static void CreatePreafab()
    {
        GameObject obj = Selection.activeGameObject;
        string prefabPath2 = "Assets/Resources/Levels/" + obj.name + ".prefab";
        PrefabUtility.CreatePrefab(prefabPath2, obj);
    }
}
