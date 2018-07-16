using UnityEngine;
using System.Collections;

namespace Spanner {
    [CreateAssetMenu(fileName = "MortalSettings", menuName = "Settings/Mortal")]
    public class MortalSettings : ScriptableObject {
        public int maxHealth;
        public int startingHealth;
    }
}
