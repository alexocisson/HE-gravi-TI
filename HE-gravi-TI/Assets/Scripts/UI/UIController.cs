using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Transports.UNET;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject serverField;

    public void Host()
    {
        NetworkManager.Singleton.StartHost();
        HideMenu();
        
    }

    public void Join()
    {
        //NetworkManager.Singleton
        string ip = serverField.GetComponent<InputField>().text;
        if (ip == "" || ip == "localhost")
        {
            ip = "127.0.0.1";
        }

        NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectAddress = ip;
        //NetworkManager.Singleton.GetComponent<UNetTransport>().ConnectAddress = "157.26.66.73";
        NetworkManager.Singleton.StartClient();
        HideMenu();
        
    }

    private void HideMenu()
    {
        menuPanel.SetActive(false);
        SceneController.singleton.GameStart();
        hasBegun = true;
    }

    public void chooseColor()
    {
        SceneManager.LoadScene(1);
    }

    public static bool hasBegun = false;

}
