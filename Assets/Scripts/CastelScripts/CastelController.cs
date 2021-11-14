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

    [SerializeField] private ParticleSystem startParticle;

    private bool warCastel = false;

    public void Init(bool _warCastel)
    {
        warCastel = _warCastel;
        // привязываем gameEnd к GameEnded и передаем заданный параметр
        CastelSetting setting = StaticGameController.Instance.SetRefCastel(this, warCastel);
        if (!warCastel) CSWarriorController.Instance.SetTargetCastel(this);

        pointText = setting.Text;
        anim = setting.anim;

        for (int i = 0; i < transform.childCount; i++)
            if (transform.GetChild(i).GetComponent<BlockController>())
                blocks.Add(transform.GetChild(i).GetComponent<BlockController>());

        PointsControlls();

        if (warCastel)
            StartCoroutine(StartParticle());
    }

    private IEnumerator StartParticle()
    {
        yield return new WaitForSeconds(0.01f);
        startParticle.Play();
    }

    public void PointsControlls()
    {
        //if (havePoints <= 0) return;

        havePoints = 0;
        for (int i = 0; i < blocks.Count; i++)
            havePoints += blocks[i].healPoint;

        //if (anim != null) anim.SetTrigger("Play");
       // pointText.text = havePoints.ToString();

        if (havePoints <= 0 && warCastel)
        {
            CSPlayerController.Instance.gun.StopFire();
            havePoints = 1;
        }
    }

    public void UpPointInController(int point = 1)
    {
        if (anim != null) anim.SetTrigger("Play");
        pointText.text = (int.Parse(pointText.text) + point).ToString();
    }

    public Transform GetFreeTargetBlock()
    {
        List<BlockController> activeBlocks = new List<BlockController>();
        for (int i = 0; i < blocks.Count; i++)
        {
            if (blocks[i].gameObject.activeInHierarchy)
                activeBlocks.Add(blocks[i]);
        }

        if (activeBlocks.Count > 0)
            return activeBlocks[Random.Range(0, activeBlocks.Count)].transform;
        else
            return null;
    }
}

