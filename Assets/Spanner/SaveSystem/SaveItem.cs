using UnityEngine;
using System;

namespace Spanner {
    [Serializable]
    public class SaveItem {
        public string name;
        public Type type;
        
        private float _floatValue;
        private int _intValue;
        private string _stringValue;

        public object getValue() {
            switch (type) {
            default:
                return null;
            case Type.FLOAT:
                return _floatValue;
            case Type.INT:
                return _intValue;
            case Type.STRING:
                return _stringValue;
            }
        }

        public void SetValue(float f) {
            _floatValue = f;
        }

        public void SetValue(int i) {
            _intValue = i;
        }

        public void SetValue(string s) {
            _stringValue = s;
        }
    }

    public enum Type {
        FLOAT,
        INT,
        STRING
    }
}
