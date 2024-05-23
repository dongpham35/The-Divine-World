using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.Networking;
using System.Linq;
using Photon.Realtime;

public class PvPController : MonoBehaviourPunCallbacks
{
    public GameObject[] characterPrefabs;
    public GameObject panel_Exit;


    private static List<GameObject> players = new List<GameObject>();
    private int indexCharacter;
    private Vector2 point1 = new Vector2(-4f, -3.5f);
    private Vector2 point2 = new Vector2(4f, -3.5f);

    private  GameObject player;

    private AudioSource soundTrack;
    private PhotonView view;

    private void Start()
    {
        panel_Exit.SetActive(false);
        soundTrack = GetComponent<AudioSource>();
        soundTrack.Play();
        if(Account.Instance.@class.Equals("air")) indexCharacter = 0;
        else if (Account.Instance.@class.Equals("water")) indexCharacter = 1;
        else if (Account.Instance.@class.Equals("earth")) indexCharacter = 2;
        else if (Account.Instance.@class.Equals("fire")) indexCharacter = 3;
        if (PhotonNetwork.CurrentRoom.Name.Contains(Account.Instance.username))
        {
            player =  PhotonNetwork.Instantiate(characterPrefabs[indexCharacter].name, point1, Quaternion.identity);
        }
        else
        {
            player =  PhotonNetwork.Instantiate(characterPrefabs[indexCharacter].name, point2, Quaternion.identity);
        }
        float volume = PlayerPrefs.GetFloat("volume");
        AudioListener.volume = volume;
        view = GetComponent<PhotonView>();
    }

    public void TurnOnExit()
    {
        panel_Exit.SetActive(true);
    }

    public void btnOk()
    {
        if(players.Count < 2)
        {
            Camera.main.transform.SetParent(null);
            PhotonNetwork.LeaveRoom();
            players.Remove(player);
        }
    }
    public override void OnLeftRoom()
    {
        if (view.IsMine)
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinLobby();
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings();
            }
        }
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("MenuGame");
    }

    public override void OnJoinedLobby()
    {
        if (view.IsMine)
        {
            SceneManager.LoadScene("MenuGame");
        }
    }

    public void btnCancle()
    {
        panel_Exit.SetActive(false);
    }



}
