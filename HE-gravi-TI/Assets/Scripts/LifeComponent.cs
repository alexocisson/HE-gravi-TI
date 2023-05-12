using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

public class LifeComponent : NetworkBehaviour
{

    public GameObject lifebar;

    public float maxLife = 1000;
    //public float currentLife = 1000;
    public NetworkVariableFloat currentLife;

    public GameObject gameOverPrefab;
    public GameObject bloodExplosionPrefab;


    void Awake()
    {
        currentLife = new NetworkVariableFloat(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.OwnerOnly }, maxLife);
        currentLife.OnValueChanged += updateLife;
    }

    // Start is called before the first frame update
    void Start()
    {
        //currentLife = maxLife;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void hit(float dammage)
    {
        if (IsLocalPlayer)
        {
            currentLife.Value -= dammage;
            if (currentLife.Value <= 0)
            {
                currentLife.Value = 0;
                //DieClientRpc(); // Can't call ClientRPC from Client
                DieServerRpc();
            } else if (currentLife.Value > maxLife)
            {
                currentLife.Value = maxLife;
            }
        }
    }

    private void updateLife(float previous, float value)
    {
        float scaleX = value / (float)maxLife;
        lifebar.transform.localScale = new Vector3(scaleX, 1, 1);
        lifebar.transform.localPosition = new Vector3((scaleX - 1) / 4, 0, 0);
    }


    [ServerRpc]
    private void DieServerRpc()
    {
        DieClientRpc();
    }

    [ClientRpc]
    private void DieClientRpc()
    {
        Debug.Log("destroy");
        Explode();
        if (IsServer)
            Destroy(gameObject);

        if (IsLocalPlayer)
        {
            Instantiate(gameOverPrefab, GameObject.Find("Canvas").transform);
        }


    }


    void Explode(bool blood = false)
    {
        GameObject explosion = Instantiate(bloodExplosionPrefab, transform.position, Quaternion.identity);

        Destroy(explosion, 4f);
        explosion.GetComponent<ParticleSystem>().Play();


    }

    [ClientRpc]
    public void HealClientRpc(float h)
    {
        hit(-h);
    }


}
