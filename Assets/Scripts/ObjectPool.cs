using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;
    [SerializeField] private GameObject soulPrefab;
    [SerializeField] private List<GameObject> pooledObjects = new List<GameObject>();
    [SerializeField] private int amountToPool = 10;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            GameObject obj = Instantiate(soulPrefab);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }
    public GameObject GetPooledObject()
    {
        for (int j = 0; j < pooledObjects.Count; j++)
        {
            if (!pooledObjects[j].activeInHierarchy)
            {
                return pooledObjects[j];
            }
        }
        return null;
    }
}
