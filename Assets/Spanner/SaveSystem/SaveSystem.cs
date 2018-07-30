using UnityEngine;
using System.Collections.Generic;
using System;

namespace Spanner {
    [Serializable]
    public class SaveSystem : MonoBehaviour {
        [SerializeField]
        public SaveFile saveFile;

        private static SaveSystem _instance;
        public static SaveSystem Instance {
            get {
                if (_instance == null) {
                    Debug.LogWarning("You didn't manually set up your Save System yet. Creating blank Save System");
                    GameObject go = new GameObject("Save System");
                    _instance = go.AddComponent<SaveSystem>();
                }
                return _instance;
            }
        }

        private void OnEnable() {
            _instance = this;
            DontDestroyOnLoad(_instance);
        }
    }
}
