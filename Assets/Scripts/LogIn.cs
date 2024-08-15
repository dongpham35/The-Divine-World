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
    public FirebaseManager firebasemanager;

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
        //Check new Login and instanctiate new data
        var ChecknewLogin = databaseReference.Child("Account").Child(username).GetValueAsync();
        yield return new WaitUntil(predicate: () => ChecknewLogin.IsCompleted);
        if (ChecknewLogin.Result.ChildrenCount <= 2)// when new login, account has two field data: password, email
        {
            foreach (var child in ChecknewLogin.Result.Children)
            {
                if (child.Key.Equals("password"))
                {
                    string pass = child.Value.ToString();
                    if (pass.Equals(password))
                    {
                        StartCoroutine(getAccount(username));
                    }
                    else
                    {
#if UNITY_EDITOR
                    EditorUtility.DisplayDialog("Thông báo", "username or password is incorrect", "Ok");
#endif
                        yield return null;
                    }
                }
                
            }
            
            yield return null;
        }
        else
        {
            foreach (var child in ChecknewLogin.Result.Children)
            {
                if (child.Key.Equals("password"))
                {
                    string pass = child.Value.ToString();
                    if (pass.Equals(password))
                    {
                        StartCoroutine(firebasemanager.GetAccount(username));
                    }
                    else
                    {
#if UNITY_EDITOR
                        EditorUtility.DisplayDialog("Thông báo", "username or password is incorrect", "Ok");
#endif
                        yield return null;
                    }
                }

            }
        }

/*        var data = databaseReference.Child("Account").Child(username).Child("password").GetValueAsync();
        yield return new WaitUntil(predicate: () => data.IsCompleted);
        try
        {
            if (data.Result.Value.Equals(password))
            {
                StartCoroutine(firebasemanager.GetAccount(username));
            }
            else
            {
#if UNITY_EDITOR
                EditorUtility.DisplayDialog("Thông báo", "username or password is incorrect", "Ok");
#endif
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }*/

        
        
    }
    public static int LargestPower(int n)
    {
        if (n == 0) return 0;
        return (int)Mathf.Floor(Mathf.Log(n, 2));
    }

    IEnumerator getAccount(string username)
    {
        var data = databaseReference.Child("Account").Child(username.ToString()).GetValueAsync();

        yield return new WaitUntil(predicate: () => data.IsCompleted);
        DataSnapshot snapshot = data.Result;
        if (snapshot.Exists)
        {
            Account.Instance.username = username.ToString();
            foreach (var i in snapshot.Children)
            {
                if (i.Key.Equals("email"))
                {
                    Account.Instance.email = i.Value.ToString();
                    continue;
                }
                if (i.Key.Equals("password"))
                {
                    Account.Instance.password = i.Value.ToString();
                    continue;
                }
            }
            SceneManager.LoadScene("NewSignIn");
        }
        else
        {
            Debug.Log("User not found.");
        }
    }
}
