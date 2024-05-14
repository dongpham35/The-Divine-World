using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.Serialization.Json;
using Assets.Scripts.Models;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class LogIn : MonoBehaviour
{
    public TMP_InputField usernameInputField;
    public TMP_InputField passwordInputField;

    private string username;
    private string password;
    private string URL;
    private AudioSource soundTrack;

    private void Start()
    {
        soundTrack = GetComponent<AudioSource>();
        soundTrack.Play();
    }
    public void SignIn()
    {
        username = usernameInputField.text;
        password = passwordInputField.text;
        URL = "http://localhost/TheDiVWorld/api/Account?username=" + username;
        StartCoroutine(SendLoginRequest());
    }
    public void SingUp()
    {
        SceneController.Instance.MoveToSignUp();
    }
    IEnumerator SendLoginRequest()
    {
        using(UnityWebRequest request = UnityWebRequest.Get(URL))
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
                        SceneController.Instance.MoveToLoading();
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
}
