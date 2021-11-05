using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatutController : MonoBehaviour {

    public static BatutController link;

    [HideInInspector]
    public bool canShoot;

    Vector3 mousePosClick;
    Vector3 bulletVelocity;
    Vector3 oldAngles;

    Transform _transform;
    TrajectoryRenderer _trajectoryRenderer;

    void Awake() {
        link = this;
        _transform = transform;
    }

    // Start is called before the first frame update
    void Start() {
        _trajectoryRenderer = FindObjectOfType<TrajectoryRenderer>();
        _trajectoryRenderer.ClearTrajectory();
    }

    // Update is called once per frame
    void Update() {
        //if (!canShoot) {
        //    return;
        //}

        if (Input.GetMouseButtonDown(0)) {
            mousePosClick = FindMousePos();
            oldAngles = new Vector3(_transform.eulerAngles.x, _transform.eulerAngles.y, FixAngle(_transform.eulerAngles.z));
        }

        if (Input.GetMouseButton(0)) {
            Vector3 mouseInWorld = FindMousePos();

            // ѕрицел по вертикали
            //float distMouseY = mousePosClick.y - mouseInWorld.y; // инверси€ Y
            float distMouseY = mouseInWorld.y - mousePosClick.y;
            float angleX = Mathf.Clamp(oldAngles.x + distMouseY * SettingsVIM.link.sensivityRotationBatut, SettingsVIM.link.limitMinYRotationBatut, SettingsVIM.link.limitMaxYRotationBatut);

            // ѕрицел по горизонтали
            float distMouseX = mousePosClick.x - mouseInWorld.x;
            float angleZ = Mathf.Clamp(oldAngles.z + distMouseX * SettingsVIM.link.sensivityRotationBatut, -SettingsVIM.link.limitXRotationBatut, SettingsVIM.link.limitXRotationBatut);

            //Debug.Log("mdistY: " + distMouseY + ", mdistX: " + distMouseX);

            _transform.eulerAngles = new Vector3(angleX, 0f, angleZ);

            //Vector3 v = new Vector3(Mathf.Cos(angleX), 0f, Mathf.Sin(angleZ));
            //float velocity = transform.forward * transform.eulerAngles * SettingsVIM.link.shootPowerBatut;
            //float velocity = v * SettingsVIM.link.shootPowerBatut;
            //Debug.Log("speed: " + Vector3.Magnitude(speed));

            float maxVelocityAxe = 25f; // ограничение velocity по ос€м
            //float angleFixZ = _transform.eulerAngles.z > 180f ? _transform.eulerAngles.z - 360f : _transform.eulerAngles.z; // фикс по углу дл€ расчета velocity
            float angleFixZ = FixAngle(_transform.eulerAngles.z); // фикс по углу дл€ расчета velocity

            float velX = Mathf.Clamp(angleFixZ * -1f / (SettingsVIM.link.limitXRotationBatut / maxVelocityAxe), -maxVelocityAxe, maxVelocityAxe); // прицел по горизонтали
            float velY = Mathf.Clamp(maxVelocityAxe - (_transform.eulerAngles.x / (SettingsVIM.link.limitMaxYRotationBatut / maxVelocityAxe)), 0f, maxVelocityAxe); // прицел по вертикали

            bulletVelocity = new Vector3(velX, velY, SettingsVIM.link.shootPowerBatut);

            //_trajectoryRenderer.DrawTrajectory(_transform.position, bulletVelocity);
        }

        if (bulletVelocity != Vector3.zero) {
            _trajectoryRenderer.DrawTrajectory(_transform.position, bulletVelocity);
        }

        //if (Input.GetMouseButtonUp(0)) {
            //_trajectoryRenderer.ClearTrajectory();
            //GameController.link.FireBullet();
        //}
    }

    Vector3 FindMousePos() {
        float enter;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        new Plane(Vector3.forward, _transform.position).Raycast(ray, out enter);
        return ray.GetPoint(enter);
    }

    float FixAngle(float angle) {
        return angle > 180f ? angle - 360f : angle;
    }

    void OnCollisionEnter(Collision collision) {
        StartCoroutine(Shoot(collision.gameObject));
    }

    IEnumerator Shoot(GameObject bullet) {
        SkinnedMeshRenderer skinnedBatut = GetComponentInChildren<SkinnedMeshRenderer>();
        bullet.GetComponent<Animator>().StopPlayback();

        // ѕрогибание батута
        float elapsedTime = 0f;
        float totalTime = 0.1f;

        Vector3 original = bullet.transform.position;
        Vector3 target   = new Vector3(bullet.transform.position.x, bullet.transform.position.y - 0.3f, bullet.transform.position.z);

        while (elapsedTime < totalTime) {
            elapsedTime += Time.deltaTime;

            float value = Mathf.Lerp(0, 100, elapsedTime / totalTime);
            skinnedBatut.SetBlendShapeWeight(0, value);

            bullet.transform.position = Vector3.Lerp(original, target, elapsedTime / totalTime);

            yield return null;
        }

        skinnedBatut.SetBlendShapeWeight(0, 100);

        //  идаем перса
        Vector3 bulletVelocityFix  = bulletVelocity == Vector3.zero ? new Vector3(Random.Range(-1f, -5f), Random.Range(-1f, -5f), Random.Range(7f, 15f)) : bulletVelocity;
        Vector3 correctingVelocity = (transform.position - bullet.transform.position) + bulletVelocityFix; // правим траекторию снар€да от места его отскока
        bullet.GetComponent<Rigidbody>().velocity = correctingVelocity;

        // TODO: вращение работает так себе
        //collision.gameObject.GetComponent<BulletController>().targetRotation = fixedVelocity;
        //collision.gameObject.GetComponent<BulletController>().rotate = true;

        // ќтгибание батута в нормальное положение
        elapsedTime = 0f;

        while (elapsedTime < totalTime) {
            elapsedTime += Time.deltaTime;

            float value = Mathf.Lerp(100, 0, elapsedTime / totalTime);
            skinnedBatut.SetBlendShapeWeight(0, value);

            yield return null;
        }

        skinnedBatut.SetBlendShapeWeight(0, 0);
    }
}
