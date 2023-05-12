using System;
using UnityEngine;

public class MouseController : IController
{
    Vector3 IController.GetPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -1;
        return mousePos;
    }

    bool IController.IsShooting()
    {
        return Input.GetMouseButtonDown(0);
    }
}
