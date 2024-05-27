using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.Serialization.Json;
using Assets.Scripts.Models;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System;

public class LogIn : MonoBehaviourPunCallbacks
{
    public TMP_InputField usernameInputField;
    public TMP_InputField passwordInputField;
    public GameObject panel_loading;

    private string username;
    private string password;
    private string URL = "http://localhost/TheDiVWorld/api/";
    private AudioSource soundTrack;

    private void Start()
    {
        
        if (PhotonNetwork.InLobby)
        {
            panel_loading.SetActive(false);
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            panel_loading.SetActive(true);
        }
        soundTrack = GetComponent<AudioSource>();
        soundTrack.Play();
        if (PlayerPrefs.HasKey("volume"))
        {
            float volume = PlayerPrefs.GetFloat("volume");
            AudioListener.volume = volume;
        }
    }

    public override void OnConnectedToMaster()
    {
        panel_loading.SetActive(false);
        PhotonNetwork.JoinLobby();
    }
    public void SignIn()
    {
        foreach(var i in PhotonNetwork.PlayerList)
        {
            Debug.Log("Nick name: " + i.NickName);
        }
        Player player = PhotonNetwork.PlayerList.FirstOrDefault(p => p.NickName.Equals(usernameInputField.text));
        if(player != null)
        {
#if UNITY_EDITOR
            EditorUtility.DisplayDialog("Thong bao", "Tai khoan da dang nhap tai noi khac", "Oke");
#endif
            return;
        }
        username = usernameInputField.text;
        password = passwordInputField.text;
        StartCoroutine(SendLoginRequest());
    }
    public void SingUp()
    {
        SceneController.Instance.MoveToSignUp();
    }
    IEnumerator SendLoginRequest()
    {
        string url = URL + "Account?username=" + username;
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            if(request.result != UnityWebRequest.Result.Success)
            {
#if UNITY_EDITOR
                EditorUtility.DisplayDialog("Thông báo", request.error, "Ok");
#endif
            }
            else
            {
                
                string json = request.downloadHandler.text;
                SimpleJSON.JSONNode stats = SimpleJSON.JSON.Parse(json);
                if (stats == null)
                {
#if UNITY_EDITOR
                    EditorUtility.DisplayDialog("Thông báo", "Tên tài khoản không tồn tại", "Ok");
#endif
                }
                else
                {
                    string sql_password = stats["password"].ToString().Replace('"', ' ').Replace(" ", "");
                    if (password.Equals(sql_password))
                    {
                        Account.Instance.username = stats["username"].ToString().Replace('"', ' ').Replace(" ", "");
                        Account.Instance.avatar = stats["avatar"].ToString().Replace('"', ' ').Replace(" ", "");
                        Account.Instance.email = stats["email"].ToString().Replace('"', ' ').Replace(" ", "");
                        Account.Instance.password = sql_password;
                        Account.Instance.gold = int.Parse(stats["gold"]);
                        Account.Instance.levelID = int.Parse(stats["levelID"]);
                        Account.Instance.@class = stats["class"].ToString().Replace('"', ' ').Replace(" ", "");
                        Account.Instance.experience_points = int.Parse(stats["experience_points"]);
                        Account.Instance.level = LargestPower(Account.Instance.experience_points);
                        SceneManager.LoadScene("Loading"); 
                    }
                    else
                    {
#if UNITY_EDITOR
                        EditorUtility.DisplayDialog("Thông báo", "Mật khẩu không đúng", "Ok");
#endif
                    }
                }
                
            }
            request.Dispose();
        }
    }
    public static int LargestPower(int n)
    {
        n |= (n >> 1);
        n |= (n >> 2);
        n |= (n >> 4);
        n |= (n >> 8);
        n |= (n >> 16);
        return n - (n >> 1);
    }


}
