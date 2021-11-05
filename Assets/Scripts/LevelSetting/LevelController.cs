using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [Header("Набор сцен")]
    [SerializeField] private GameObject[] scenesObj = new GameObject[1];


    [Header("Набор замков")]
    [SerializeField] private GameObject[] castelsObj = new GameObject[2];

   // [Header("Набор пушек")]
   // [SerializeField] private GameObject[] gunsObj = new GameObject[2];

    private void Start()
    {
        ScenePosController spc = Instantiate(scenesObj[Random.Range(0, scenesObj.Length)], Vector3.zero, Quaternion.identity).GetComponent<ScenePosController>();
        if (spc == null)
        {
            Debug.LogError("К основной сцене не прикреплен элемент ScenePosController");
            return;
        }

        int castelModelNomber = Random.Range(0, castelsObj.Length); 

        Instantiate(castelsObj[castelModelNomber], spc.GetPlayerCastelPos().position, spc.GetPlayerCastelPos().rotation).GetComponent<CastelController>().Init(false);
        Instantiate(castelsObj[castelModelNomber], spc.GetWarriorCastelPos().position, spc.GetWarriorCastelPos().rotation).GetComponent<CastelController>().Init(true);
    }
}
