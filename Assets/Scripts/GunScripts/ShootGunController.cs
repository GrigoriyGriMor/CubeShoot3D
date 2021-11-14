using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootGunController : MonoBehaviour
{
    public int startFireObjCount = 20;
    [SerializeField] private TMPro.TextMeshProUGUI fireObjText; 

    [SerializeField] private Transform gunPos;
    [SerializeField] private GameObject fireObjPrefab;

    private Transform fireTarget;
    private List<GameObject> objPool = new List<GameObject>();

    [SerializeField] private Animator gunAnim;
    [SerializeField] private ParticleSystem gunFireParticle;

    [Header("miniCooldown Fire Time")]
    [SerializeField] private float fireCooldownTime = 0.5f;

    private int gunMagazineCount = 20;

    private bool fireOpen = false;

    public void Init(Transform target = null)
    {
        fireTarget = target;
        if (fireObjText != null) fireObjText.text = "x0";

        for (int i = 0; i < 15; i++)
        {
            GameObject _go = Instantiate(fireObjPrefab, Vector3.zero, Quaternion.identity, gameObject.transform.parent);
            objPool.Add(_go);
            _go.SetActive(false);
        }
    }

    private int bonuseFireObj = 0;
    public void SetFireObjText(int count)
    {
        bonuseFireObj += count;

        if (fireObjText != null) fireObjText.text = $"x{startFireObjCount + bonuseFireObj}";
    }

    private bool warrior;
    public void Fire(int fireObj, bool _warriorFire)
    {
        warrior = _warriorFire;
        gunMagazineCount = startFireObjCount + bonuseFireObj;
        if (fireObjText != null) fireObjText.text = $"x{gunMagazineCount}";
        StartCoroutine(FireOpen());
    }

    private bool animStartBeUse = false;
    public void GunAnimUse()
    {
      //  if (animStartBeUse) return;

      //  animStartBeUse = true;
      //  gunAnim.SetTrigger("Fireplus");
    }

    private bool stopFire = false;
    public void StopFire()
    {
        if (gunMagazineCount <= 0) return;

        stopFire = true;
    }

    private IEnumerator FireOpen()
    {
        if (fireOpen) yield break;

        fireOpen = true;

        bool bonusFO = false;
        int boardFOCount = gunMagazineCount - startFireObjCount;

        while (gunMagazineCount > 0 && !stopFire)
        {
            if (gunMagazineCount <= boardFOCount)
            {
                bonuseFireObj -= 1;
                bonusFO = true;
            } 

            gunMagazineCount -= 1;
            if (fireObjText != null) fireObjText.text = $"x{gunMagazineCount}";
            gunAnim.SetTrigger("Fire");

            if (gunFireParticle != null)
            {
                gunFireParticle.Stop();
                gunFireParticle.Play();
            }

            for (int i = 0; i < objPool.Count; i++)
            {
                if (!objPool[i].activeInHierarchy)
                {
                    objPool[i].SetActive(true);

                    if (fireTarget != null) objPool[i].GetComponent<PLFireObjController>().Init(fireTarget.position, bonusFO);
                    objPool[i].transform.position = gunPos.position;
                    objPool[i].transform.rotation = gunPos.rotation;
                    break;
                }
                else
                    if (i == objPool.Count - 1)
                {
                    objPool[0].SetActive(false);
                    yield return new WaitForFixedUpdate();
                    objPool[0].SetActive(true);

                    if (fireTarget != null) objPool[i].GetComponent<PLFireObjController>().Init(fireTarget.position, bonusFO);
                    objPool[0].transform.position = gunPos.position;
                    objPool[0].transform.rotation = gunPos.rotation;
                    break;
                }
            }

            yield return new WaitForSeconds(fireCooldownTime);
        }

        fireOpen = false;

        if (!warrior)
            CSPlayerController.Instance.FireEnd();
        else
            CSWarriorController.Instance.FireEnd();

        stopFire = false;
    }
}