using System.Collections;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;

public class CameraController : MonoBehaviour
{


    public float minScale = 10;
    public float maxScale = 10;

    public float smoothness= 10;
    public float smoothnessScale= 5;



    private GameObject[] players;

    private Vector2 target;
    private Vector2 velocity;

    private float targetScale;
    private float scaleVelocity;
    private Vector2 minPosition;
    private Vector2 maxPosition;

    private Camera cam;


    // Start is called before the first frame update
    void Start()
    {
        velocity = Vector2.zero;
        targetScale = 10;
        minPosition = Vector2.zero;
        maxPosition = Vector2.zero;
        cam = GetComponent<Camera>();
        players = GameObject.FindGameObjectsWithTag("Player");
    }


    // Update is called once per frame
    void Update()
    {
        if (!UIController.hasBegun)
            return;

        bool hasChanged = false;
        if (players != null)
        {
            foreach (var player in players)
            {
                if (player == null)
                {
                    hasChanged = true;
                }
            }
        }

        if (hasChanged || Time.frameCount % 60 == 0)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
        }


        Vector2 myPosition = Vector2.zero;
        // Get the center of all the players
        target = Vector2.zero;
        if (players.Length > 0)
        {
            minPosition = players[0].transform.position;
            maxPosition = players[0].transform.position;
            foreach (var player in players)
            {
                target.x += player.transform.position.x;
                target.y += player.transform.position.y;

                minPosition = Vector2.Min(minPosition, player.transform.position);
                maxPosition = Vector2.Max(maxPosition, player.transform.position);
                if (player.GetComponent<NetworkObject>().IsLocalPlayer)
                {
                    myPosition = player.transform.position;
                }
            }
            target /= players.Length;

        }

        // Calculate the camera velocity
        Vector2 position = transform.position;
        velocity = (target - position) / smoothness;


        // Physics
        position += velocity;

        transform.position = new Vector3(position.x, position.y, transform.position.z);


        // Calculate the scale velocity
        Vector2 size = (maxPosition - minPosition) / 1.8f;
        float scale = cam.orthographicSize;
        targetScale = Mathf.Max(size.x, size.y);
        scaleVelocity = (targetScale - scale) / smoothnessScale;

        scale += scaleVelocity;

        scale = Mathf.Clamp(scale, minScale, maxScale);

        // Physics
        cam.orthographicSize = scale;

        Vector3 pos2 = transform.position;

        pos2.x = Mathf.Clamp(pos2.x, myPosition.x - scale / 1.6f, myPosition.x + scale / 1.6f);
        pos2.y = Mathf.Clamp(pos2.y, myPosition.y - scale / 1.6f, myPosition.y + scale / 1.6f);
        transform.position = pos2;
    }
}
