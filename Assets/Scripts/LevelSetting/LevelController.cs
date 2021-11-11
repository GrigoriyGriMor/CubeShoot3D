using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    private static LevelController instance;
    public static LevelController Instance => instance;

    [Header("Набор сцен")]
    [SerializeField] private GameObject[] scenesObj = new GameObject[1];


    [Header("Набор замков")]
    [SerializeField] private GameObject[] castelsObj = new GameObject[2];

    // [Header("Набор пушек")]
    // [SerializeField] private GameObject[] gunsObj = new GameObject[2];

    private GameObject[] currentCastles = new GameObject[2];

    private ScenePosController spc;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        spc = Instantiate(scenesObj[Random.Range(0, scenesObj.Length)], Vector3.zero, Quaternion.identity).GetComponent<ScenePosController>();
        if (spc == null)
        {
            Debug.LogError("К основной сцене не прикреплен элемент ScenePosController");
            return;
        }

         InitCastle();
    }

    private int castelModelNomber = 1000;
    public void InitCastle()
    {
        if (castelModelNomber == 1000)
            castelModelNomber = Random.Range(0, castelsObj.Length);
        else
        {
            int n = castelModelNomber;
            while (castelsObj.Length > 1 && castelModelNomber == n)
                castelModelNomber = Random.Range(0, castelsObj.Length);
        }

        for (int i = 0; i < currentCastles.Length; i++)
            if (currentCastles[i] != null)
                Destroy(currentCastles[i]);

        currentCastles[0] = Instantiate(castelsObj[castelModelNomber], spc.GetPlayerCastelPos().position, spc.GetPlayerCastelPos().rotation);
        currentCastles[0].GetComponent<CastelController>().Init(false);

        currentCastles[1] = Instantiate(castelsObj[castelModelNomber], spc.GetWarriorCastelPos().position, spc.GetWarriorCastelPos().rotation);
        currentCastles[1].GetComponent<CastelController>().Init(true);
    }
}
