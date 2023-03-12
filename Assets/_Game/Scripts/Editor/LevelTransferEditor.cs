using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Unity Editor Window'da bir seçim menüsü oluşturur ve seçilen Gameobject'in belirli bir sahneye aktarılmasına olanak sağlar
public class LevelTransferEditor : EditorWindow {
    private GameObject selectedObject;
    private string[] sceneNames;
    private int selectedSceneIndex;

    [MenuItem("Window/Level Transfer &l")]
    public static void ShowWindow() {
        GetWindow<LevelTransferEditor>("Level Transfer");
    }

    private void OnGUI() {
        // Seçilen Gameobject'i al
        selectedObject = Selection.activeGameObject;

        // Mevcut sahnenin adını al
        string currentScene = SceneManager.GetActiveScene().name;

        // Sahne isimlerini al
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        sceneNames = new string[sceneCount];
        for (int i = 0; i < sceneCount; i++) {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            sceneNames[i] = System.IO.Path.GetFileNameWithoutExtension(scenePath);
        }

        // Seçim menüsünü oluştur
        GUILayout.Label("Selected Object: " + (selectedObject != null ? selectedObject.name : "None"));
        GUILayout.Label("Current Scene: " + currentScene);
        selectedSceneIndex = EditorGUILayout.Popup("Transfer Object to Scene:", selectedSceneIndex, sceneNames);

        // Objenin sahneye taşınması için düğme
        if (selectedObject != null && GUILayout.Button("Transfer Object")) {
            // Seçilen sahne adını al
            string selectedScene = sceneNames[selectedSceneIndex];

            // Belirtilen sahne var mı diye kontrol et
            bool sceneExists = false;
            for (int i = 0; i < sceneCount; i++) {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                if (System.IO.Path.GetFileNameWithoutExtension(scenePath) == selectedScene) {
                    sceneExists = true;
                    break;
                }
            }

            // Eğer belirtilen sahne varsa, objeyi o sahneye taşı
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
        // Kaydedilen prefabı farklı bir sahneye aktar
        string prefabPath = "Assets/Resources/Levels/" + Selection.activeGameObject.name + ".prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;

        if (prefab != null)
        {
            // Farklı bir sahne aç
            EditorSceneManager.OpenScene($"Assets/Scenes/{selectedScene}.unity", OpenSceneMode.Single);

            // Prefab içeriğini yükle
            GameObject instance = PrefabUtility.LoadPrefabContents(prefabPath);

            // Prefabı sahneye ekle
            PrefabUtility.InstantiatePrefab(prefab);

            // Prefab içeriğini kaydet ve yükleme kaynaklarını temizle
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
