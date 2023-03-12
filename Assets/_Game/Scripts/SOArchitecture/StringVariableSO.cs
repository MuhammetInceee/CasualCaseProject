using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YinYangCase.SOArchitecture
{
    [CreateAssetMenu(fileName = "StringVariable", menuName = "SOArchitecture/StringVariable")]
    public class StringVariableSO : ScriptableObject
    {
        public string value;
    }
}
