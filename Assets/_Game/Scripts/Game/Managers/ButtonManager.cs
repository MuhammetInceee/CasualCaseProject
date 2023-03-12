using UnityEngine;
using YinYangCase.Game.Nodes;

namespace YinYangCase.Game.Managers
{
    public class ButtonManager : MonoBehaviour
    {
        private NodeTypes _nodeTypes;
        
        public AnswerDataHolder dataHolder;

        private void GetReferences()
        {
            _nodeTypes = GetComponentInParent<NodeTypes>();
        }

        private void OnEnable()
        {
            GetReferences();
        }

        public void ButtonClick()
        {
            switch (dataHolder.type)
            {
                case AnswerType.Heal:
                    PerformHealingAction();
                    break;
        
                case AnswerType.Purify:
                    PerformPurifyAction();
                    break;
        
                case AnswerType.Leave:
                    break;
        
                case AnswerType.Treasure:
                    PerformTreasureAction();
                    break;
            }
    
            UIManager.PlayerStatUIRefresh.Invoke();
            _nodeTypes.CanvasCloser();
        }

        private void PerformHealingAction()
        {
            PlayerDataManager.SpendMoneyAction.Invoke(dataHolder.cost.value);
            PlayerDataManager.HealingAction.Invoke(dataHolder.effectValue.value);
        }

        private void PerformPurifyAction()
        {
            PlayerDataManager.SpendMoneyAction.Invoke(dataHolder.cost.value);
        }

        private void PerformTreasureAction()
        {
            PlayerDataManager.EarnMoneyAction.Invoke(dataHolder.effectValue.value);
        }

    }
}
