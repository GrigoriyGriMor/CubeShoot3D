using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueController : MonoBehaviour
{
    private static QueueController instance;
    public static QueueController Instance => instance;

    [Header("Триггер в аним: Start")]
    //[SerializeField] private bool warriorFireFirst = false;

    [SerializeField] private Animator[] castleUIAnim = new Animator[3];

    private int roundNumber = -1;

    private void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        roundNumber = -1;
        NextQueue(false);
    }

    public void NextQueue(bool playerEndedFire)
    {
        if (playerEndedFire)
            CSWarriorController.Instance.StartPlayQueue();
        else
        {
            if (roundNumber >= 0 && castleUIAnim[roundNumber] != null)
                castleUIAnim[roundNumber].SetTrigger("Start");

            roundNumber = roundNumber + 1;

            if (roundNumber > 2)
                StaticGameController.Instance.GameEnded();
            else
                CSPlayerController.Instance.StartPlayQueue(roundNumber);
        }
    }
}
