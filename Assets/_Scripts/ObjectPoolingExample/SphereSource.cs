using UnityEngine;
using System.Collections;
using Spanner;

public class SphereSource : MonoBehaviour {

    public GameObject spherePrefab;
    public float time;
    public float minForce;
    public float maxForce;

    private float _timer;

    private void Start() {
        _timer = Time.time;
    }

    private void Update() {
        if (Time.time - _timer >= time) {
            SpawnSphere();
            _timer = Time.time;
        }
    }

    private void SpawnSphere() {
        GameObject sphereObject = MasterObjectPooler.Instance.Get(spherePrefab);
        if (sphereObject != null) {
            sphereObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            float x = Random.Range(-1f, 1f);
            float z = Random.Range(-1f, 1f);
            float mag = Random.Range(minForce, maxForce);
            sphereObject.transform.position = transform.position;
            sphereObject.GetComponent<Rigidbody>().AddForce((new Vector3(x, 1f, z)) * mag);
        }
    }
}
