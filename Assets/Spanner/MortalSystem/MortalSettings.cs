using UnityEngine;
using System;

namespace Spanner {
    [CreateAssetMenu(fileName = "MortalSettings", menuName = "Spanner/Mortal/Settings"), Serializable]
    public class MortalSettings : ScriptableObject {
        /// <summary>Should the health values be randomized or not (affects editor view)</summary>
        public bool randomizeHealth;
        /// <summary>If not randomized, what is the maximum health?</summary>
        public int maxHealth;
        /// <summary>If not randomized, what is the starting health value?</summary>
        public int startingHealth;
        /// <summary>If randomized, what is the maximum value for this object's max health?</summary>
        public int maxHealthCap;
        /// <summary>If randomized, what is the minimum value for this object's maximum health?</summary>
        public int minHealthCap;
        /// <summary>If randomized, what is the maximum starting damage?</summary>
        public int maxStartingHealth;
        /// <summary>If randomized, what is the minimum starting health?</summary>
        public int minStartingHealth;
        /// <summary>Should the settings be applied during the Mortal's Start() phase?</summary>
        public bool initializeOnStart;
    }
}
