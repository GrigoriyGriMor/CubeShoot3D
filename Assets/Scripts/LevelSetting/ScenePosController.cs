using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePosController : MonoBehaviour
{
    [Header("����� ���������� �� ������ �����!")]
    [SerializeField] private Transform playerCastelPos;
    [SerializeField] private Transform warriorCastelPos;

    public Transform GetPlayerCastelPos()
    {
        return playerCastelPos;
    }

    public Transform GetWarriorCastelPos()
    {
        return warriorCastelPos;
    }
}
