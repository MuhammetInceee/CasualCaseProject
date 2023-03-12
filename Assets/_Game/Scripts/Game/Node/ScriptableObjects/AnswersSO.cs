using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YinYangCase.SOArchitecture;

namespace YinYangCase.Game.Nodes
{
    [CreateAssetMenu(fileName = "AnswerTypeSO", menuName = "YinYangCase/Node/AnswerSO")]
    public class AnswersSO : ScriptableObject
    {
        [SerializeField] private AnswerDataHolder[] answerData;
        public AnswerDataHolder[] AnswerData => answerData;
    }

    [System.Serializable]
    public class AnswerDataHolder
    {
        public AnswerType type;
        public FloatVariableSO cost;
        public StringVariableSO effect;
        public FloatVariableSO effectValue;
    }

    public enum AnswerType
    {
        Heal,
        Purify,
        Leave,
        Treasure
    }
}
