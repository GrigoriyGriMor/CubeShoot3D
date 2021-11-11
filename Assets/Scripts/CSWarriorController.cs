using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSWarriorController : MonoBehaviour
{
    private static CSWarriorController instance;
    public static CSWarriorController Instance => instance;

    [Header("Fire Interface")]
    [SerializeField] private ShootGunController gun;
    [SerializeField] private GameObject GunObj;

    [Header("Nomber Chose")]
    private int fireObjCount = 0;

    private CastelController targetCastel;
    private Transform target;

    private bool fire = false;

    [Header("Сложность бота")]
    [Range(0.001f, 1)] [SerializeField] private float warriorHardcoreLevel = 0.25f;
    [SerializeField] private int minFireObjValue = 5;
    [SerializeField] private int maxFireObjValue = 20;

    public void SetTargetCastel(CastelController castel)
    {
        targetCastel = castel;
    }

    private void Awake()
    {
        fireObjCount = Random.Range(minFireObjValue, maxFireObjValue);
        gun.Init();
        instance = this;
    }

    public void StartPlayQueue()
    {
        if (!StaticGameController.Instance.gameIsPlayed) return;
        StopNumerical();
    }

    public void StopNumerical()
    {
        fire = true;
        StartCoroutine(FireToCastel());
    }

    private IEnumerator FireToCastel()
    {
        target = targetCastel.GetFreeTargetBlock();

        yield return new WaitForSeconds(1);

        GunObj.transform.LookAt(target);
        gun.Fire(fireObjCount, true);
        fireObjCount = 0;

        while (fire && target != null)
        {
            yield return new WaitForSeconds(warriorHardcoreLevel);
            target = targetCastel.GetFreeTargetBlock();

            if (target == null)
                gun.StopFire();
        }
    }

    private void FixedUpdate()
    {
        if (!StaticGameController.Instance.gameIsPlayed) return;

        if (fire)
            GunObj.transform.LookAt(target);
    }

    public void FireEnd()
    {
        fire = false;

        //StartCoroutine(EndGame());
        QueueController.Instance.NextQueue(false);
    }

   /* private IEnumerator EndGame()
    {
        yield return new WaitForSeconds(1);

        StaticGameController.Instance.GameEnded();
    }*/
}
