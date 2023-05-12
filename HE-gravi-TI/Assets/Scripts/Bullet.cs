using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

public class Bullet : NetworkBehaviour
{

    NetworkVariable<Vector3> direction;
    //public Vector3 direction;
    public float speed = 2f;

    public int lifeDuration = 100;

    private int timeToLive;

    private ulong ownerID;
    private LayerMask layerGround;

    public GameObject explosionPrefab;


    public void init(Vector3 direction, ulong ownerID)
    {
        //this.direction = direction.normalized * speed;
        this.ownerID = ownerID;
        this.direction.Value = direction.normalized * speed;
    }

    void Awake()
    {
        layerGround = LayerMask.NameToLayer("Terrain");
        this.direction = new NetworkVariable<Vector3>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.OwnerOnly }, Vector3.zero);
        timeToLive = lifeDuration;
    }

    void Update()
    {
        this.transform.position += direction.Value * Time.deltaTime;
        timeToLive--;

        if (IsServer)
        {
            if (timeToLive < 0)
            {
                ExplodeClientRpc();
            }

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, transform.localScale.x / 2, 1 << layerGround);
            for (int i = 0; i < colliders.Length; i++)
            {
                ExplodeClientRpc();
                break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.GetComponent<NetworkObject>().NetworkObjectId == ownerID)
        //    return;

        if (IsServer)
        {
            if (collision.gameObject.tag == "Player" && lifeDuration - timeToLive > 10)
            {
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();
                player.HitAndGravityChangeClientRpc(10);
                ExplodeClientRpc(true);
            }
        }
        
    }

    void Explode(bool blood = false)
    {
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        if (blood)
        {
            ParticleSystem.MainModule settings = explosion.GetComponent<ParticleSystem>().main;
            settings.startColor = new Color(1, 0, 0);
        }
        Destroy(explosion, 4f);
        explosion.GetComponent<ParticleSystem>().Play();
    }

    [ClientRpc]
    void ExplodeClientRpc(bool blood = false)
    {
        Debug.Log("Bullet Explode");
        Explode(blood);
        if (IsServer)
        {
            Debug.Log("destroy object !");
            Destroy(gameObject);
        }
    }



}
