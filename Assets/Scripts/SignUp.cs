using Assets.Scripts.Models;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;

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

    private AudioSource soundTrack;

    private void Start()
    {
        soundTrack = GetComponent<AudioSource>();
        soundTrack.Play();
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

        URL = "http://192.168.1.4/TheDiVWorld/api/Account";
        StartCoroutine(getAccount());
    }

    public void SignIn()
    {
        SceneController.Instance.MoveToSignIn();
    }

    IEnumerator getAccount()
    {
        using(UnityWebRequest request = UnityWebRequest.Get(URL + "?username=" + username))
        {
            yield return request.SendWebRequest();
            if(request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
            }
            else
            {
                string json = request.downloadHandler.text;
                if (!json.Equals("null", System.StringComparison.OrdinalIgnoreCase))
                {
#if UNITY_EDITOR
                    EditorUtility.DisplayDialog("Thông báo", "Tài khoản đã tồn tại", "Ok");
#endif
                }
                else
                {
                    //data = "{\"username\":\"" + username + "\", \"email\":\"" + email + "\", \"password\":\"" + password + "\"}";
                    data = string.Format($"?username={username}&email={email}&password={password}");

                    StartCoroutine(saveToAccount());
                }
            }
            request.Dispose();
        }
    }

    IEnumerator saveToAccount()
    {
        using (UnityWebRequest request = UnityWebRequest.Put(URL + data ,data))
        {
            
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {

                Debug.LogError(request.error);
            }
            else
            {
                SceneController.Instance.MoveToSignIn();
            }
            request.Dispose();
        }
    }


}
