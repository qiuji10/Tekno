using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler<T> where T : MonoBehaviour
{
    private static ObjectPooler<T> instance;
    private T prefab;
    private int poolSize;
    private List<T> pooledObjects;
    private GameObject parentObject;

    public static ObjectPooler<T> Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ObjectPooler<T>();
            }
            return instance;
        }
    }

    public ObjectPooler()
    {
        pooledObjects = new List<T>();
    }

    public void Initialize(T prefab, int poolSize)
    {
        this.prefab = prefab;
        this.poolSize = poolSize;
    }

    private void CreateParentObjectIfNeeded()
    {
        if (parentObject == null)
        {
            parentObject = new GameObject("ObjectPool_" + typeof(T).Name);
        }
    }

    public T GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].gameObject.activeInHierarchy)
            {
                pooledObjects[i].gameObject.SetActive(true);
                return pooledObjects[i];
            }
        }

        T newObj = Object.Instantiate(prefab);
        newObj.gameObject.SetActive(true);
        pooledObjects.Add(newObj);

        CreateParentObjectIfNeeded();
        newObj.transform.SetParent(parentObject.transform);

        return newObj;
    }

    public void Release(T obj)
    {
        obj.gameObject.SetActive(false);
        CreateParentObjectIfNeeded();
        obj.transform.SetParent(parentObject.transform);
    }
}
