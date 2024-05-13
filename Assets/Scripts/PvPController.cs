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

public class PvPController : MonoBehaviourPunCallbacks
{
    public GameObject[] characterPrefabs;
    public GameObject panel_Exit;

    private float distanceMultiplayer;
    public float smoothSpeed = 0.125f;
    private int minSizecamera = 5;
    private int maxSizecamera = 20;


    private static List<GameObject> players = new List<GameObject>();
    private int indexCharacter;
    private Vector2 point1 = new Vector2(-4f, -3.5f);
    private Vector2 point2 = new Vector2(4f, -3.5f);

    private  GameObject player;
    private bool isFinish;


    private AudioSource soundTrack;

    private void Start()
    {
        isFinish = false;
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
    }

    private void Update()
    {
        
        if (players.Count == 1)
        {
            Camera.main.transform.position = players[0].transform.position + new Vector3(0, 0 , -10);
            Camera.main.transform.SetParent(players[0].transform);
        }else if(players.Count == 2)
        {
            Camera.main.transform.SetParent(null);

            distanceMultiplayer = Vector3.Distance(players[0].transform.position, players[1].transform.position);
            if (distanceMultiplayer / 2 > Camera.main.orthographicSize && Camera.main.orthographicSize < maxSizecamera)
            {
                Camera.main.orthographicSize += 0.1f;
            }
            if (distanceMultiplayer / 2 < Camera.main.orthographicSize && Camera.main.orthographicSize > minSizecamera)
            {
                Camera.main.orthographicSize -= 0.1f;
            }
            Vector3 middlePoint = (players[0].transform.position + players[0].transform.position) / 2f;
            Vector3 desiredPosition = middlePoint;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            Vector3 temp = new Vector3(0, 0, -10);
            transform.position = smoothedPosition + temp;
        }

        if (!isFinish && players.Count == 2)
        {
            if (players[0].GetComponent<PlayerController>().currentHealth == 0)
            {

            }
            if(players[1].GetComponent<PlayerController>().currentHealth == 0)
            {

            }
        }

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
        SceneManager.LoadScene("MenuGame");
    }



    public void btnCancle()
    {
        panel_Exit.SetActive(false);
    }

    
}
