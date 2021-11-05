using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatapultController : MonoBehaviour {

    public static CatapultController link;

    public Transform throwPoint;

    [HideInInspector]
    public bool bulletCharged;
    [HideInInspector]
    public Transform bullet;

    Vector3 mousePosClick;
    Vector3 bulletVelocity;
    Vector3 oldAngles;
    Vector2 oldVel;

    Transform _transform;
    Animator _anim;
    TrajectoryRenderer _trajectoryRenderer;

    bool canShoot;

    void Awake() {
        link = this;
        _transform = transform;
        _anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start() {
        _trajectoryRenderer = FindObjectOfType<TrajectoryRenderer>();
        _trajectoryRenderer.ClearTrajectory();
    }

    // Update is called once per frame
    void Update() {
        /*
        if (!bulletCharged) {
            return;
        }
        */

        if (Input.GetMouseButtonDown(0)) {
            mousePosClick = FindMousePos();
            oldAngles = new Vector3(_transform.eulerAngles.x, FixAngle(_transform.eulerAngles.y), _transform.eulerAngles.z);
            //canShoot = true;
        }

        if (!canShoot) {
            return;
        }

        
        if (Input.GetMouseButton(0)) {
            Vector3 mouseInWorld = FindMousePos();

            // ѕрицел по вертикали
            float distMouseY = mouseInWorld.y - mousePosClick.y;

            // ѕрицел по горизонтали
            float distMouseX = mouseInWorld.x - mousePosClick.x;

            float angleY = Mathf.Clamp(oldAngles.y + distMouseX * SettingsVIM.link.sensivityRotationBatut * 3f, -SettingsVIM.link.limitXRotationBatut, SettingsVIM.link.limitXRotationBatut);
            float angleFixY = FixAngle(angleY);
            _transform.eulerAngles = new Vector3(0f, angleFixY, 0f);

            //Debug.Log("mdistY: " + distMouseY + ", mdistX: " + distMouseX);

            float velX = Mathf.Clamp(oldVel.x + distMouseX * SettingsVIM.link.sensivityRotationBatut, SettingsVIM.link.minVelocityX, SettingsVIM.link.maxVelocityX); // прицел по горизонтали
            float velY = Mathf.Clamp(oldVel.y + distMouseY * SettingsVIM.link.sensivityRotationBatut, SettingsVIM.link.minVelocityY, SettingsVIM.link.maxVelocityY); // прицел по вертикали

            bulletVelocity = new Vector3(velX, velY, SettingsVIM.link.shootPowerBatut);

            if (bulletVelocity != Vector3.zero) {
                _trajectoryRenderer.DrawTrajectory(_transform.position, bulletVelocity);
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            Shoot();
            _trajectoryRenderer.ClearTrajectory();
            oldVel = bulletVelocity;
        }
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

    void Shoot() {
        _anim.SetTrigger("Attack");

        //  идаем перса
        Vector3 bulletVelocityFix = bulletVelocity == Vector3.zero ? new Vector3(Random.Range(-1f, -5f), Random.Range(-1f, -5f), Random.Range(7f, 15f)) : bulletVelocity;
        Vector3 correctingVelocity = (transform.position - bullet.transform.position) + bulletVelocityFix; // правим траекторию снар€да от места его отскока
        bullet.GetComponent<Rigidbody>().velocity = correctingVelocity;
        bullet.GetComponent<Rigidbody>().isKinematic = false;

        StartCoroutine(bullet.GetComponent<BulletControllerV2>().RotateAhead());
        //bullet.localEulerAngles = new Vector3(75f, bullet.localEulerAngles.y, bullet.localEulerAngles.z);

        //StartCoroutine(GameControllerV2.link.CameraIn(bullet));
        Camera.main.GetComponent<CameraState>().SetState(CameraState.State.CameraIn, bullet);

        bullet.SetParent(null);
        bullet = null;
        bulletCharged = false;
        canShoot = false;
    }

    public void ChargeBullet(Transform bullet) {
        //bulletCharged = true;
        canShoot = true;
        bullet.SetParent(_transform);
        this.bullet = bullet;
    }
}
