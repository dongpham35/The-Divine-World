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
        PhotonNetwork.JoinLobby();
    }


    public override void OnJoinedLobby()
    {
        if (string.IsNullOrEmpty(Account.Instance.classname))
        {
            SceneManager.LoadScene("NewSignIn");
        }
        else
        {
            SceneManager.LoadScene("MenuGame");
        }
    }


}
