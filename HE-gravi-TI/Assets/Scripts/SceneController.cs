using System.Collections;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;

public class SceneController : NetworkBehaviour
{

    public GameObject healthpackPrefab;


    // Start is called before the first frame update
    void Start()
    {
        singleton = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void GameStart()
    {
        if (IsServer)
        {
            GameObject healthpacks = GameObject.Find("Healthpacks");
            foreach (Transform child in healthpacks.transform)
            {
                GameObject healthpack = Instantiate(healthpackPrefab, child.position, Quaternion.identity);
                healthpack.GetComponent<NetworkObject>().Spawn();
            }
        }
    }

    public Vector2 getSpawnPoint()
    {
        GameObject spawnpoints = GameObject.Find("Spawnpoints");
        List<Transform> transforms = new List<Transform>();
        foreach (Transform child in spawnpoints.transform)
        {
            transforms.Add(child);
        }

        return transforms[Random.Range(0, transforms.Count)].transform.position;
    }

    public static SceneController singleton;
}
