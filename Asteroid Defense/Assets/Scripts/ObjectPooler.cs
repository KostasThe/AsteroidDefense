using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectPoolItem
{
    //Set the values in the Inspector
    public GameObject objectToPool;     //Our Prefab
    public string poolName;            // The pool name (e.g. missiles for the object missile); this is for better organization in the hierarchy
    public int amountToPool;           // the amount of first instantiated objects
    public bool shouldExpand = true;   // if the above number is "not enough" then grow the poolsize
}

public class ObjectPooler : MonoBehaviour
{
    public const string DefaultRootObjectPoolName = "Pooled Objects";

    public static ObjectPooler Instance;
    public string rootPoolName = DefaultRootObjectPoolName; // ...
    [HideInInspector] public List<GameObject> pooledObjects;
    public List<ObjectPoolItem> itemsToPool;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("Multiple instances of GenerateAsteroid!");
            Destroy(this);
            return;
        }
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        if (string.IsNullOrEmpty(rootPoolName))
            rootPoolName = DefaultRootObjectPoolName;

        GetParentPoolObject(rootPoolName);

        pooledObjects = new List<GameObject>();
        foreach (var item in itemsToPool)
        {
            for (int i = 0; i < item.amountToPool; i++)
            {
                CreatePooledObject(item);
            }
        }
    }

    private GameObject GetParentPoolObject(string objectPoolName)
    {
        // Use the root object pool name if no name was specified
        if (string.IsNullOrEmpty(objectPoolName))
            objectPoolName = rootPoolName;

        GameObject parentObject = GameObject.Find(objectPoolName);

        // Create the parent object if necessary
        if (parentObject == null)
        {
            parentObject = new GameObject();
            parentObject.name = objectPoolName;

            // Add sub pools to the root object pool if necessary
            if (objectPoolName != rootPoolName)
                parentObject.transform.parent = GameObject.Find(rootPoolName).transform;
        }

        return parentObject;
    }

    public GameObject GetPooledObject(string tag)
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].CompareTag(tag))
                return pooledObjects[i];
        }

        foreach (var item in itemsToPool)
        {
            if (item.objectToPool.CompareTag(tag))
            {
                if (item.shouldExpand)
                {
                    return CreatePooledObject(item);
                }
            }
        }

        return null;
    }

    private GameObject CreatePooledObject(ObjectPoolItem item)
    {
        GameObject obj = Instantiate<GameObject>(item.objectToPool);

        // Get the parent for this pooled object and assign the new object to it
        var parentPoolObject = GetParentPoolObject(item.poolName);
        obj.transform.parent = parentPoolObject.transform;

        obj.SetActive(false);
        pooledObjects.Add(obj);
        return obj;
    }
}