using UnityEngine;
using System.Collections.Generic;
using System;

namespace Spanner {
    [Serializable, CreateAssetMenu(fileName = "SaveFile", menuName = "Spanner/SaveSystem/SaveFile")]
    public class SaveFile : ScriptableObject {
        public string fileName;
        public List<SaveItem> items;
    }
}
