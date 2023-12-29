using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [HideInInspector]
    public static ObjectPool instance;
    [HideInInspector]
    public static List<List<GameObject>> pools = new List<List<GameObject>>();

    [Header("Red Blood")]
    public GameObject redBloodPrefab;
    public int redBloodPoolAmount = 20;
    List<GameObject> redBloodParticles = new List<GameObject>();

    private void Awake() {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < redBloodPoolAmount; ++i)
        {
            GameObject obj = Instantiate(redBloodPrefab);
            obj.SetActive(false);
            redBloodParticles.Add(obj);

        }

        pools.Add(redBloodParticles);
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < redBloodParticles.Count; i++)
        {
            if (!redBloodParticles[i].activeInHierarchy)
                return redBloodParticles[i];
        }
        return null;
    }
    public IEnumerator HidePooledObject(GameObject gameObject, float time)
    {
        gameObject.SetActive(false);
        yield return time;
    }
}
