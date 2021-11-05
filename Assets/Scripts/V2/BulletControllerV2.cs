using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControllerV2 : MonoBehaviour {

    [HideInInspector]
    public BulletState bulletState;
    [HideInInspector]
    public bool bulletLost = false;

    public List<GameObject> saveWaterObjects;
    public List<Material> shorts;
    public List<GameObject> hats;

    public enum BulletState { Normal, Strike, Crash }

    Transform _transform;
    Animator _animator;
    Rigidbody _rigidbody;
    SkinnedMeshRenderer _skinnedMesh;
    GameObject selectedHat;
    Transform isInDownWater;

    void Awake() {
        _transform = transform;
        _animator = GetComponentInChildren<Animator>();
        _rigidbody = GetComponent<Rigidbody>();
        _skinnedMesh = GetComponentInChildren<SkinnedMeshRenderer>();
        isInDownWater = null;
    }

    void Start() {
        foreach (GameObject go in saveWaterObjects) {
            go.SetActive(false);
        }

        // Рандомный выбор трусов
        if (shorts.Count > 0) {
            int rnd = Random.Range(0, shorts.Count);
            Material[] mats = _skinnedMesh.materials;
            mats[1] = shorts[rnd];
            _skinnedMesh.materials = mats;
        }

        // Рандомный выбор шляпы
        if (hats.Count > 0) {
            int rnd = Random.Range(0, hats.Count);            
            hats[rnd].SetActive(true);
            selectedHat = hats[rnd];
        }
    }

    void Update() {
        if (isInDownWater != null) {
            _transform.position = new Vector3(_transform.position.x, isInDownWater.position.y - 0.25f, _transform.position.z);
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (bulletState == BulletState.Normal) {
            // Столкновение со стеной
            if (collision.gameObject.CompareTag("Wall")) {
                bulletState = BulletState.Crash;
                StartCoroutine(LikeJellyV2());
                HatFall();

                GameControllerV2.link.BulletLost(this);
            }
        }

        // Столкновение с водой
        if (collision.gameObject.CompareTag("Water")) {
            bulletState = BulletState.Crash;

            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;

            isInDownWater = collision.gameObject.name == "WaterDown" ? collision.transform : null;
            float yPos = collision.gameObject.name == "WaterUp" ? 8f : -0.25f;
            _transform.position = new Vector3(_transform.position.x, yPos, _transform.position.z);
            _transform.localEulerAngles = new Vector3(270f, 0f, 0f);
            _animator.SetBool("Swim", true);

            //_rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            _rigidbody.isKinematic = false;

            // Показываем спасательные средства
            if (saveWaterObjects.Count > 0) {
                int rndSave = Random.Range(0, 2);
                if (rndSave == 0) {
                    saveWaterObjects[0].SetActive(true);
                } else {
                    saveWaterObjects[1].SetActive(true);
                    saveWaterObjects[2].SetActive(true);
                }
            }

            GameControllerV2.link.BulletLost(this);
        }
    }

    void OnTriggerEnter(Collider collider) {
        if (!GameControllerV2.link.endGame) {
            if (bulletState == BulletState.Normal) {
                // Столкновение с дыркой
                if (collider.gameObject.CompareTag("Hole")) {
                    collider.gameObject.GetComponent<Hole>().closed = true;

                    bulletState = BulletState.Strike;

                    collider.gameObject.GetComponentInChildren<ParticleSystem>().gameObject.SetActive(false);
                    collider.GetComponentInChildren<MeshRenderer>().material = GameControllerV2.link.holeClosed;

                    FixBullet(collider.transform);

                    GameControllerV2.link.Strike(this);
                }
            }
        }
    }

    void FixBullet(Transform hole) {
        _rigidbody.isKinematic = true;

        _transform.position = new Vector3(hole.position.x, hole.position.y - 0.7f, hole.position.z + 0.1f);
        _transform.eulerAngles = new Vector3(0f, 0f, 0f);

        _animator.enabled = true;
        _animator.SetBool("Strike", true);
    }

    public IEnumerator RotateAhead() {
        float elapsedTime = 0f;
        float totalTime = 0.5f;

        float start  = _transform.localEulerAngles.x;
        float target = 75f;

        while (elapsedTime < totalTime) {
            elapsedTime += Time.deltaTime;            
            float x = Mathf.Lerp(start, target, elapsedTime / totalTime);
            _transform.localEulerAngles = new Vector3(x, _transform.localEulerAngles.y, _transform.localEulerAngles.z);

            yield return null;
        }
    }

    IEnumerator LikeJellyV2() {
        SkinnedMeshRenderer skinnedMesh = GetComponentInChildren<SkinnedMeshRenderer>();

        _rigidbody.isKinematic = true;
        _rigidbody.mass = 100f;

        float elapsedTime = 0f;
        float totalTime = SettingsVIM.link.timeWallSplash;

        while (elapsedTime < totalTime) {
            elapsedTime += Time.deltaTime;

            float value = Mathf.Lerp(0, 55, elapsedTime / totalTime);
            skinnedMesh.SetBlendShapeWeight(0, value);

            yield return null;
        }

        yield return new WaitForSeconds(SettingsVIM.link.timePauseFall);

        elapsedTime = 0f;
        while (elapsedTime < totalTime) {
            elapsedTime += Time.deltaTime;

            float value = Mathf.Lerp(55, 0, elapsedTime / totalTime);
            skinnedMesh.SetBlendShapeWeight(0, value);

            yield return null;
        }

        _rigidbody.isKinematic = false;
    }

    void HatFall() {
        selectedHat.transform.SetParent(null);
        selectedHat.transform.position = new Vector3(_transform.position.x, _transform.position.y, _transform.position.z - 2f);
        selectedHat.AddComponent<Rigidbody>();
        selectedHat.GetComponent<BoxCollider>().enabled = true;
    }

    /*
    IEnumerator LikeJelly() {
        _rigidbody.isKinematic = true;
        _rigidbody.mass = 100f;

        Vector3 original = _transform.localScale;
        Vector3 target = new Vector3(2.0f, 1.3f, 0.5f);
        float elapsedTime = 0f;
        float totalTime = 0.1f;

        while (elapsedTime < totalTime) {
            elapsedTime += Time.deltaTime;
            _transform.localScale = Vector3.Lerp(original, target, elapsedTime / totalTime);
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        elapsedTime = 0f;
        while (elapsedTime < totalTime) {
            elapsedTime += Time.deltaTime;
            _transform.localScale = Vector3.Lerp(target, original, elapsedTime / totalTime);
            yield return null;
        }

        _rigidbody.isKinematic = false;
    }
    */
}
