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
   // [SerializeField] private NumericalAggregator choseNomberPanel;
   // [SerializeField] private Animator cameraAnim;
    private int fireObjCount = 15;

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
        gun.Init();
        instance = this;
    }

    public void StartPlayQueue()
    {
        if (!StaticGameController.Instance.gameIsPlayed) return;

       // choseNomberPanel.gameObject.SetActive(true);
       // if (cameraAnim != null) cameraAnim.SetTrigger("SelectNomber");
        fireObjCount = 0;
        // choseNomberPanel.StartSelection();

        //StartCoroutine(StopSelect());

        StopNumerical(Random.Range(minFireObjValue, maxFireObjValue));
    }

  /*  private IEnumerator StopSelect()
    {
        yield return new WaitForSeconds(1);
        choseNomberPanel.StopSelection();
    }*/

    public void StopNumerical(int count)
    {
        fire = true;
        fireObjCount = count;

        StartCoroutine(FireToCastel());
    }

    private IEnumerator FireToCastel()
    {
        target = targetCastel.GetFreeTargetBlock();
        yield return new WaitForSeconds(1);

        GunObj.transform.LookAt(target);
        gun.Fire(fireObjCount, true);

        while (fire)
        {
            yield return new WaitForSeconds(warriorHardcoreLevel);
            target = targetCastel.GetFreeTargetBlock();
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

        StartCoroutine(EndGame());
        //QueueController.Instance.NextQueue(false);
    }

    private IEnumerator EndGame()
    {
        yield return new WaitForSeconds(1);

        StaticGameController.Instance.GameEnded();
    }
}
