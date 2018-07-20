using UnityEngine;
using System.Collections;
using Spanner;

public class ReturnToPoolWhenBelowThreshold : MonoBehaviour {
    public float threshold = -50f;

    private void Update() {
        if (transform.position.y <= threshold) {
            MasterObjectPooler.Instance.Return(gameObject);
        }
    }
}
