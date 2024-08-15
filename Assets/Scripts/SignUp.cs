using Assets.Scripts.Models;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
using Firebase.Database;
using Unity.VisualScripting;

public class SignUp : MonoBehaviour
{
    public TMP_InputField username_InputField;
    public TMP_InputField email_InputField;
    public TMP_InputField password_InputField;
    public TMP_InputField repassword_InputField;

    private string URL;
    private string username;
    private string email;
    private string password;
    private string repassword;
    private string data;

    DatabaseReference databaseReference;

    private AudioSource soundTrack;

    private void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        soundTrack = GetComponent<AudioSource>();
        soundTrack.Play();
        if (PlayerPrefs.HasKey("volume"))
        {
            float volume = PlayerPrefs.GetFloat("volume");
            AudioListener.volume = volume;
        }
    }


    public void SignUpAcount()
    {
        username = username_InputField.text;
        email = email_InputField.text;
        password = password_InputField.text;
        repassword = repassword_InputField.text;

        if(username.Count() > 16)
        {
#if UNITY_EDITOR
            EditorUtility.DisplayDialog("Thông báo", "Tên tài khoản không được quá 16 ký tự", "Ok");
#endif
            return;
        }
        if(password.Count() > 16)
        {
#if UNITY_EDITOR
            EditorUtility.DisplayDialog("Thông báo", "Mật không được quá 16 ký tự", "Ok");
#endif
            return;
        }    
        if(!password.Equals(repassword))
        {
#if UNITY_EDITOR
            EditorUtility.DisplayDialog("Thông báo", "Mật khẩu không đúng", "Ok");
#endif
            return;
        }
        StartCoroutine(CheckUsername(username, password, email));

    }

    IEnumerator CheckUsername(string username, string password, string email)
    {
        var task = databaseReference.Child("Account").Child(username).GetValueAsync();

        yield return new WaitUntil(predicate: () => task.IsCompleted);

        DataSnapshot datasnapshot = task.Result;

        if (datasnapshot.Exists)
        {
#if UNITY_EDITOR
            EditorUtility.DisplayDialog("Thông báo", "Tài khoản đã tồn tại", "Ok");
#endif
            yield return 0;
        }
        else
        {
            StartCoroutine(SetupIntoDb(username, password, email));
        }
    }

    IEnumerator SetupIntoDb(string username, string _password, string _email)
    {
        var data = new Dictionary<string, object>
        {
            {"password", _password},
            {"email", _email},
        };


        var task = databaseReference.Child("Account").Child(username).SetValueAsync(data);

        yield return new WaitUntil(predicate: () => task.IsCompleted);

        SignIn();
    }

    public void SignIn()
    {
        SceneController.Instance.MoveToSignIn();
    }

    


}

