using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.Serialization.Json;
using Assets.Scripts.Models;
using UnityEngine.UI;
using TMPro;

public class LogIn : MonoBehaviour
{
    public TMP_InputField usernameInputField;
    public TMP_InputField passwordInputField;

    private string username;
    private string password;
    private string URL;

    public void SignIn()
    {
        username = usernameInputField.text;
        password = passwordInputField.text;
        URL = "http://localhost/TheDiVWorld/api/Account?username=" + username;
        StartCoroutine(SendLoginRequest());
    }

    IEnumerator SendLoginRequest()
    {
        using(UnityWebRequest request = UnityWebRequest.Get(URL))
        {
            yield return request.SendWebRequest();
            if(request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
            }
            else
            {
                string json = request.downloadHandler.text;
                SimpleJSON.JSONNode stats = SimpleJSON.JSON.Parse(json);
                string sql_password = stats["password"].ToString().Replace('"',' ').Replace(" ", "");
                if (password.Equals(sql_password))
                {
                    SceneController.Instance.MoveMenuGame();
                }
                else
                {
                    Debug.Log("Tên tài khoản hoặc mật khẩu không đúng!");
                }
            }
            request.Dispose();
        }
    }
}
