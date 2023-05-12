using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cameraman9000 : MonoBehaviour
{

    public float angle = 0;
    public float speed = 0.6f;
    public float rayon = 5;
    // Update is called once per frame
    void Update()
    {
        if (!UIController.hasBegun)
        {
            transform.position = new Vector3(Mathf.Cos(angle) * rayon, Mathf.Sin(angle) * rayon, transform.position.z);
            angle += speed * Time.deltaTime;
        }
    }
}
