using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Assets.Scripts.Models;
using Photon.Realtime;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        if (string.IsNullOrEmpty(Account.Instance.@class))
        {
            SceneManager.LoadScene("NewSignIn");
        }
        else
        {
            SceneManager.LoadScene("MenuGame");
        }
    }


}
