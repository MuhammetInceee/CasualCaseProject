using UnityEngine;
using YinYangCase.SOArchitecture;

namespace YinYangCase.Game.Nodes
{
    [CreateAssetMenu(fileName = "NodeDataHolder", menuName = "YinYangCase/Node/DataHolder")]
    public class NodeDataHolderSO : ScriptableObject
    {
        [SerializeField] private Sprite typeSprite;
        public Sprite TypeSprite => typeSprite;
        
        [SerializeField] private StringVariableSO creatureName;
        public StringVariableSO CreatureName => creatureName;
        
        [SerializeField] private StringVariableSO infoText;
        public StringVariableSO InfoText => infoText;

        [SerializeField] private AnswersSO answers;
        public AnswersSO Answers => answers;

    }
}
