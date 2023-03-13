using System.Collections;
using UnityEngine;
using YinYangCase.Editor.Interfaces;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;
using YinYangCase.Game.Managers;

namespace YinYangCase.Game.Nodes
{
    [SelectionBase, DisallowMultipleComponent]
    public class NodeTypes : MonoBehaviour, ISpawnable
    {
        private BoxCollider _collider;
        private ParticleSystem _particle;

        [SerializeField] private GameObject previousNode;

        // An array of neighbor nodes.
        [Header("About LineRenderer")] public GameObject[] neighborNodes;

        [Header("About User Interface")] public GameObject uIWorkspace;
        public float comeOutDuration = 1f;
        [SerializeField] private UIElements uIElements;


        [Header("About Character Data")] public NodeDataHolderSO nodeData;


        private void GetReferences()
        {
            _collider = GetComponent<BoxCollider>();
            _particle = GetComponentInChildren<ParticleSystem>();
        }

        private void InitVariables()
        {
            _collider.enabled = true;
            if (gameObject.layer == 6) _particle.Play();
        }

        private void CanvasInitializer()
        {
            uIElements.typeImage.sprite = nodeData.TypeSprite;
            uIElements.creatureNameText.text = nodeData.CreatureName.value;
            uIElements.infoText.text = nodeData.InfoText.value;
            ButtonInitializer();
        }

        private void ButtonInitializer()
        {
            AnswerDataHolder[] answerData = nodeData.Answers.AnswerData;

            foreach (AnswerDataHolder currentData in answerData)
            {
                Button button = Instantiate(uIElements.buttonPrefab, Vector3.zero, Quaternion.identity,
                    uIElements.buttonPanel.transform);

                TextMeshProUGUI buttonText = button.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

                float costValue = currentData.cost.value;
                float effectValue = currentData.effectValue.value;
                AnswerType typeValue = currentData.type;

                if (costValue != 0 && effectValue != 0)
                {
                    buttonText.text =
                        $"<color=#0A00FF>[{typeValue}] </color><color=#FF0000>{costValue} Gold: </color>{currentData.effect.value} {effectValue}";
                }
                else if (costValue != 0 && effectValue == 0)
                {
                    buttonText.text =
                        $"<color=#0A00FF>[{typeValue}] </color><color=#FF0000>{costValue} Gold: </color>{currentData.effect.value}";
                }
                else if (costValue == 0 && effectValue != 0)
                {
                    buttonText.text = $"<color=#0A00FF>[{typeValue}] </color>{currentData.effect.value} {effectValue}";
                }
                else
                {
                    buttonText.text = $"<color=#0A00FF>[{typeValue}] </color>{currentData.effect.value}";
                }

                button.GetComponent<ButtonManager>().dataHolder = currentData;
            }
        }


        private void Awake()
        {
            GetReferences();
            InitVariables();
            CanvasInitializer();
        }

        public void CanvasOpener()
        {
            uIWorkspace.SetActive(true);
            uIWorkspace.transform.localScale = Vector3.zero;
            StartCoroutine(ScaleChanger(uIWorkspace.transform, comeOutDuration, Vector3.one));
            gameObject.layer = LayerMask.NameToLayer("Default");
        }

        public void CanvasCloser()
        {
            StartCoroutine(ScaleChanger(uIWorkspace.transform, comeOutDuration, Vector3.zero, true));
        }

        IEnumerator ScaleChanger(Transform transform, float duration, Vector3 targetScale, bool shouldHide = false)
        {
            if (transform == null)
            {
                yield break;
            }

            Vector3 originalScale = transform.localScale;

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
                yield return null;
            }

            if (shouldHide)
            {
                transform.gameObject.SetActive(false);

                if (previousNode != null)
                {
                    NodeTypes node = previousNode.GetComponent<NodeTypes>();
                    if (node != null && node.neighborNodes != null)
                    {
                        foreach (GameObject neighbor in node.neighborNodes)
                        {
                            neighbor.layer = LayerMask.NameToLayer("Default");
                            ParticleSystem particleSystem = GetParticleSystem(neighbor);
                            if (particleSystem != null)
                            {
                                particleSystem.Stop();
                            }
                        }
                    }
                }
                else
                {
                    gameObject.layer = LayerMask.NameToLayer("Default");
                    ParticleSystem particleSystem = GetParticleSystem(gameObject);
                    if (particleSystem != null)
                    {
                        particleSystem.Stop();
                    }
                }

                if (neighborNodes != null)
                {
                    foreach (GameObject neighbor in neighborNodes)
                    {
                        neighbor.layer = LayerMask.NameToLayer("Selectable");
                        ParticleSystem particleSystem = GetParticleSystem(neighbor);
                        if (particleSystem != null)
                        {
                            particleSystem.Play();
                        }
                    }
                }
            }
        }

        private ParticleSystem GetParticleSystem(GameObject obj)
        {
            if (obj != null)
            {
                Transform childTransform = obj.transform.GetChild(1);
                if (childTransform != null)
                {
                    return childTransform.GetComponent<ParticleSystem>();
                }
            }

            return null;
        }


        // This function is called in the editor when the script is loaded or a value is changed in the inspector.
        private void OnValidate()
        {
            if (transform.GetComponentsInChildren<LineRenderer>().Length == neighborNodes.Length * 2) return;

            // Make sure that the neighbor nodes array is not null or empty.
            if (neighborNodes == null || neighborNodes.Length == 0)
            {
                Debug.LogWarning("Neighbor nodes list is empty. Please add a few nodes.");
                return;
            }

            // Load the line renderer material from the specified path.
            Material lineMaterial = Resources.Load<Material>("Editor/Materials/LineRenderMaterial");

            // Create a line renderer between each neighbor node and the current object.
            for (int i = 0; i < neighborNodes.Length; i++)
            {
                if (neighborNodes[i] != null)
                {
                    // Create a new game object to hold the line renderer.
                    GameObject lineRendererObj = new GameObject("LineRenderer");

                    // Add a LineRenderer component to the game object.
                    LineRenderer lineRenderer = lineRendererObj.AddComponent<LineRenderer>();

                    // Set the start and end widths of the line renderer.
                    lineRenderer.startWidth = 0.1f;
                    lineRenderer.endWidth = 0.1f;

                    // Set the material of the line renderer.
                    lineRenderer.material = lineMaterial;

                    // Set the order in layer of the line renderer to -1 to ensure that it is rendered behind other objects.
                    lineRenderer.sortingOrder = -1;

                    // Set the number of positions that the line renderer should have.
                    lineRenderer.positionCount = 2;

                    // Set the positions of the line renderer.
                    lineRenderer.SetPosition(0, transform.position);
                    lineRenderer.SetPosition(1, neighborNodes[i].transform.position);

                    // Set the parent of the line renderer object to the current object.
                    lineRendererObj.transform.parent = transform;
                }
            }
        }
    }
}

[System.Serializable]
public struct UIElements
{
    public Image typeImage;
    public TextMeshProUGUI creatureNameText;
    public TextMeshProUGUI infoText;

    [Header("About Button")] public GameObject buttonPanel;
    public Button buttonPrefab;
}