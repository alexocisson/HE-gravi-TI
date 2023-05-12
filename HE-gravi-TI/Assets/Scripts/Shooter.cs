using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine;

public class Shooter : NetworkBehaviour
{

    private Transform shootingPoint;
    private Transform gun;
    private Transform lifebar;

    public GameObject bulletPrefab;


    private bool isFacingRight = true;


    NetworkVariableFloat sharedAngle;
    NetworkVariableBool sharedIsFacedRight;

    private void Awake()
    {
        sharedAngle = new NetworkVariableFloat(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            SendTickrate = 10
        }, 0);
        sharedIsFacedRight = new NetworkVariableBool(new NetworkVariableSettings {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            SendTickrate = 10
        },true);

    }

    // Start is called before the first frame update
    void Start()
    {
        shootingPoint = transform.Find("gun/shootingPoint");
        gun = transform.Find("gun");
        lifebar = transform.Find("lifebarContainer");

    }

    // Update is called once per frame
    void Update()
    {
        if (IsLocalPlayer)
        {
            UpdateLocal();
        } else {
            gun.rotation = Quaternion.Euler(0, 0, sharedAngle.Value);
            Vector3 theScale = transform.localScale;
            theScale.x = sharedIsFacedRight.Value ? 1 : -1;
            transform.localScale = theScale;

            Vector3 scaleLifebar = lifebar.localScale;
            scaleLifebar.x = Mathf.Abs(scaleLifebar.x) * Mathf.Sign(theScale.x);
            lifebar.localScale = scaleLifebar;
        }
    }

    void UpdateLocal()
    {
        Vector3 mousePos = CrosshairController.crossHairPosition;

        Vector3 gun2Cursor = mousePos - shootingPoint.position;
        gun2Cursor.z = 0;


        float angle = Mathf.Rad2Deg * Mathf.Atan2(gun2Cursor.y, gun2Cursor.x);

        if (mousePos.x > transform.position.x != isFacingRight)
        {
            isFacingRight = !isFacingRight;
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;

            Vector3 scaleLifebar = lifebar.localScale;
            scaleLifebar.x *= -1;
            lifebar.localScale = scaleLifebar;
            sharedIsFacedRight.Value = isFacingRight;

        }
        if (!isFacingRight)
        {
            angle += 180;
        }
        sharedAngle.Value = angle;
        gun.rotation = Quaternion.Euler(0, 0, angle);


        if(CrosshairController.isShooting)
        {
            shootServerRpc(gun2Cursor);
        }

    }


    [ServerRpc]
    void shootServerRpc(Vector3 direction)
    {
        
        GameObject bulletObj = Instantiate(bulletPrefab, shootingPoint.position, Quaternion.identity);

        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bullet.init(direction.normalized, this.NetworkObjectId);
        bullet.GetComponent<NetworkObject>().Spawn();

    }
}
