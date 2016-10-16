using UnityEngine;
using System.Collections;

public class NumUp : MonoBehaviour {

	void Start () {
        Invoke("remove",2);
	}
    void Update() {
        transform.Translate(Vector3.up * Time.deltaTime);
    }
    void remove() {
        Destroy(gameObject);
    }
}
