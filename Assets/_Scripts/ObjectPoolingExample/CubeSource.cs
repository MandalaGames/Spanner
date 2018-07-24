using UnityEngine;
using Spanner;

public class CubeSource : MonoBehaviour {

    public float time;
    public float minForce;
    public float maxForce;

    private float _timer;

    private void Start() {
        _timer = Time.time;
    }

    private void Update() {
        if (Time.time - _timer >= time) {
            SpawnCube();
            _timer = Time.time;
        }
    }

    private void SpawnCube() {
        GameObject cubeObject = MasterObjectPooler.Instance.Get("Jay's Cubes");
        if (cubeObject != null) {
            cubeObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            float x = Random.Range(-1f, 1f);
            float z = Random.Range(-1f, 1f);
            float mag = Random.Range(minForce, maxForce);
            cubeObject.transform.position = transform.position;
            cubeObject.GetComponent<Rigidbody>().AddForce((new Vector3(x, 1f, z)) * mag);
        }
    }
}
