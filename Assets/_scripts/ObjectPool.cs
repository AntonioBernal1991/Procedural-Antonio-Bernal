using System.Collections.Generic;
using UnityEngine;




//Object pooling for better management of the cubes intances
public class ObjectPool : IObjectPool
{
    private Queue<GameObject> _pool = new Queue<GameObject>();
    private GameObject _prefab;

    public ObjectPool(GameObject prefab, int initialSize)
    {
        this._prefab = prefab;

        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Object.Instantiate(prefab);
            obj.SetActive(false);
            _pool.Enqueue(obj);
        }
    }

    public GameObject GetObject()
    {
        if (_pool.Count > 0)
        {
            GameObject obj = _pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }

        GameObject newObj = Object.Instantiate(_prefab);
        return newObj;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        _pool.Enqueue(obj);
    }
}
