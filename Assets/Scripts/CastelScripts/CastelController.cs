using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class CastelController : MonoBehaviour
{
    private int havePoints;
    [SerializeField] private TextMeshProUGUI pointText;

    private List<BlockController> blocks = new List<BlockController>();

    [SerializeField] private Animator anim;

    [HideInInspector] public UnityEvent gameEnd;

    public void Init(bool warCastel)
    {
        // привязываем gameEnd к GameEnded и передаем заданный параметр
        CastelSetting setting = StaticGameController.Instance.SetRefCastel(this, warCastel);
        if (!warCastel) CSWarriorController.Instance.SetTargetCastel(this);

        pointText = setting.Text;
        anim = setting.anim;

        for (int i = 0; i < transform.childCount; i++)
            if (transform.GetChild(i).GetComponent<BlockController>())
                blocks.Add(transform.GetChild(i).GetComponent<BlockController>());

        PointsControlls();
    }

    public void PointsControlls()
    {
        havePoints = 0;
        for (int i = 0; i < blocks.Count; i++)
            havePoints += blocks[i].healPoint;

        if (anim != null) anim.SetTrigger("Play");
        pointText.text = havePoints.ToString();

        if (havePoints <= 0) gameEnd.Invoke();
    }

    public Transform GetFreeTargetBlock()
    {
        List<BlockController> activeBlocks = new List<BlockController>();
        for (int i = 0; i < blocks.Count; i++)
        {
            if (blocks[i].gameObject.activeInHierarchy)
                activeBlocks.Add(blocks[i]);
        }

        return activeBlocks[Random.Range(0, activeBlocks.Count)].transform;
    }
}

