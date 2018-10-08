using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoSingleton<ObjectPoolManager>
{
    public RectTransform canvasParent;

    [Header("Pool Info")]
    public List<ObjectPoolItem> objectsToPool;

    private Dictionary<PooledObject, Queue<GameObject>> pooledObjectsDictionary;


    // Set up object pools
    public  IEnumerator CreatePools()
    {
        //Debug.Log("[PoolManager] Start creating object pool");
        this.pooledObjectsDictionary = new Dictionary<PooledObject, Queue<GameObject>>();

        // Traverse objects to pool
        for (int i = 0; i < this.objectsToPool.Count; i++)
        {
            // Create new dictionary entry for each object
            this.pooledObjectsDictionary.Add(this.objectsToPool[i].type, new Queue<GameObject>());

            // Instantiate the required amount of objects for each pool
            for (int j = 0; j < this.objectsToPool[i].amountToPool; j++)
            {
                GameObject obj;

                if (this.objectsToPool[i].isCanvasChild)
                {
                    obj = Instantiate(this.objectsToPool[i].objectToPool, this.canvasParent);
                    obj.GetComponent<RectTransform>().anchoredPosition = new Vector3(1000.0f, 1000.0f, 0.0f);
                }
                else
                {
                    obj = Instantiate(this.objectsToPool[i].objectToPool, null);
                    obj.transform.position = new Vector3(1000.0f, 1000.0f, 0.0f);
                }

                this.pooledObjectsDictionary[this.objectsToPool[i].type].Enqueue(obj);
            }
        }

        //Debug.Log("[PoolManager] Finished creating object pools");
        yield return null;
    }

    // Get pooled item
    public GameObject GetPooledObject(PooledObject _type)
    {
        // Check for an entry to return
        if (this.pooledObjectsDictionary.ContainsKey(_type))
        {
            if (this.pooledObjectsDictionary[_type].Count != 0)
            {
                return this.pooledObjectsDictionary[_type].Dequeue();
            }
        }

        // If there is no obj available, check if the object pool is set to expand, if so create a new obj for the pool
        for (int i = 0; i < this.objectsToPool.Count; i++)
        {
            if (this.objectsToPool[i].type == _type)
            {
                if (this.objectsToPool[i].isExpanding)
                {
                    GameObject obj;

                    if (this.objectsToPool[i].isCanvasChild)
                    {
                        obj = Instantiate(this.objectsToPool[i].objectToPool, this.canvasParent);
                        obj.GetComponent<RectTransform>().anchoredPosition = new Vector3(1000.0f, 1000.0f, 0.0f);
                    }
                    else
                    {
                        obj = Instantiate(this.objectsToPool[i].objectToPool, null);
                        obj.transform.position = new Vector3(1000.0f, 1000.0f, 0.0f);
                    }

                    return obj;
                }
            }
        }

        return null;
    }

    // Enqueue the object passed
    public void ReturnObject(PooledObject _type, GameObject _obj)
    {
        if (this.pooledObjectsDictionary.ContainsKey(_type))
        {
            _obj.transform.parent = null;
            _obj.transform.position = new Vector3(1000.0f, 1000.0f, 0.0f);
            this.pooledObjectsDictionary[_type].Enqueue(_obj);
        }
    }


    public override IEnumerator Initialize()
    {
        throw new NotImplementedException();
    }

    public override IEnumerator Run()
    {
        throw new NotImplementedException();
    }

    public override IEnumerator Stop()
    {
        throw new NotImplementedException();
    }
}

public enum PooledObject
{
    SongSelectButton = 0
}

[System.Serializable]
public class ObjectPoolItem
{
    public GameObject objectToPool;
    public PooledObject type;
    public int amountToPool;
    public bool isExpanding = true;
    public bool isCanvasChild = false;
}
