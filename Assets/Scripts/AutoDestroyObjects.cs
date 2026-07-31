using System.Collections;
using UnityEngine;

public class AutoDestroyObjects : MonoBehaviour
{
    [SerializeField]
    private float lifetime;

    void Start()
    {
        StartCoroutine(DestroyObjectC());
    }



    private IEnumerator DestroyObjectC()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}
