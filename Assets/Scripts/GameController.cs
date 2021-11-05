using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public static GameController link;

    public Transform prefabBullet;
    public Transform spawnPoint;

    [Header("UI")]
    public GameObject panelUI;
    public Transform textUI;
    public Text buttonText;

    [HideInInspector]
    public bool endGame = false;
    [HideInInspector]
    public bool win = false;

    List<Transform> bullets = new List<Transform>();
    int firedBullets = 0;
    int maxHoles;
    int curHoles;    

    void Awake() {
        link = this;
    }

    void Start() {
        CreateAllBullets();
        CalculateHolesNum();

        panelUI.SetActive(false);
    }

    void CreateAllBullets() {
        for (int i = 0; i < SettingsVIM.link.bulletMaxCount; i++) {
            CreateBullet(i);
        }

        StartCoroutine(FireBullet());
    }

    void CalculateHolesNum() {
        maxHoles = GameObject.FindGameObjectsWithTag("Hole").Length;
        curHoles = 0;
    }

    void CreateBullet(float step) {
        // Левый трамплин
        Vector3 position = new Vector3(spawnPoint.position.x - step, spawnPoint.position.y, spawnPoint.position.z);
        Transform bullet = Instantiate(prefabBullet, position, Quaternion.Euler(new Vector3(0, 180, 0)));

        /* Нижний трамплин
        Vector3 position = new Vector3(spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z - step);
        Transform bullet = Instantiate(prefabBullet, position, Quaternion.Euler(new Vector3(0, 0, 0)));
        */

        bullets.Add(bullet);
    }

    IEnumerator FireBullet() {
        yield return new WaitForSeconds(SettingsVIM.link.bulletFirePauseTime);

        if (!endGame) {
            bullets[0].GetComponent<Animator>().SetTrigger("Jump");
            //yield return new WaitForSeconds(0.3f);
            //bullets[0].GetComponent<Animator>().StopPlayback();

            bullets[0].GetComponent<Rigidbody>().isKinematic = false;
            //bullets[0].position = new Vector3(bullets[0].position.x + 1.5f, bullets[0].position.y, bullets[0].position.z);
            //bullets[0].GetComponent<Rigidbody>().velocity = new Vector3(1.75f, 2f, 0f); // левый трамплин
            bullets[0].GetComponent<Rigidbody>().velocity = new Vector3(2f, 2f, 0f); // левый трамплин
            //bullets[0].GetComponent<Rigidbody>().velocity = new Vector3(0f, 5f, 5f); // нижний трамплин

            bullets.RemoveAt(0);

            yield return new WaitForSeconds(0.1f);

            // Двигаем к концу вышки все патроны
            foreach (Transform bullet in bullets) {
                StartCoroutine(bullet.GetComponent<BulletController>().MoveOnce());
            }

            if (bullets.Count > 0) {
                StartCoroutine(FireBullet());
            }
        }
    }

    public void Strike(BulletController bullet) {
        curHoles++;
        BulletLost(bullet);
    }

    public void BulletLost(BulletController bullet) {
        if (!bullet.bulletLost) {
            firedBullets++;
            bullet.bulletLost = true;
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
                if (firedBullets == SettingsVIM.link.bulletMaxCount) {
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
        StartCoroutine(WinUI());
    }

    IEnumerator WinUI() {
        panelUI.SetActive(true);

        Vector3 zero  = Vector3.zero;
        Vector3 scale = new Vector3(1.1f, 1.1f, 1.1f);
        Vector3 norm  = new Vector3(1f, 1f, 1f);

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
}
