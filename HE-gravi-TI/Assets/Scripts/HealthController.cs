using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

public class HealthController : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsServer)
        {
            LifeComponent life = collision.GetComponent<LifeComponent>();
            if (life != null)
            {
                life.HealClientRpc(30);
                Destroy(gameObject);
            }
        }
    }


}
