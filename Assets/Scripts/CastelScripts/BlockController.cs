using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlockController : MonoBehaviour
{
    [Header("HealPointsValue")] [Range(1, 99)]
    public int healPoint = 15;

    [SerializeField] private MeshRenderer material;
    [SerializeField] private TextMeshPro text;

    [SerializeField] private ParticleSystem particle;

    private CastelController castel;

    [Header("drop interface")]
    [SerializeField] private Rigidbody _rb;
    private BoxCollider collider;
    [SerializeField] private ParticleSystem dropParticle;
    [SerializeField] private GameObject dropObj;

    private Vector3 startPos;

    private void Start()
    {
        if (_rb == null) _rb.GetComponent<Rigidbody>();
        collider = GetComponent<BoxCollider>();
        castel = transform.parent.GetComponent<CastelController>();
        if (MaterialAsset.Instance) material.material = MaterialAsset.Instance.SelectColor(healPoint);
        text.text = healPoint.ToString();

        StartCoroutine(StartMove());
    }

    private IEnumerator StartMove()
    {
        canUse = false;
        startPos = transform.position;
        collider.isTrigger = true;
        _rb.isKinematic = true;

        

        transform.position = new Vector3(transform.position.x + Random.Range(-20, 20), transform.position.y + Random.Range(2, 15), transform.position.z + Random.Range(-20, 20));
        Transform pos = transform;
        while (Vector3.Distance(transform.position, startPos) > 0.01f)
        {
            transform.position = Vector3.Lerp(pos.position, startPos, 0.05f);
            yield return new WaitForFixedUpdate();
        }

        collider.isTrigger = false;
        _rb.isKinematic = false;

        canUse = true;
    }

    public void UpdatePoint(int point)
    {
        healPoint -= point;
        castel.PointsControlls();

        if (healPoint <= 0)
        {
            if (particle != null)
            {
                particle.gameObject.SetActive(true);
                particle.transform.SetParent(transform.parent);
                particle.Play();
            }
            else
                Debug.Log("Particl is null");

            GameObject go = Instantiate(dropObj, transform.position, transform.rotation);
            for (int i = 0; i < go.transform.childCount; i++)
                if (go.transform.GetChild(i).GetComponent<MeshRenderer>())
                    go.transform.GetChild(i).GetComponent<MeshRenderer>().material = material.material;

            gameObject.SetActive(false);
            castel.UpPointInController(1);
            return;
        }

        material.material = MaterialAsset.Instance.SelectColor(healPoint);
        text.text = healPoint.ToString();
    }

    private bool fly = false;
    private bool canUse = false;
    private Vector3 distance;
    private void FixedUpdate()
    {
        if (!StaticGameController.Instance.gameIsPlayed && !canUse) return;

        if (!fly && _rb.velocity.y < 0)
        {
            Collider col = GetComponent<Collider>();
            if (!Physics.Raycast(new Vector3(transform.position.x, col.bounds.min.y, transform.position.z), Vector3.up * -1, 0.5f))
            {
                fly = true;
                distance = gameObject.transform.position;
            }
        }
        else
            if (fly && dropParticle != null)
            {
                fly = false;
                if (Vector3.Distance(gameObject.transform.position, distance) > 0.05f)
                    dropParticle.Play();
            }
    }
}
