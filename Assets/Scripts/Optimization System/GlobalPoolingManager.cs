using UnityEngine;
using System.Collections.Generic;

public abstract class GlobalPoolingManager : MonoBehaviour
{

    [SerializeField] private Transform objectParent;
    [SerializeField] private PoolObject objectPrefab;
    [SerializeField] private int initialObjectsCount;


    private Queue<PoolObject> pool;
    
    protected virtual void Start()
    {
        pool = new ();
        InitPool();        
    }

    [ContextMenu("Cache Objects")]
    protected virtual void InitPool()
    {
        if(pool == null) pool = new ();
        List<PoolObject> poolObjects = new ();

        for(int i = 0; i < objectParent.childCount; i++)
        {
            poolObjects.Add(objectParent.GetChild(i).GetComponent<PoolObject>());
        }
        
        
        if(poolObjects.Count < initialObjectsCount || poolObjects.Count > initialObjectsCount)
        {
            if(poolObjects.Count != 0)
            { 
                foreach(var child in poolObjects)
                {
                    #if UNITY_EDITOR
                    DestroyImmediate(child.gameObject);
                    #else
                    Destroy(child.gameObject);
                    #endif
                }
                poolObjects.Clear();
            }

            for(int i = 0; i < initialObjectsCount; i++)
            {
                PoolObject obj = Instantiate(objectPrefab, objectParent);
                obj.SetPoolManager(this);
                obj.gameObject.SetActive(false);
                poolObjects.Add(obj);
            }
        }

        foreach(var obj in poolObjects)
        {
            pool.Enqueue(obj);
        }
        
    }


    public virtual GameObject GetObject()
    {
        PoolObject obj;

        if(pool.Count > 0)
        {
            obj = pool.Dequeue();
            obj.gameObject.SetActive(true);
        }
        else
        {
            obj = Instantiate(objectPrefab, objectParent);
            obj.SetPoolManager(this);
        }
        
        return obj.gameObject;
    }

    public virtual void ReturnObject(PoolObject obj)
    {
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }
}
