using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using OpenCvSharp.Util;
using OpenCvSharp.Tracking;
using System.Threading.Tasks;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices;
using System.Linq;
using UnityEngine.SceneManagement;

public class ChooseColor : MonoBehaviour
{
    // Video size
    private const int imWidth = 1280;
    private const int imHeight = 720;
    private int index = 0;


    public static Color colorGreen;
    public static Color colorRed;

    public static bool hasChoosen = false;

    void Start()
    {
        index = 0;

        // initialized the webcam texture by the specific device number if null
        if (ImageProcessing.webcamTexture == null)
        {
            // initialized the webcam texture by the specific device number
            ImageProcessing.webcamTexture = new WebCamTexture(imWidth, imHeight);
        }


        // Play the video source
        ImageProcessing.webcamTexture.Play();

        // Set the webcam texture to the squad
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = ImageProcessing.webcamTexture;
    }


    void OnMouseDown()
    {
        // Get the position of the object
        Vector3 objectPos = this.transform.position;

        // Get the scale of the object
        Vector3 scale = this.transform.localScale;

        // Get the center of the object
        Vector3 centerOffset = scale / 2;

        // Coordinate of click inside the cube in game cooridnate
        Vector3 gameCoord = Camera.main.ScreenToWorldPoint(Input.mousePosition) - objectPos + centerOffset;

        // Get the uv coord
        int x = Mathf.FloorToInt(gameCoord.x / scale.x * ImageProcessing.webcamTexture.width);
        int y = Mathf.FloorToInt(gameCoord.y / scale.y * ImageProcessing.webcamTexture.height);

        const int c = 6; // Get a square of c over c pxls

        // Get all the pixel of the square
        Color[] pixels = ImageProcessing.webcamTexture.GetPixels(Math.Max(0, x - (int)(c/2)), Math.Max(0, y - (int)(c / 2)), c, c);

        // Get the mean of the color
        Color color = pixels.Aggregate(Color.black, (acc, p) => new Color(acc.r + p.r / c*c, acc.g + p.g / c*c, acc.b + p.b / c*c));

        // Set the new color to track
        //colorToTrack = color;

        if (index == 0)
        {
            colorGreen = color;
        } else if (index == 1)
        {
            colorRed = color;
        } else if (index == 2)
        {
            hasChoosen = true;
            Renderer renderer = GetComponent<Renderer>();
            renderer.material.mainTexture = null;
            SceneManager.LoadScene(0);
        }
        index++;
    }

    // close the opencv window
    public void OnDestroy()
    {
        Cv2.DestroyAllWindows();
    }
}