using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

    [HideInInspector]
    public BulletState bulletState;
    [HideInInspector]
    public bool rotate = false;
    [HideInInspector]
    public Vector3 targetRotation;
    [HideInInspector]
    public bool bulletLost = false;

    public List<GameObject> saveWaterObjects;
    public List<Material> shorts;

    public enum BulletState { Normal, Strike, Crash }

    Transform _transform;
    Animator _animator;
    Rigidbody _rigidbody;
    SkinnedMeshRenderer _skinnedMesh;

    void Awake() {
        _transform   = transform;
        _animator    = GetComponent<Animator>();
        _rigidbody   = GetComponent<Rigidbody>();
        _skinnedMesh = GetComponentInChildren<SkinnedMeshRenderer>();
    }

    void Start() {
        foreach (GameObject go in saveWaterObjects) {
            go.SetActive(false);
        }

        int rnd = Random.Range(0, shorts.Count);
        Material[] mats = _skinnedMesh.materials;
        mats[1] = shorts[rnd];
        _skinnedMesh.materials = mats;

    }

    void OnCollisionEnter(Collision collision) {
        if (bulletState == BulletState.Normal) {
            // Столкновение со стеной
            if (collision.gameObject.CompareTag("Wall")) {
                bulletState = BulletState.Crash;
                rotate = false;
                StartCoroutine(LikeJelly());

                GameController.link.BulletLost(this);
            }
        }

        // Столкновение с водой
        if (collision.gameObject.CompareTag("Water")) {
            bulletState = BulletState.Crash;

            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;

            float yPos = collision.gameObject.name == "WaterUp" ? 8f : -0.25f;
            _transform.position = new Vector3(_transform.position.x, yPos, _transform.position.z);
            _transform.localEulerAngles = new Vector3(0f, 270f, 270f);
            _animator.SetBool("Swim", true);

            _rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            _rigidbody.isKinematic = false;

            // Показываем спасательные средства
            int rndSave = Random.Range(0, 2);
            if (rndSave == 0) {
                saveWaterObjects[0].SetActive(true);
            } else {
                saveWaterObjects[1].SetActive(true);
                saveWaterObjects[2].SetActive(true);
            }

            GameController.link.BulletLost(this);
        }
    }

    void OnTriggerEnter(Collider collider) {
        if (!GameController.link.endGame) {
            if (bulletState == BulletState.Normal) {
                // Столкновение с дыркой
                if (collider.gameObject.CompareTag("Hole")) {
                    bulletState = BulletState.Strike;
                    rotate = false;

                    collider.gameObject.GetComponentInChildren<ParticleSystem>().gameObject.SetActive(false);
                    FixBullet(collider.transform);

                    GameController.link.Strike(this);
                }
            }
        }
    }

    void FixBullet(Transform hole) {
        _rigidbody.isKinematic = true;

        _transform.position = new Vector3(hole.position.x, hole.position.y - 0.7f, hole.position.z + 0.1f);
        _transform.eulerAngles = new Vector3(0f, 90f, 0f);

        _animator.enabled = true;
        _animator.SetBool("Strike", true);
    }

    void Update() {
        if (rotate) {
            //transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, targetRotation, 5f * Time.deltaTime, 0.0F));
            _transform.rotation = Quaternion.LookRotation(targetRotation);
            _transform.eulerAngles = new Vector3(_transform.eulerAngles.x + 90f, _transform.eulerAngles.y, _transform.eulerAngles.z);
        }
    }

    public IEnumerator MoveOnce() {
        GetComponent<Animator>().SetTrigger("Walk");

        Vector3 startPosition = _transform.position;
        Vector3 targetPosition = new Vector3(_transform.position.x + 1f, _transform.position.y, _transform.position.z); // левый трамплин
        //Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1f); // нижний трамплин

        float elapsedTime = 0f;
        float totalTime   = 0.5f;

        while (elapsedTime < totalTime) {
            elapsedTime += Time.deltaTime;
            _transform.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / totalTime);
            yield return null;
        }

        _transform.position = targetPosition;
    }

    IEnumerator LikeJelly() {
        _rigidbody.isKinematic = true;
        _rigidbody.mass = 100f;

        Vector3 original = _transform.localScale;
        Vector3 target   = new Vector3(2.0f, 1.3f, 0.5f);
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
}
