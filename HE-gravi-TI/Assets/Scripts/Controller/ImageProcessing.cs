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



public class ImageProcessing : MonoBehaviour, IController
{
    // Video parameters
    public static WebCamTexture webcamTexture;

    // Video size
    private const int imWidth = 1280;
    private const int imHeight = 720;

    // Tracking param and objects
    private Color colorToTrack = Color.black;
    private const int MAX_AREA = 7000;
    private const int MIN_AREA = 3000;
    private const int HUE_VAR = 10;

    // Gun position
    private Point position;
    private bool isShooting;
    private const int shootingGameCounterMax = 3;
    private int shootingGameCounter = shootingGameCounterMax;

    private int index = 0;

    private Mat videoSourceImage;
    private Vec3b[] videoSourceImageData;


    public void Start()
    {
        if (ChooseColor.hasChoosen)
        {
            index = 0;

            // initialized the webcam texture by the specific device number if null
            if (webcamTexture == null)
            {
                webcamTexture = new WebCamTexture(imWidth, imHeight);
            }

            // Play the video source
            webcamTexture.Play();

            // initialize video / image with given size
            videoSourceImage = new Mat(imHeight, imWidth, MatType.CV_8UC3);
            videoSourceImageData = new Vec3b[imHeight * imWidth];
        }
    }

    private void Update()
    {
        if (ChooseColor.hasChoosen)
        {
            colorToTrack = ChooseColor.colorGreen;
            TextureToMat();

            // update the opencv window of source video
            ProcessImage(videoSourceImage);
        }
    }


    // Convert Unity Texture2D object to OpenCVSharp Mat object
    void TextureToMat()
    {
        // Color32 array : r, g, b, a
        Color32[] c = webcamTexture.GetPixels32();

        // Parallel for loop
        // convert Color32 object to Vec3b object
        // Vec3b is the representation of pixel for Mat
        Parallel.For(0, imHeight, i => {
            for (var j = 0; j < imWidth; j++)
            {
                var col = c[j + i * imWidth];
                var vec3 = new Vec3b
                {
                    Item0 = col.b,
                    Item1 = col.g,
                    Item2 = col.r
                };
                // set pixel to an array
                videoSourceImageData[j + i * imWidth] = vec3;
            }
        });
        // assign the Vec3b array to Mat
        videoSourceImage.SetArray(0, 0, videoSourceImageData);
    }

    Point? FindCircle(Mat image)
    {
        var imageHsv = new Mat(imHeight, imWidth, MatType.CV_8UC1);
        Cv2.CvtColor(image, imageHsv, ColorConversionCodes.BGR2HSV);

        //Circle color
        System.Drawing.Color color = System.Drawing.Color.FromArgb((int)colorToTrack.r, (int)colorToTrack.g, (int)colorToTrack.b);

        int hue = (int)(color.GetHue() / 2.0);

        var lower = new OpenCvSharp.Scalar(hue - HUE_VAR, 30, 30);
        var upper = new OpenCvSharp.Scalar(hue + HUE_VAR, 255, 255);

        var thresh = new Mat(imHeight, imWidth, MatType.CV_8U);
        Cv2.InRange(imageHsv, lower, upper, thresh);

        //Cv2.ImShow("Thresh", thresh);
        //Cv2.ImShow("src", image);

        /*
        var dilatedThresh = new Mat(imHeight, imWidth, MatType.CV_8U);
        Cv2.Dilate(thresh, dilatedThresh, Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(3, 3)), null, 3);

        Cv2.ImShow("Dilated thresh", dilatedThresh);
        */
        var erodedThresh = new Mat(imHeight, imWidth, MatType.CV_8U);
        Cv2.Erode(thresh, erodedThresh, Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(3, 3)), null, 10);

        //Cv2.ImShow("Eroded thresh", erodedThresh);

        var closedThresh = new Mat(imHeight, imWidth, MatType.CV_8U);
        Cv2.MorphologyEx(erodedThresh, closedThresh, MorphTypes.Close, Cv2.GetStructuringElement(MorphShapes.Ellipse, new Size(3, 3)), null, 10);


        //Cv2.ImShow("Closed thresh", closedThresh);

        //Contours
        Point[][] contours;
        HierarchyIndex[] hierarchy;

        Cv2.FindContours(closedThresh, out contours, out hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxSimple);

        if (contours.Length > 0)
        {
            double maxArea = 1.0;
            double maxCircleFactor = 0.0;
            Point[] bestContour = null;

            foreach (var contour in contours)
            {
                double area = Cv2.ContourArea(contour);
                double perimeter = Cv2.ArcLength(contour, true);
                double circleFactor = 4 * Math.PI * area / (perimeter * perimeter);

                if (circleFactor > maxCircleFactor && area < MAX_AREA && area > MIN_AREA)
                {
                    maxCircleFactor = circleFactor;
                    maxArea = area;
                    bestContour = contour;
                }
            }

            // If we find the best contour
            if (bestContour != null)
            {
                isShooting = false;
                Moments M = Cv2.Moments(bestContour);

                int cx = (int)(M.M10 / M.M00);
                int cy = (int)(M.M01 / M.M00);

                return new Point(cx, cy);
            }
            else
            {
                if (shootingGameCounter-- <= 0)
                {
                    isShooting = true;
                    shootingGameCounter = shootingGameCounterMax;
                }
            }
        }

        return null;
    }

    Rect2d box = new Rect2d(0, 0, 10, 10);
    int dx = 20;
    int dy = 20;
    // Display the original video in a opencv window
    void ProcessImage(Mat image)
    {
        Cv2.Flip(image, image, FlipMode.X);

        // Find the circle
        Point? center = FindCircle(image);
        
        if (center is Point c)
        {
            // The box
            box = new Rect2d(Math.Max(0, (int)(c.X - dx / 2)), Math.Max(0, (int)(c.Y - dy / 2)), dx, dy);

            position = new Point(c.X, c.Y);
        }


        //Cv2.Rectangle(image, box.TopLeft, box.BottomRight, new Scalar(255.0, 192.0, 203.0));

        //Cv2.ImShow("Copy video", image);
    }

    public Vector3 GetPosition()
    {
        return new Vector3((imWidth-position.X)*2, (imHeight-position.Y)*2, -1);
    }


    public bool IsShooting()
    {
        return isShooting;
    }
}
