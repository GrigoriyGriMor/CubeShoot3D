using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameControllerV2 : MonoBehaviour {

    public static GameControllerV2 link;

    [Header("UI")]
    public GameObject panelUI;
    public Transform textUI;
    public Text buttonText;

    [Header("Прочее")]
    public Material holeClosed;

    [Header("Дамба")]
    public GameObject damba;
    public GameObject dambaDestroyed;
    public List<GameObject> dambaParts;
    public GameObject explosion;
    public List<Transform> explosionPlaces;
    public GameObject waterfall;


    [HideInInspector]
    public bool endGame = false;
    [HideInInspector]
    public bool win = false;

    List<Transform> bullets = new List<Transform>();
    int firedBullets = 0;
    int maxHoles;
    int curHoles;
    int bulletMaxCount;
    Vector3 cameraDefault;
    bool blockCharge;

    void Awake() {
        link = this;
        cameraDefault = Camera.main.transform.position;
    }

    void Start() {
        CreateAllBullets();
        CalculateHolesNum();

        panelUI.SetActive(false);
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            //if (!CatapultController.link.bulletCharged && !blockCharge && bullets.Count > 0) {
            if (!blockCharge && bullets.Count > 0) {
                blockCharge = true;
                StartCoroutine(ChargeBullet());
            }
        }

        // ТЕСТ
        if (Input.GetKeyDown(KeyCode.A)) {
            LooseGame();
        }
    }

    void CreateAllBullets() {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject go in gos) {
            bullets.Add(go.transform);
        }

        bullets = bullets.OrderBy((d) => Vector3.Distance(CatapultController.link.throwPoint.position, d.position)).ToList();
        bulletMaxCount = bullets.Count;
    }

    void CalculateHolesNum() {
        maxHoles = GameObject.FindGameObjectsWithTag("Hole").Length;
        curHoles = 0;
    }

    IEnumerator ChargeBullet() {
        if (!endGame) {
            float dist = Vector3.Distance(CatapultController.link.throwPoint.position, bullets[0].position);
            
            Vector3 startPos = bullets[0].position;
            Vector3 targetPos = CatapultController.link.throwPoint.position;

            float elapsedTime = 0f;
            float totalTime = 0.3f;

            bullets[0].GetComponentInChildren<Animator>().SetTrigger("Walk");

            while (elapsedTime < totalTime) {
                elapsedTime += Time.deltaTime;

                if (dist > 0.2f) {
                    bullets[0].position = Vector3.Lerp(startPos, targetPos, elapsedTime / totalTime);
                }

                bullets[0].LookAt(targetPos, Vector3.up);
                bullets[0].eulerAngles = new Vector3(0f, bullets[0].eulerAngles.y, 0f);

                dist = Vector3.Distance(CatapultController.link.throwPoint.position, bullets[0].position);
                yield return null;
            }

            //bullets[0].GetComponentInChildren<Animator>().SetTrigger("In catapult");

            CatapultController.link.ChargeBullet(bullets[0]);

            bullets[0].localEulerAngles = Vector3.zero;

            bullets.RemoveAt(0);

            yield return new WaitForSeconds(0.1f);
        }
    }

    /*
    public IEnumerator CameraIn(Transform bullet) {
        Vector3 start = Camera.main.transform.position;

        float elapsedTime = 0f;
        float totalTime = SettingsVIM.link.cameraSpeed;

        while (elapsedTime < totalTime) {
            elapsedTime += Time.deltaTime;
            Vector3 target = bullet.position + SettingsVIM.link.cameraOffset;
            Camera.main.transform.position = Vector3.Lerp(start, target, elapsedTime / totalTime);

            // Камера фикс
            float calcX = 1f + Mathf.Abs((-25f - Camera.main.transform.position.z) / 5f);
            float limitX = Mathf.Clamp(Camera.main.transform.position.x, -calcX, calcX);
            Camera.main.transform.position = new Vector3(limitX, Camera.main.transform.position.y, Camera.main.transform.position.z);

            yield return null;
        }
    }

    public IEnumerator CameraOut() {
        Vector3 start  = Camera.main.transform.position;
        Vector3 target = cameraDefault;

        float elapsedTime = 0f;
        float totalTime = SettingsVIM.link.cameraSpeed;

        while (elapsedTime < totalTime) {
            elapsedTime += Time.deltaTime;
            Camera.main.transform.position = Vector3.Lerp(start, target, elapsedTime / totalTime);

            yield return null;
        }

        blockCharge = false;
    }
    */

    public void Strike(BulletControllerV2 bullet) {
        curHoles++;
        BulletLost(bullet);
    }

    public void BulletLost(BulletControllerV2 bullet) {
        if (!bullet.bulletLost) {
            firedBullets++;
            bullet.bulletLost = true;

            //StartCoroutine(CameraOut());
            Camera.main.GetComponent<CameraState>().SetState(CameraState.State.CameraOut, null);
            blockCharge = false;
        }

        CheckEndGame();
    }

    public void CheckEndGame() {
        if (!endGame) {
            // Проверка на победу
            if (curHoles == maxHoles) {
                WinGame();

            } else {
                // Проверка на поражение
                if (firedBullets == bulletMaxCount) {
                    LooseGame();
                }
            }
        }
    }

    void WinGame() {
        endGame = true;
        win = true;
        StartCoroutine(WinUI());
    }

    void LooseGame() {
        endGame = true;
        StartCoroutine(DambaDestroy());
    }

    IEnumerator WinUI() {
        panelUI.SetActive(true);

        Vector3 zero = Vector3.zero;
        Vector3 scale = new Vector3(1.1f, 1.1f, 1.1f);
        Vector3 norm = new Vector3(1f, 1f, 1f);

        buttonText.text = win ? "NEXT LEVEL" : "RESTART";

        textUI.GetComponent<Text>().text = win ? "VICTORY!" : "FAILED!";
        textUI.localScale = zero;

        float elapsedTime = 0f;
        float totalTime = 1f;

        while (elapsedTime < totalTime) {
            elapsedTime += Time.deltaTime;
            textUI.localScale = Vector3.Lerp(zero, scale, elapsedTime / totalTime);
            yield return null;
        }

        elapsedTime = 0f;

        while (elapsedTime < totalTime) {
            elapsedTime += Time.deltaTime;
            textUI.localScale = Vector3.Lerp(scale, norm, elapsedTime / totalTime);
            yield return null;
        }
    }

    IEnumerator DambaDestroy() {
        yield return new WaitForSeconds(0.5f);

        // Расширяем незакрытые дырки и увеличиваем струю
        List<Transform> _listT = new List<Transform>();
        List<Transform> _listP = new List<Transform>();

        GameObject[] holes = GameObject.FindGameObjectsWithTag("Hole");
        foreach (GameObject hole in holes) {
            if (!hole.GetComponent<Hole>().closed) {
                _listT.Add(hole.transform);
                _listP.Add(hole.GetComponentInChildren<ParticleSystem>().transform);
            }
        }

        float elapsedTime = 0f;
        float totalTime = SettingsVIM.link.timeHolesScale;

        while (elapsedTime < totalTime) {
            elapsedTime += Time.deltaTime;

            foreach (Transform tr in _listT) {
                tr.localScale = Vector3.Lerp(new Vector3(0.5f, 0.5f, 0.5f), Vector3.one, elapsedTime / totalTime);
            }

            foreach (Transform pr in _listP) {
                pr.localScale = Vector3.Lerp(Vector3.one, new Vector3(3f, 3f, 3f), elapsedTime / totalTime);
            }

            yield return null;
        }


        // Взрывы
        foreach (Transform tr in explosionPlaces) {
            Instantiate(explosion, tr.position, Quaternion.identity);
        }

        // Тряска камеры
        StartCoroutine(CameraShock());

        // Подмена моделей (для разрушения)
        damba.SetActive(false);
        dambaDestroyed.SetActive(true);

        // Отлетающие части дамбы
        foreach (GameObject part in dambaParts) {
            /*
            Vector3 target = Camera.main.transform.position - part.transform.position;
            Vector3 targetRnd = new Vector3
                (
                    Random.Range(target.x - 10f, target.x + 10f),
                    Random.Range(target.y - 10f, target.y + 10f),
                    Random.Range(-3f, -5f)
                );
            part.GetComponent<Rigidbody>().velocity = targetRnd;
            */

            Vector3 targetRnd = new Vector3
                (
                    Random.Range(part.transform.position.x - 1f, part.transform.position.x + 1f),
                    Random.Range(part.transform.position.y - 1f, part.transform.position.y + 1f),
                    Random.Range(-1f, -2f)
                );

            part.GetComponent<Rigidbody>().isKinematic = false;
            part.GetComponent<Rigidbody>().velocity = targetRnd;
            part.GetComponent<RotateRandom>().enabled = true;
        }        

        // Скрываем дырки
        //GameObject[] holes = GameObject.FindGameObjectsWithTag("Hole");
        foreach (GameObject hole in holes) {
            hole.SetActive(false);
        }

        // Скрываем толстяков в дырках
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in bullets) {
            if (bullet.GetComponent<BulletControllerV2>().bulletState == BulletControllerV2.BulletState.Strike) {
                bullet.SetActive(false);
            }
        }        

        // Водопад
        waterfall.SetActive(true);

        yield return new WaitForSeconds(SettingsVIM.link.timePauseAfterDambaExplosion);

        // Подъем воды
        Transform waterDown = GameObject.Find("WaterDown").transform;
        Vector3 start = waterDown.position;
        Vector3 target = new Vector3(waterDown.position.x, waterDown.position.y + 8f, waterDown.position.z);

        elapsedTime = 0f;
        totalTime = SettingsVIM.link.timeWaterRise;

        while (elapsedTime < totalTime) {
            elapsedTime += Time.deltaTime;
            waterDown.position = Vector3.Lerp(start, target, elapsedTime / totalTime);
            yield return null;
        }

        CatapultController.link.GetComponent<Animator>().SetBool("Death", true);

        // Показываем UI проигрыша
        StartCoroutine(WinUI());
    }

    IEnumerator CameraShock() {
        Vector3 defaultPos = Camera.main.transform.position;
        Vector3 pos1 = defaultPos + new Vector3(-1f, -1f, 0);
        Vector3 pos2 = defaultPos + new Vector3(1f, 1f, 0);

        float elapsedTime = 0f;
        float totalTime   = SettingsVIM.link.cameraShockTime;

        while (elapsedTime < totalTime) {
            elapsedTime += Time.deltaTime;
            float pingPong = Mathf.PingPong(Time.time * SettingsVIM.link.cameraShockSpeed, 1);
            Camera.main.transform.position = Vector3.Lerp(pos1, pos2, pingPong);

            yield return null;
        }

        Camera.main.transform.position = defaultPos;
    }
}
