using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    public GameObject imageController;
    IController controller;
    IController mouseController;
    // Start is called before the first frame update
    void Start()
    {
        controller = imageController.GetComponent<ImageProcessing>();
        mouseController = new MouseController();
    }

    // Update is called once per frame
    void Update()
    {
        if (UIController.hasBegun)
        {
            //Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //transform.position = new Vector3(mousePos.x, mousePos.y, transform.position.z);
            if (ChooseColor.hasChoosen)
            {
                crossHairPosition = Camera.main.ScreenToWorldPoint(controller.GetPosition());
                crossHairPosition.z = -1;
                
                isShooting = controller.IsShooting();
            } else
            {
                crossHairPosition = Camera.main.ScreenToWorldPoint(mouseController.GetPosition());
                crossHairPosition.z = -1;
                isShooting = mouseController.IsShooting();

            }
            transform.position = crossHairPosition;


        }
    }

    public static Vector3 crossHairPosition;
    public static bool isShooting;
}
