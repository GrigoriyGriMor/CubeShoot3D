using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraState : MonoBehaviour {

    public State camState = State.Normal;

    Vector3 defaultPos;
    Transform _transform;
    Transform target;

    public enum State { Normal, CameraIn, CameraOut }

    // Start is called before the first frame update
    void Start() {
        _transform = transform;
        defaultPos = _transform.position;
    }

    // Update is called once per frame
    void Update() {
        if (camState == State.CameraIn) {
            Vector3 targetPosition = target.position + SettingsVIM.link.cameraOffset;
            _transform.position = Vector3.Lerp(_transform.position, targetPosition, SettingsVIM.link.cameraSpeed * Time.deltaTime);

            // Камера фикс
            float calcX = 1f + Mathf.Abs((-25f - _transform.position.z) / 5f);
            float limitX = Mathf.Clamp(_transform.position.x, -calcX, calcX);
            _transform.position = new Vector3(limitX, _transform.position.y, Mathf.Clamp(_transform.position.z, defaultPos.z, 20f));

        } else if (camState == State.CameraOut) {
            _transform.position = Vector3.Lerp(_transform.position, defaultPos, SettingsVIM.link.cameraSpeed / 4f * Time.deltaTime);
        }
    }

    public void SetState(State state, Transform target) {
        camState = state;
        this.target = target;
    }
}
