using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public NetworkManager manager;
    public GameObject mainMenu;
    private NetworkManagerHUD networkHUD;

    private void Awake()
    {
        mainMenu = transform.Find("Main Menu").gameObject;
        networkHUD = manager.GetComponent<NetworkManagerHUD>();
    }

    public void Update()
    {
        if (NetworkClient.active)
        {
            mainMenu.SetActive(false);
        }
        else
        {
            mainMenu.SetActive(true);
        }
    }

    public void HostGame() 
    {
        //HideMainMenu();
        manager.StartHost();
    }

    public void JoinGame() 
    {
        //HideMainMenu();
        manager.networkAddress = "139.216.63.221"; // TEMP!
        manager.StartClient();
    }

    public void StartServerOnly()
    {
        //HideMainMenu();
        manager.StartServer();
    }

    public void ExitGame() 
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    public void HideMainMenu()
    {
        this.GetComponent<Image>().sprite = null;
    }

}
