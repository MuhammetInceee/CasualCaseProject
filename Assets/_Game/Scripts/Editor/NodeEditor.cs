using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using YinYangCase.Editor.Interfaces;

namespace YinYangCase.Editor.Nodes
{
    public class NodeEditor : EditorWindow
    {
        private bool controlPressed;
        private Transform parentTransform;

        GameObject nodePrefab;
        GameObject previewNode;

        [MenuItem("Window/Node Editor &n")]
        public static void ShowWindow()
        {
            GetWindow<NodeEditor>("Node Editor");
        }

        void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            DestroyImmediate(previewNode);
        }

// Prefabların tutulduğu klasörün referansı
        private DefaultAsset prefabFolder;

// Prefabların listesi
        private SerializedObject prefabList;
        private SerializedProperty prefabArray;

// Prefab listesi penceresinin scroll pozisyonu
        private Vector2 scrollPos;

        void OnGUI()
        {
            GUILayout.Label("Node Settings", EditorStyles.boldLabel);

            // Prefab listesini seçmek için bir klasör seçici oluşturun
            prefabFolder =
                EditorGUILayout.ObjectField("Prefab Folder", prefabFolder, typeof(DefaultAsset), false) as DefaultAsset;

            // Eğer bir klasör seçilmişse prefab listesini yükle
            if (prefabFolder != null)
            {
                string path = AssetDatabase.GetAssetPath(prefabFolder);
                string[] assetGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] { path });
                GameObject[] prefabs = new GameObject[assetGUIDs.Length];
                for (int i = 0; i < assetGUIDs.Length; i++)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(assetGUIDs[i]);
                    prefabs[i] = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                }

                // Prefab listesini göstermek için bir ScrollView oluşturun
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                {
                    // Her prefab için bir düğme oluşturun
                    for (int i = 0; i < prefabs.Length; i++)
                    {
                        GameObject prefab = prefabs[i];
                        if (prefab == null) continue;

                        EditorGUILayout.BeginHorizontal();
                        {
                            // Prefab'ın önizlemesini gösteren bir ObjectField oluşturun
                            GameObject newPrefab = EditorGUILayout.ObjectField(
                                new GUIContent(prefab.name, AssetPreview.GetAssetPreview(prefab)),
                                prefab, typeof(GameObject), false) as GameObject;

                            if (newPrefab != prefab)
                            {
                                // Eğer prefab değiştiyse, previewNode'u yok et
                                if (previewNode != null)
                                {
                                    DestroyImmediate(previewNode);
                                }

                                prefab = newPrefab;
                            }

                            // Create Node Button
                            if (GUILayout.Button("Create Node", GUILayout.Width(100)))
                            {
                                nodePrefab = prefab;
                                if (previewNode)
                                {
                                    DestroyImmediate(previewNode);
                                }

                                GameObject node = Instantiate(nodePrefab, previewNode.transform.position,
                                    Quaternion.identity, CalculateParentTransform());
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndScrollView();
            }
        }


        private void CreateObj()
        {
            if (controlPressed)
                return;

            if (nodePrefab == null)
            {
                Debug.LogWarning("Node prefab is not assigned.");
                return;
            }

            if (previewNode != null)
            {
                // If a preview node already exists, destroy it and create the real node

                GameObject node = Instantiate(nodePrefab, previewNode.transform.position, Quaternion.identity,
                    CalculateParentTransform());
                Selection.SetActiveObjectWithContext(node, null);

                DestroyImmediate(previewNode);

                // Register the created object for undo
                Undo.RegisterCreatedObjectUndo(node, "Create Node");
            }
            else
            {
                // If there is no preview node, create one


                Vector3 nodePos = Vector3.zero;
                GameObject node = Instantiate(nodePrefab, nodePos, Quaternion.identity, CalculateParentTransform());
                Selection.SetActiveObjectWithContext(node, null);

                // Register the created object for undo
                Undo.RegisterCreatedObjectUndo(node, "Create Node");
            }
        }


        void OnSceneGUI(SceneView sceneView)
        {
            if (nodePrefab == null)
            {
                return;
            }

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                if (controlPressed)
                {
                    RemoveSelectionObject();
                }
                else
                {
                    CreateObj();
                }

                Debug.Log("Clicked");
            }

            GetNodePositionOnRaycast(sceneView);

            if (previewNode == null)
                return;

            controlPressed = Event.current.capsLock;

            // Set the cursor based on whether the control key is pressed or not
            if (controlPressed)
            {
                EditorGUIUtility.AddCursorRect(SceneView.lastActiveSceneView.position, MouseCursor.ArrowMinus);
            }
            else
            {
                EditorGUIUtility.AddCursorRect(SceneView.lastActiveSceneView.position, MouseCursor.Arrow);
            }

            previewNode.gameObject.SetActive(!controlPressed);

            // Repaint the scene view to show the cursor
            sceneView.Repaint();
        }

        private async void RemoveSelectionObject()
        {
            await Task.Delay(TimeSpan.FromSeconds(0.1));

            GameObject go = Selection.activeGameObject;

            if (!go.TryGetComponent(out ISpawnable spawnable))
                return;

            if (go == null)
            {
                return;
            }


            Undo.RegisterCompleteObjectUndo(go, "Remove Node");
            Undo.DestroyObjectImmediate(go);
        }

        private void GetNodePositionOnRaycast(SceneView sceneView)
        {
            if (controlPressed)
                return;

            // Get mouse position in the scene view
            Vector3 mousePos = Event.current.mousePosition;
            mousePos.y = sceneView.camera.pixelHeight - mousePos.y;
            Ray ray = sceneView.camera.ScreenPointToRay(mousePos);
            RaycastHit hit;

            // Raycast to the ground to get the node position
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Vector3 nodePos = hit.point;
                //     nodePos += spawnObjectOffset;

                // Destroy the preview node if it already exists
                if (previewNode != null)
                {
                    previewNode.transform.position = nodePos;
                }
                else
                {
                    previewNode = Instantiate(nodePrefab, nodePos, Quaternion.identity, CalculateParentTransform());
                }
            }
        }

        private Transform CalculateParentTransform()
        {
            if (GameObject.Find("LevelParent") == null)
            {
                Transform level = new GameObject("LevelParent").GetComponent<Transform>();
                parentTransform = level;
            }
            else
            {
                parentTransform = GameObject.Find("LevelParent").GetComponent<Transform>();
            }

            return parentTransform;
        }
    }
}