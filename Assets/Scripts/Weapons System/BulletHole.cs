using UnityEngine;
using System.Collections;

public class BulletHole : PoolObject
{
    [SerializeField] private float lifetime;
    
    [SerializeField] private GlobalPoolingManager poolingManager;

    private Coroutine returnToPool;

    void OnEnable()
    {
        if(returnToPool == null) returnToPool = StartCoroutine(ReturnToPool());
    }

    

    public override IEnumerator ReturnToPool()
    {
        yield return new WaitForSeconds(lifetime);
        poolingManager.ReturnObject(this);
        returnToPool = null;
    }

    public override void SetPoolManager(GlobalPoolingManager poolingManager)
    {
        this.poolingManager = poolingManager;
    }

}
