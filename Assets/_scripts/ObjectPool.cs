using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : IObjectPool
{
    private Queue<GameObject> pool = new Queue<GameObject>();
    private GameObject prefab;

    public ObjectPool(GameObject prefab, int initialSize)
    {
        this.prefab = prefab;

        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Object.Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject GetObject()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }

        GameObject newObj = Object.Instantiate(prefab);
        return newObj;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
