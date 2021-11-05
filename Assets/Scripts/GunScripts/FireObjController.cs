using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireObjController : MonoBehaviour
{
    [SerializeField] private float fireForce = 50f;
    [SerializeField] private GameObject destroyObjPrefab;

    [SerializeField] private Rigidbody _rb;

    private void OnEnable()
    {
        if (_rb == null) _rb = GetComponent<Rigidbody>();

        _rb.isKinematic = false;
        StartCoroutine(OpenFire());
    }

    private IEnumerator OpenFire()
    {
        yield return new WaitForFixedUpdate();

        if (_rb != null) _rb.AddForce(transform.forward * fireForce);

        yield return new WaitForSeconds(5);

        _rb.isKinematic = true;
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
            }

            _rb.isKinematic = true;
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
            }

            _rb.isKinematic = true;
            gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        if (gameObject.transform.position.y < -5)
        {
            _rb.isKinematic = true;
            gameObject.SetActive(false);
        }
    }
}
