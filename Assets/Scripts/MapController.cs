using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Assets.Scripts.Models;
using Unity.VisualScripting;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using Firebase.Database;

public class MapController : MonoBehaviourPunCallbacks
{
    DatabaseReference databaseReference;

    [SerializeField] GameObject[] characterPrefabs;
    [SerializeField] GameObject AngryPigPrefabs;
    [SerializeField] GameObject ChickenPrefabs;
    [SerializeField] TMP_Text nameMap;
    [SerializeField] TMP_Text txtExp;
    [SerializeField] TMP_Text txtGold;

    private string namecharacter;
    private int characterIndex;


    private GameObject characterSpawned;

    private List<GameObject> SpawnAngryPigs = new List<GameObject>();
    private List<GameObject> SpawnChickens = new List<GameObject>();
    private int maxAngryPig = 4;
    private int maxChicken = 5;

    private int oldExp;
    private int oldGold;

    private AudioSource soundTrack;

    private void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        soundTrack = GetComponent<AudioSource>();
        soundTrack.Play();

        if (Account.Instance.classname.Equals("air"))
        {
            namecharacter = "Aang";
            characterIndex = 0;
        } else if (Account.Instance.classname.Equals("water"))
        {
            namecharacter = "Katara";
            characterIndex = 1;
        } else if (Account.Instance.classname.Equals("earth"))
        {
            namecharacter = "Toph";
            characterIndex = 2;
        } else if (Account.Instance.classname.Equals("fire"))
        {
            namecharacter = "Zuko";
            characterIndex = 3;
        }
        characterSpawned = PhotonNetwork.Instantiate(characterPrefabs[characterIndex].name, new Vector2(-26, -4), Quaternion.identity);
        
        if (PhotonNetwork.InRoom)
        {
            nameMap.text = PhotonNetwork.CurrentRoom.Name;
        }
        oldExp = Account.Instance.experience_points;
        oldGold = Account.Instance.gold;
        StartCoroutine(SpawnAngryPigAfter15Seconds());
        StartCoroutine(SpawnChickenAfter10Seconds());
        StartCoroutine(ChangeText());

        float volume = PlayerPrefs.GetFloat("volume");
        AudioListener.volume = volume;
    }



    private void SpawnAngryPig()
    {
        if (SpawnAngryPigs.Count == 0)
        {
            for (int i = 0; i < maxAngryPig; i++)
            {
                float randomX = Random.Range(15, 22);

                GameObject angrypig = PhotonNetwork.Instantiate(AngryPigPrefabs.name, new Vector2(randomX, -6.5f), Quaternion.identity);
                AngryPig scriptAngrypig = angrypig.GetComponent<AngryPig>();
                SpawnAngryPigs.Add(angrypig);

            }
        }
        else
        {
            for (int i = 0; i < maxAngryPig; i++)
            {
                if (SpawnAngryPigs[i] != null) continue;
                float randomX = Random.Range(15, 22);

                GameObject angrypig = PhotonNetwork.Instantiate(AngryPigPrefabs.name, new Vector2(randomX, -6.5f), Quaternion.identity);
                AngryPig scriptAngrypig = angrypig.GetComponent<AngryPig>();
                SpawnAngryPigs[i] = angrypig;

            }
        }

    }

    private void SpawnChicken()
    {
        if (SpawnChickens.Count == 0)
        {
            for (int i = 0; i < maxChicken; i++)
            {
                float randomX = Random.Range(-24, -20);

                GameObject chicken = PhotonNetwork.Instantiate(ChickenPrefabs.name, new Vector2(randomX, -7.5f), Quaternion.identity);
                Chicken scriptChicken = chicken.GetComponent<Chicken>();
                SpawnChickens.Add(chicken);
            }
        }
        else
        {
            for (int i = 0; i < maxChicken; i++)
            {
                if (SpawnChickens[i] != null) continue;
                float randomX = Random.Range(-24, -20);

                GameObject chicken = PhotonNetwork.Instantiate(ChickenPrefabs.name, new Vector2(randomX, -7.5f), Quaternion.identity);
                Chicken scriptChicken = chicken.GetComponent<Chicken>();
                SpawnChickens[i] = chicken;
            }
        }

    }

    IEnumerator ChangeText()
    {
        while (true)
        {
            txtExp.text = "Exp: " + (Account.Instance.experience_points - oldExp).ToString();
            txtGold.text = "Gold: " + (Account.Instance.gold - oldGold).ToString();
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator SpawnAngryPigAfter15Seconds()
    {
        while (true)
        {
            SpawnAngryPig();
            yield return new WaitForSeconds(15f);
        }
    }

    IEnumerator SpawnChickenAfter10Seconds()
    {
        while (true)
        {
            SpawnChicken();
            yield return new WaitForSeconds(10f);
        }
    }

    public void ReturnMenuGame()
    {
        StartCoroutine(updateAccountTable(Account.Instance.username, Account.Instance.gold, Account.Instance.experience_points));
        if (PhotonNetwork.InRoom)
        {
            Camera.main.transform.SetParent(null);
            PhotonNetwork.LeaveRoom();
        }
    }
    public override void OnLeftRoom()
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

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("MenuGame");
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("MenuGame");
    }
    IEnumerator updateAccountTable(string username, int gold, int exp)
    {
        var data = new Dictionary<string, object>
        {
            {"avartar", Account.Instance.avatar },
            {"email", Account.Instance.email },
            {"password", Account.Instance.password },
            {"gold", gold },
            {"levelID", Account.Instance.levelID },
            {"class", Account.Instance.classname },
            {"level", Account.Instance.level },
            {"exp", exp }
        };

        var task = databaseReference.Child("Account").Child(username).SetValueAsync(data);

        yield return new WaitUntil(predicate: () => task.IsCompleted);
    }

}
