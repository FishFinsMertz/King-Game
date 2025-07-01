using UnityEngine;
using System.Collections.Generic;

public class ObjPool : MonoBehaviour
{
    [Header("Spawned Prefab")]
    public GameObject spawnedPrefab;

    [Header("Pool Stats")]
    public int poolSizeCap;

    private Queue<GameObject> objPool;

    void Start()
    {
        PopulatePool();
    }

    private void PopulatePool()
    {
        objPool = new Queue<GameObject>();
        for (int i = 0; i < poolSizeCap; i++)
        {
            GameObject pooledObject = Instantiate(spawnedPrefab, transform);
            pooledObject.SetActive(false);
            objPool.Enqueue(pooledObject);
        }
    }

    public GameObject SpawnObject(Vector2 position)
    {
        if (objPool.Count == 0)
        {
            Debug.LogWarning("Object pool exhausted!");
            return null;
        }

        GameObject obj = objPool.Dequeue();
        obj.transform.position = position;

        // Assign the pool before activation
        PooledObjects pooled = obj.GetComponent<PooledObjects>();
        if (pooled != null)
        {
            pooled.SetPool(this);
        }
        else
        {
            Debug.LogError("Pooled object does not inherit from PooledObject!");
        }

        obj.SetActive(true);
        return obj;
    }

    public void RecycleObject(GameObject obj)
    {
        obj.SetActive(false);
        objPool.Enqueue(obj);
    }

    void Update()
    {
        /*
        if (Input.GetMouseButtonDown(0)) // Left mouse button clicked
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            worldPos.z = 0; // Assuming 2D game in XY plane

            GameObject obj = SpawnObject(worldPos);
            if (obj == null)
            {
                Debug.LogWarning("Failed to spawn object: pool exhausted");
            }
        }

        // Recycle one active object when pressing Y
        if (Input.GetKeyDown(KeyCode.Y))
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.activeInHierarchy)
                {
                    RecycleObject(child.gameObject);
                    Debug.Log("Recycled one object");
                    break;
                }
            }
        }
        */
    }
}
