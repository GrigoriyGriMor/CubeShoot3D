using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireObjDestroyModel : MonoBehaviour
{
    [SerializeField] private float lifeTime = 2;

    private void OnEnable()
    {
        StartCoroutine(DestroyObj());
    }

    private IEnumerator DestroyObj()
    {
        yield return new WaitForSeconds(lifeTime);

        Destroy(gameObject);
    }
}
