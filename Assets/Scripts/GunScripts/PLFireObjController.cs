using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PLFireObjController : MonoBehaviour
{
    [SerializeField] private float fireSpeed = 5f;
    [SerializeField] private float parableHight = 5f;
    private float parableFackHight;
    [SerializeField] private GameObject destroyObjPrefab;
    [SerializeField] private float destroyTime = 5;

    private Vector3 targetPos;

    public void Init(Vector3 _targetPos)
    {
        targetPos = _targetPos;
        parableFackHight = 0;
        StopCoroutine(OpenFire());
        StartCoroutine(OpenFire());
    }

    private IEnumerator OpenFire()
    {
        yield return new WaitForFixedUpdate();

        float startYPos = Vector3.Distance(transform.position, targetPos);
        StopCoroutine(DestroyTime());
        StartCoroutine(DestroyTime());

        Transform startPos = transform;
        while (Vector3.Distance(transform.position, targetPos) > 0.2f)
        {
              if (Vector3.Distance(transform.position, targetPos) > (startYPos / 2))
                parableFackHight = parableHight;
              else
                parableFackHight = 0; //if (parableFackHight > 0)
                                                 //    parableFackHight -= 50 * Time.deltaTime;

            transform.position = Vector3.Lerp(startPos.position, new Vector3(targetPos.x, targetPos.y + parableFackHight, targetPos.z + 2.5f), fireSpeed);
            yield return new WaitForFixedUpdate();
        }

        gameObject.SetActive(false);
        if (destroyObjPrefab != null)
        {
            GameObject destroyObj = Instantiate(destroyObjPrefab, gameObject.transform.position, Quaternion.identity, gameObject.transform.parent);
            destroyObj.transform.position = gameObject.transform.position;
            destroyObj.transform.rotation = gameObject.transform.rotation;
            destroyObj.GetComponent<ParticleSystem>().Play();
        }
    }

    private IEnumerator DestroyTime()
    {
        yield return new WaitForSeconds(destroyTime);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<BlockController>())
        {
            collision.gameObject.GetComponent<BlockController>().UpdatePoint(1);

            if (destroyObjPrefab != null)
            {
                GameObject destroyObj = Instantiate(destroyObjPrefab, gameObject.transform.position, Quaternion.identity, gameObject.transform.parent);
                destroyObj.transform.position = gameObject.transform.position;
                destroyObj.transform.rotation = gameObject.transform.rotation;
                destroyObj.GetComponent<ParticleSystem>().Play();
            }

            StopCoroutine(OpenFire());
            gameObject.SetActive(false);
        }
        else
        if (!collision.isTrigger)
        {
            if (destroyObjPrefab != null)
            {
                GameObject destroyObj = Instantiate(destroyObjPrefab, gameObject.transform.position, Quaternion.identity, gameObject.transform.parent);
                destroyObj.transform.position = gameObject.transform.position;
                destroyObj.transform.rotation = gameObject.transform.rotation;
                destroyObj.GetComponent<ParticleSystem>().Play();
            }

            StopCoroutine(OpenFire());
            gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if (gameObject.transform.position.y < -5)
        {
            StopCoroutine(OpenFire());
            gameObject.SetActive(false);
        }
    }
}
