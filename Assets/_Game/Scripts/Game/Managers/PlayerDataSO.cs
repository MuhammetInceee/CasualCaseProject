using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YinYangCase.Game.Managers
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "YinYangCase/MainPlayer/PlayerData")]
    public class PlayerDataSO : ScriptableObject
    {
        public float Health;
        public float Money;
        public float Stamina;
    }
}
