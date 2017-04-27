﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CanvasControl : MonoBehaviour {

    public GameObject m_ConnectButtons;
    public GameObject m_LobbyButtons;


	public void StartConnectionAsServer()
    {
       if (OnlineManager.s_Singleton.StartServer()) Debug.Log("Server initialized correctly."); else Debug.Log("Server initialization fault.");
       m_ConnectButtons.SetActive(false);
    }

    public void StartConnectionAsClient()
    {
        OnlineManager.s_Singleton.StartClient();
        m_ConnectButtons.SetActive(false);
        m_LobbyButtons.SetActive(true);

    }

    public void StartConnectionAsHost()
    {
        OnlineManager.s_Singleton.StartHost();
        m_ConnectButtons.SetActive(false);
        m_LobbyButtons.SetActive(true);
    }

    public void SelectClass(int n)
    {
        LocalGameManager.Instance.m_LocalPlayer.GetComponent<PlayerManager>().setLocalClass(n);
    }

    public void ReadyToPlay()
    {
        LocalGameManager.Instance.m_LocalPlayer.GetComponent<NetworkLobbyPlayer>().readyToBegin = true;
        m_LobbyButtons.SetActive(false);
    }
}