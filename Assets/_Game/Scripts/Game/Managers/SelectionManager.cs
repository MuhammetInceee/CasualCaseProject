using UnityEngine;
using YinYangCase.Game.Nodes;

namespace YinYangCase.Game.Managers
{
    public class SelectionManager : MonoBehaviour
    {
        private readonly int _layerToCheck = 6; 

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.layer == _layerToCheck)
                    {
                        if (hit.collider.gameObject.TryGetComponent(out NodeTypes node))
                        {
                            node.CanvasOpener();
                        }
                    }
                }
            }
        }
    }
}
