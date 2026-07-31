using UnityEngine;
using System.Collections;

public abstract class PoolObject : MonoBehaviour
{
    public abstract void SetPoolManager(GlobalPoolingManager poolingManager);
    public abstract IEnumerator ReturnToPool();

}
