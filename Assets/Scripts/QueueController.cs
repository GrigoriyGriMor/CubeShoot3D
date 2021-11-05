using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueController : MonoBehaviour
{
    private static QueueController instance;
    public static QueueController Instance => instance;

    [Header("Кто стреляет первым")]
    [SerializeField] private bool warriorFireFirst = false;

    private void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        NextQueue(warriorFireFirst);
    }

    public void NextQueue(bool playerEndedFire)
    {
        if (playerEndedFire)
            CSWarriorController.Instance.StartPlayQueue();
        else
            CSPlayerController.Instance.StartPlayQueue();
    }
}
