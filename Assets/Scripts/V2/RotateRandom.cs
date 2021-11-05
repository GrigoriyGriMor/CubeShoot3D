using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateRandom : MonoBehaviour {

    Quaternion _toRotation;

    // Start is called before the first frame update
    void Start() {
        _toRotation = Random.rotation;
        Destroy(gameObject, 5f);
    }

    // Update is called once per frame
    void Update() {
        transform.localRotation = Quaternion.Slerp(transform.localRotation, _toRotation, Mathf.Clamp01(Time.deltaTime * 2f));
    }
}
