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
using Firebase.Database;

public class LogIn : MonoBehaviourPunCallbacks
{

    public TMP_InputField usernameInputField;
    public TMP_InputField passwordInputField;
    public GameObject panel_loading;

    private string username;
    private string password;
    private AudioSource soundTrack;

    DatabaseReference databaseReference;

    private void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        if (PhotonNetwork.IsConnected)
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
    }
    public void SignIn()
    {
        username = usernameInputField.text;
        password = passwordInputField.text;
        StartCoroutine(CheckLogin(username, password));
    }
    public void SingUp()
    {
        SceneController.Instance.MoveToSignUp();
    }
    
    public IEnumerator CheckLogin(string username, string password)
    {
        var task = databaseReference.Child("Account").Child(username).Child("password").GetValueAsync();

        yield return new WaitUntil(predicate: () => task.IsCompleted);
        if (task.Result.Value.ToString().Equals(password))
        {
            Account.Instance.username = username;
            SceneController.Instance.MoveToLoading();
        }
        else
        {
            Debug.Log("Tài khoản hoặc mật khẩu không đúng");
        }
    }
    public static int LargestPower(int n)
    {
        if (n == 0) return 0;
        return (int)Mathf.Floor(Mathf.Log(n, 2));
    }
}
